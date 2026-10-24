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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// Gives the symbols a row for every method of the image.
/// </summary>
/// <remarks>
/// A portable PDB says which method a MethodDebugInformation row belongs to by
/// its position: row n is the symbols of the method in row n of the MethodDef
/// table. PersistedAssemblyBuilder only writes a row for a method that has a
/// body, so each interface, abstract or extern method before a method shifts
/// that method's symbols onto the one before it. The debugger then finds the
/// sequence points of other methods, in other files, in a method it is stepping
/// through.
///
/// The symbols are read back and written again with an empty row for each
/// method that has no body. Nothing else changes.
/// </remarks>
internal static class PdbAlignment
{
	/// <summary>
	/// The symbols for an image whose methods without bodies are the ones given.
	/// </summary>
	/// <remarks>
	/// Both builders can be written out only once, so what is wanted of the image
	/// is read from it as it is copied, and this takes the row counts of the
	/// builder that is going to be written.
	/// </remarks>
	internal static MetadataBuilder Aligned(MethodTracker tracker, MetadataBuilder metadata, MetadataBuilder pdb)
	{
		if (tracker == null || tracker.BodylessMethodCount == 0)
			return pdb;

		var image = new BlobBuilder();
		new PortablePdbBuilder(pdb, metadata.GetRowCounts(), default).Serialize(image);
		using var provider = MetadataReaderProvider.FromPortablePdbImage(image.ToImmutableArray());
		var symbols = provider.GetMetadataReader();

		// Every method with a body should have a row already. If not, the shift
		// is something else, and the symbols are better left as they are.
		if (symbols.MethodDebugInformation.Count + tracker.BodylessMethodCount != tracker.TotalMethods)
			return pdb;

		var aligned = new MetadataBuilder();
		CopyDocuments(symbols, aligned);
		CopyMethodDebugInformation(symbols, aligned, tracker);
		CopyLocals(symbols, aligned);
		return aligned;
	}

	/// <summary>
	/// The rows of the methods an image gives no body to.
	/// </summary>
	internal sealed class MethodTracker
	{
		private readonly HashSet<int> _rows = [];

		internal int TotalMethods;
		internal int BodylessMethodCount => _rows.Count;

		internal bool IsBodyless(int row) => _rows.Contains(row);

		/// <summary>
		/// Reads them from metadata whose method bodies are at addresses above zero, as the copier writes them.
		/// </summary>
		internal static MethodTracker In(MetadataReader reader)
		{
			var result = new MethodTracker();
			foreach (var handle in reader.MethodDefinitions)
			{
				++result.TotalMethods;
				if (reader.GetMethodDefinition(handle).RelativeVirtualAddress == 0)
				{
					result._rows.Add(MetadataTokens.GetRowNumber(handle));
				}
			}
			return result;
		}
	}

	private static void CopyDocuments(MetadataReader source, MetadataBuilder target)
	{
		foreach (var handle in source.Documents)
		{
			var document = source.GetDocument(handle);
			target.AddDocument(
				target.GetOrAddDocumentName(source.GetString(document.Name)),
				target.GetOrAddGuid(source.GetGuid(document.HashAlgorithm)),
				document.Hash.IsNil ? default : target.GetOrAddBlob(source.GetBlobBytes(document.Hash)),
				target.GetOrAddGuid(source.GetGuid(document.Language)));
		}
	}

	private static void CopyMethodDebugInformation(MetadataReader source, MetadataBuilder target, MethodTracker tracker)
	{
		var rows = source.MethodDebugInformation.GetEnumerator();
		for (var row = 1; row <= tracker.TotalMethods; ++row)
		{
			if (tracker.IsBodyless(row))
			{
				target.AddMethodDebugInformation(default, default);
				continue;
			}

			rows.MoveNext();
			var information = source.GetMethodDebugInformation(rows.Current);
			target.AddMethodDebugInformation(
				information.Document,
				information.SequencePointsBlob.IsNil
					? default
					: target.GetOrAddBlob(source.GetBlobBytes(information.SequencePointsBlob)));
		}
	}

	private static void CopyLocals(MetadataReader source, MetadataBuilder target)
	{
		// Scopes name their variables by the first of a run, and the runs follow one another.
		var nextVariable = 1;
		foreach (var handle in source.LocalScopes)
		{
			var scope = source.GetLocalScope(handle);
			target.AddLocalScope(
				scope.Method,
				scope.ImportScope,
				MetadataTokens.LocalVariableHandle(nextVariable),
				MetadataTokens.LocalConstantHandle(1),
				scope.StartOffset,
				scope.Length);
			nextVariable += scope.GetLocalVariables().Count;
		}

		foreach (var handle in source.LocalVariables)
		{
			var variable = source.GetLocalVariable(handle);
			target.AddLocalVariable(variable.Attributes, variable.Index, target.GetOrAddString(source.GetString(variable.Name)));
		}
	}
}
