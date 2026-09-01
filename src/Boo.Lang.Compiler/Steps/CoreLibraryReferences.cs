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
/// those live types, so the image names System.Private.CoreLib.
///
/// Roslyn will not resolve a type through the implementation assembly, which
/// leaves an emitted assembly unusable from C#: a Boo type can be named, but not
/// declared, constructed, or called. The image is correct at runtime either way,
/// so only the references are rewritten.
/// </remarks>
internal static class CoreLibraryReferences
{
	private const string Implementation = "System.Private.CoreLib";
	private const string CoreReference = "System.Runtime";

	private static readonly Lazy<ReferenceAssemblyIndex> Index =
		new(ReferenceAssemblyIndex.ForRunningRuntime);

	internal static MetadataBuilder Resolved(CompilerContext context, MetadataBuilder metadata)
	{
		return Resolved(context, metadata, Index.Value);
	}

	internal static MetadataBuilder Resolved(
		CompilerContext context, MetadataBuilder metadata, ReferenceAssemblyIndex index)
	{
		var core = index?.NameOf(CoreReference);
		if (core == null)
		{
			// Nothing to rewrite the image with, so it goes out as generated.
			context.Warnings.Add(CompilerWarningFactory.ReferenceAssembliesNotFound());
			return metadata;
		}

		return MetadataCopier.Copy(metadata, copier =>
		{
			var implementation = ReferenceTo(copier.Source, Implementation);
			if (implementation.IsNil)
				return;

			// The reference standing in for the whole framework becomes the core
			// reference assembly, and types the core does not export get a row of
			// their own. Renaming is only safe once every type has somewhere to
			// go, so an image naming a type the index does not know keeps the
			// implementation reference and gains rows beside it.
			var complete = EveryTypeIsExported(copier.Source, implementation, index);
			if (complete)
				copier.ResolveAssemblyName = name => name == Implementation ? core : null;

			var added = ReferencesOf(copier.Source);
			copier.ResolveScope = (scope, @namespace, name) =>
			{
				if (scope != (EntityHandle) implementation)
					return default;

				var exporter = index.ExporterOf(ReferenceAssemblyIndex.FullName(@namespace, name));
				if (exporter == null)
					return default;

				if (complete && exporter.Name == CoreReference)
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
	/// Whether the reference pack accounts for every type the image names
	/// through the implementation assembly.
	/// </summary>
	private static bool EveryTypeIsExported(
		MetadataReader metadata,
		AssemblyReferenceHandle implementation,
		ReferenceAssemblyIndex index)
	{
		foreach (var handle in metadata.TypeReferences)
		{
			var type = metadata.GetTypeReference(handle);
			if (type.ResolutionScope != (EntityHandle) implementation)
				continue;

			var name = ReferenceAssemblyIndex.FullName(
				metadata.GetString(type.Namespace), metadata.GetString(type.Name));
			if (index.ExporterOf(name) == null)
				return false;
		}

		return true;
	}

	private static AssemblyReferenceHandle ReferenceTo(MetadataReader metadata, string name)
	{
		foreach (var handle in metadata.AssemblyReferences)
			if (metadata.GetString(metadata.GetAssemblyReference(handle).Name) == name)
				return handle;
		return default;
	}

	/// <summary>
	/// The references the image already has, so an exporter it names is reused
	/// rather than written a second time.
	/// </summary>
	private static Dictionary<string, EntityHandle> ReferencesOf(MetadataReader metadata)
	{
		var references = new Dictionary<string, EntityHandle>(StringComparer.Ordinal);
		foreach (var handle in metadata.AssemblyReferences)
			references[metadata.GetString(metadata.GetAssemblyReference(handle).Name)] = handle;
		return references;
	}
}
