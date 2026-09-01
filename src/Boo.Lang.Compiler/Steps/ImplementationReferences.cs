#region license
// Copyright (c) 2026 the Boo contributors
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// Points the framework types an emitted image names at the reference
/// assemblies that export them.
/// </summary>
/// <remarks>
/// The type system reads the running runtime's implementation assemblies,
/// because the compiler executes macros and attributes out of what it
/// references and a reference assembly has no method bodies. Emission inherits
/// those live types, so the image names System.Private.CoreLib and its
/// siblings: Uri is defined by System.Private.Uri and XmlDocument by
/// System.Private.Xml.
///
/// Roslyn will not resolve a type through an implementation assembly, which
/// leaves an emitted assembly unusable from C#: a Boo type can be named, but not
/// declared, constructed, or called. The image is correct at runtime either way,
/// so only the references are rewritten.
/// </remarks>
internal static class ImplementationReferences
{
	private static readonly Lazy<ReferenceAssemblyIndex> Index =
		new(ReferenceAssemblyIndex.ForRunningRuntime);

	/// <summary>
	/// An assembly reference the pack does not have, standing in for the
	/// public assemblies that export its types.
	/// </summary>
	private sealed class Implementation
	{
		internal readonly Dictionary<string, AssemblyName> Exporters = new(StringComparer.Ordinal);
		internal readonly Dictionary<string, int> Demand = new(StringComparer.Ordinal);

		/// <summary>Whether every type named through it has an exporter.</summary>
		internal bool Complete = true;

		/// <summary>What the row is rewritten to, or null to leave it alone.</summary>
		internal AssemblyName Principal;
	}

	internal static MetadataBuilder Resolved(CompilerContext context, MetadataBuilder metadata)
	{
		return Resolved(context, metadata, Index.Value);
	}

	internal static MetadataBuilder Resolved(
		CompilerContext context, MetadataBuilder metadata, ReferenceAssemblyIndex index)
	{
		if (index == null)
		{
			// Nothing to rewrite the image with, so it goes out as generated.
			context.Warnings.Add(CompilerWarningFactory.ReferenceAssembliesNotFound());
			return metadata;
		}

		return MetadataCopier.Copy(metadata, copier =>
		{
			var implementations = ImplementationsIn(copier.Source, index);
			if (implementations.Count == 0)
				return;

			copier.ResolveAssemblyName = name =>
				implementations.TryGetValue(name, out var implementation)
					? implementation.Principal
					: null;

			var added = ReferencesAfterRenaming(copier.Source, implementations);
			copier.ResolveScope = (scope, @namespace, name) =>
			{
				if (scope.Kind != HandleKind.AssemblyReference)
					return default;

				var assembly = NameOf(copier.Source, (AssemblyReferenceHandle) scope);
				if (!implementations.TryGetValue(assembly, out var implementation))
					return default;

				// Only an assembly whose every type is accounted for is
				// rewritten, so a half repointed image is never produced.
				if (implementation.Principal == null)
					return default;

				var exporter = index.ExporterOf(ReferenceAssemblyIndex.FullName(@namespace, name));
				if (exporter == null || exporter.Name == implementation.Principal.Name)
					return default;

				if (added.TryGetValue(exporter.Name, out var handle))
					return handle;

				handle = copier.Target.AddAssemblyReference(
					copier.Target.GetOrAddString(exporter.Name),
					exporter.Version,
					default,
					copier.Target.GetOrAddBlob(exporter.GetPublicKeyToken() ?? Array.Empty<byte>()),
					default,
					default);

				added.Add(exporter.Name, handle);
				return handle;
			};
		});
	}

	/// <summary>
	/// Every assembly the image names that the reference pack does not have,
	/// with the exporters its types actually need.
	/// </summary>
	/// <remarks>
	/// Only assemblies the running framework implements without publishing are
	/// candidates, so an ordinary reference is never touched however its types
	/// are named.
	/// </remarks>
	private static Dictionary<string, Implementation> ImplementationsIn(
		MetadataReader metadata, ReferenceAssemblyIndex index)
	{
		var found = new Dictionary<string, Implementation>(StringComparer.Ordinal);

		foreach (var handle in metadata.TypeReferences)
		{
			var type = metadata.GetTypeReference(handle);
			if (type.ResolutionScope.Kind != HandleKind.AssemblyReference)
				continue;

			var assembly = NameOf(metadata, (AssemblyReferenceHandle) type.ResolutionScope);
			if (!index.IsImplementation(assembly))
				continue;

			if (!found.TryGetValue(assembly, out var implementation))
			{
				implementation = new Implementation();
				found.Add(assembly, implementation);
			}

			var exporter = index.ExporterOf(ReferenceAssemblyIndex.FullName(
				metadata.GetString(type.Namespace), metadata.GetString(type.Name)));
			if (exporter == null)
			{
				implementation.Complete = false;
				continue;
			}

			implementation.Exporters[exporter.Name] = exporter;
			implementation.Demand.TryGetValue(exporter.Name, out var tally);
			implementation.Demand[exporter.Name] = tally + 1;
		}

		AssignPrincipals(found);
		return found;
	}

	/// <summary>
	/// The identity each implementation row is rewritten to.
	/// </summary>
	/// <remarks>
	/// One row cannot become two, so each takes a different exporter and the
	/// rest are appended. The assemblies with the fewest exporters to choose
	/// from pick first, since the ones with many can always give way.
	/// </remarks>
	private static void AssignPrincipals(Dictionary<string, Implementation> implementations)
	{
		var claimed = new HashSet<string>(StringComparer.Ordinal);

		var rewritable = implementations
			.Where(pair => pair.Value.Complete && pair.Value.Exporters.Count > 0)
			.OrderBy(pair => pair.Value.Exporters.Count)
			.ThenBy(pair => pair.Key, StringComparer.Ordinal);

		foreach (var pair in rewritable)
		{
			var wanted = pair.Value.Demand
				.OrderByDescending(exporter => exporter.Value)
				.ThenBy(exporter => exporter.Key, StringComparer.Ordinal)
				.Select(exporter => exporter.Key)
				.ToList();

			var choice = wanted.FirstOrDefault(name => !claimed.Contains(name)) ?? wanted[0];
			claimed.Add(choice);
			pair.Value.Principal = pair.Value.Exporters[choice];
		}
	}

	/// <summary>
	/// The references the image has once the rows are renamed, so an exporter
	/// one of them now carries is reused rather than written a second time.
	/// </summary>
	private static Dictionary<string, EntityHandle> ReferencesAfterRenaming(
		MetadataReader metadata, Dictionary<string, Implementation> implementations)
	{
		var references = new Dictionary<string, EntityHandle>(StringComparer.Ordinal);

		foreach (var handle in metadata.AssemblyReferences)
		{
			var name = NameOf(metadata, handle);
			if (implementations.TryGetValue(name, out var implementation) &&
				implementation.Principal != null)
				name = implementation.Principal.Name;

			references[name] = handle;
		}

		return references;
	}

	private static string NameOf(MetadataReader metadata, AssemblyReferenceHandle handle)
	{
		return metadata.GetString(metadata.GetAssemblyReference(handle).Name);
	}
}
