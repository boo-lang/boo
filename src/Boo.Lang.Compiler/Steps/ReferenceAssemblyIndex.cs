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
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// Which reference assembly exports each framework type.
/// </summary>
/// <remarks>
/// One implementation assembly stands for many reference assemblies:
/// System.Object is exported by System.Runtime and Dictionary`2 by
/// System.Collections, though both live in System.Private.CoreLib. A reference
/// to the implementation therefore cannot be renamed, only resolved type by type.
/// </remarks>
internal sealed class ReferenceAssemblyIndex
{
	private readonly Dictionary<string, AssemblyName> _exporters;

	private ReferenceAssemblyIndex(Dictionary<string, AssemblyName> exporters)
	{
		_exporters = exporters;
	}

	/// <summary>
	/// The reference pack for the running runtime, or null when none is
	/// installed, in which case emission stays as it was.
	/// </summary>
	internal static ReferenceAssemblyIndex ForRunningRuntime()
	{
		return For(typeof(object).Assembly.Location);
	}

	/// <summary>
	/// The index for the runtime whose core library sits at a given path.
	/// </summary>
	internal static ReferenceAssemblyIndex For(string coreLibrary)
	{
		var directory = ReferenceDirectory(coreLibrary);
		if (directory == null)
			return null;

		// Sorted so that a name two assemblies both define resolves the same
		// way on every machine, whatever order the directory comes back in.
		var paths = Directory.GetFiles(directory, "*.dll");
		Array.Sort(paths, StringComparer.Ordinal);

		var exporters = new Dictionary<string, AssemblyName>(StringComparer.Ordinal);
		foreach (var path in paths)
			Read(path, exporters);

		return exporters.Count == 0 ? null : new ReferenceAssemblyIndex(exporters);
	}

	/// <summary>
	/// The assembly defining a type, named as the reference assembly names
	/// itself, or null when the type is not framework.
	/// </summary>
	internal AssemblyName ExporterOf(string fullName)
	{
		AssemblyName assembly;
		return _exporters.TryGetValue(fullName, out assembly) ? assembly : null;
	}

	/// <summary>
	/// The identity a reference assembly gives itself, or null when the pack
	/// does not have it.
	/// </summary>
	internal AssemblyName NameOf(string assembly)
	{
		foreach (var name in _exporters.Values)
			if (name.Name == assembly)
				return name;
		return null;
	}

	private static void Read(string path, Dictionary<string, AssemblyName> exporters)
	{
		try
		{
			// Version and public key vary across the pack, and some of it is
			// unsigned, so the identity is read rather than assumed.
			var name = AssemblyName.GetAssemblyName(path);

			using var stream = File.OpenRead(path);
			using var pe = new PEReader(stream);
			if (!pe.HasMetadata)
				return;

			var metadata = pe.GetMetadataReader();

			// Only definitions. A type is forwarded by several assemblies at once,
			// mscorlib among them, and naming a forwarder rather than the
			// assembly that defines the type is what breaks resolution.
			foreach (var handle in metadata.TypeDefinitions)
			{
				var type = metadata.GetTypeDefinition(handle);
				if (type.IsNested || !IsPublic(type.Attributes))
					continue;
				exporters.TryAdd(FullName(metadata.GetString(type.Namespace), metadata.GetString(type.Name)), name);
			}
		}
		catch (BadImageFormatException)
		{
		}
		catch (IOException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
	}

	private static bool IsPublic(TypeAttributes attributes)
	{
		return (attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public;
	}

	internal static string FullName(string @namespace, string name)
	{
		return string.IsNullOrEmpty(@namespace) ? name : @namespace + "." + name;
	}

	/// <summary>
	/// The newest reference pack of the running runtime's version, beside the
	/// shared framework the process runs on.
	/// </summary>
	/// <remarks>
	/// The pack is versioned separately from the shared framework, so a runtime
	/// patched past the SDK that installed the pack has no exact match. The
	/// major and minor have to agree; the patch does not.
	/// </remarks>
	internal static string ReferenceDirectory(string coreLibrary)
	{
		if (string.IsNullOrEmpty(coreLibrary))
			return null;

		// .../shared/Microsoft.NETCore.App/<version>/System.Private.CoreLib.dll
		var versionDirectory = Path.GetDirectoryName(coreLibrary);
		if (versionDirectory == null)
			return null;

		var runtime = VersionOf(Path.GetFileName(versionDirectory));
		var root = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(versionDirectory)));
		if (runtime == null || root == null)
			return null;

		var packs = Path.Combine(root, "packs", "Microsoft.NETCore.App.Ref");
		if (!Directory.Exists(packs))
			return null;

		var pack = NewestPack(packs, runtime);
		if (pack == null)
			return null;

		// A pack holds one target framework directory. Anything else is a shape
		// this does not know how to read.
		var reference = Path.Combine(pack, "ref");
		var frameworks = Directory.Exists(reference)
			? Directory.GetDirectories(reference)
			: Array.Empty<string>();
		return frameworks.Length == 1 ? frameworks[0] : null;
	}

	/// <summary>
	/// The highest pack sharing the runtime's major and minor version and no
	/// newer than it.
	/// </summary>
	private static string NewestPack(string packs, Version runtime)
	{
		string best = null;
		Version bestVersion = null;

		foreach (var candidate in Directory.GetDirectories(packs))
		{
			var version = VersionOf(Path.GetFileName(candidate));
			if (version == null || version > runtime)
				continue;
			if (version.Major != runtime.Major || version.Minor != runtime.Minor)
				continue;
			if (bestVersion != null && version <= bestVersion)
				continue;

			best = candidate;
			bestVersion = version;
		}

		return best;
	}

	private static Version VersionOf(string name)
	{
		var release = name.IndexOf('-'); // 10.0.0-preview.1
		if (release >= 0)
			name = name.Substring(0, release);
		return Version.TryParse(name, out var version) ? version : null;
	}
}
