#region license
// Copyright (c) the Boo contributors
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

using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// Assembly-level attributes whose constructor lives in the assembly being
/// built, written straight to the metadata rather than through the builder.
/// </summary>
/// <remarks>
/// PersistedAssemblyBuilder writes the assembly's custom attribute rows
/// before it hands out metadata handles for the module's own types, so a
/// constructor defined here ends up in the table as a nil MethodDef and the
/// attribute cannot be read back at runtime. These rows are added after
/// GenerateMetadata instead, once the token exists.
/// </remarks>
internal static class DeferredAssemblyAttributes
{
	private static readonly PropertyInfo ConstructorOf =
		typeof(CustomAttributeBuilder).GetProperty("Ctor", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly PropertyInfo BlobOf =
		typeof(CustomAttributeBuilder).GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>
	/// True when the attribute has to bypass the builder. Reading the
	/// constructor needs CustomAttributeBuilder's internals, so a runtime
	/// that no longer exposes them defers nothing and behaves as before.
	/// </summary>
	internal static bool Defers(CustomAttributeBuilder attribute)
	{
		return ConstructorOf != null
			&& BlobOf != null
			&& ConstructorOf.GetValue(attribute) is ConstructorBuilder;
	}

	internal static void Write(CompilerContext context, MetadataBuilder metadata)
	{
		var attributes = ContextAnnotations.GetDeferredAssemblyAttributes(context);
		if (attributes == null)
			return;

		var assembly = MetadataTokens.EntityHandle(TableIndex.Assembly, 1);
		foreach (var attribute in attributes)
		{
			var constructor = (ConstructorBuilder) ConstructorOf.GetValue(attribute);
			metadata.AddCustomAttribute(
				assembly,
				MetadataTokens.MethodDefinitionHandle(constructor.MetadataToken),
				metadata.GetOrAddBlob((byte[]) BlobOf.GetValue(attribute)));
		}
	}
}
