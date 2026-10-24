#region license
// Copyright (c) 2066, the Boo contributors
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
using System.Reflection.Metadata.Ecma335;
using System.Resources;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// The managed resources of the assembly being compiled.
/// </summary>
/// <remarks>
/// PersistedAssemblyBuilder cannot define resources, so ICompilerResource
/// implementations write into this collection while EmitAssembly runs, and
/// AssemblyImage puts them into the image once it is built. A resource made
/// with DefineResource is a .resources stream, as it was with the old
/// backend; its writers are never asked to close, so they are finished here.
/// </remarks>
internal sealed class EmbeddedResources
{
	private readonly List<KeyValuePair<string, Func<byte[]>>> _resources = [];

	internal static EmbeddedResources Of(CompilerContext context)
	{
		var result = ContextAnnotations.GetEmbeddedResources(context);
		if (result == null)
		{
			result = new EmbeddedResources();
			ContextAnnotations.SetEmbeddedResources(context, result);
		}
		return result;
	}

	internal void AddFile(string name, string path)
	{
		var bytes = File.ReadAllBytes(path);
		_resources.Add(new KeyValuePair<string, Func<byte[]>>(name, () => bytes));
	}

	internal IResourceWriter DefineWriter(string name)
	{
		var stream = new MemoryStream();
		var writer = new ResourceWriter(stream);
		_resources.Add(new KeyValuePair<string, Func<byte[]>>(name, () =>
		{
			writer.Close();
			return stream.ToArray();
		}));
		return writer;
	}

	/// <summary>
	/// Adds the resources to the metadata, and returns the blob that holds
	/// their contents; null when there are none.
	/// </summary>
	internal static BlobBuilder WriteInto(CompilerContext context, MetadataBuilder metadata)
	{
		var resources = ContextAnnotations.GetEmbeddedResources(context);
		if (resources == null || resources._resources.Count == 0)
			return null;

		var blob = new BlobBuilder();
		foreach (var pair in resources._resources)
		{
			var bytes = pair.Value();
			var offset = blob.Count;
			blob.WriteInt32(bytes.Length);
			blob.WriteBytes(bytes);
			metadata.AddManifestResource(
				ManifestResourceAttributes.Public,
				metadata.GetOrAddString(pair.Key),
				default(EntityHandle),
				(uint)offset);
		}
		return blob;
	}
}
