#region license
// Copyright (c) 2010 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.IO;
using System.Text;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Parser
{
	internal class CodeFactory
	{
		public static TypeReference EnumerableTypeReferenceFor(TypeReference tr)
		{
			var result = new GenericTypeReference(tr.LexicalInfo, "System.Collections.Generic.IEnumerable");
			result.GenericArguments.Add(tr);
			return result;
		}
		
		public static Module NewQuasiquoteModule(LexicalInfo li)
		{
			return new Module(li) { Name = ModuleNameFrom(li.FileName) + "$" + li.Line };
		}
		
		public static string ModuleNameFrom(string readerName)
		{
			if (readerName.IndexOfAny(Path.GetInvalidPathChars()) > -1)
				return EncodeModuleName(readerName);
			return Path.GetFileNameWithoutExtension(Path.GetFileName(readerName));
		}
	
		static string EncodeModuleName(string name)
		{
			var buffer = new StringBuilder(name.Length);
			foreach (var ch in name)
			{
				if (Char.IsLetterOrDigit(ch))
					buffer.Append(ch);
				else
					buffer.Append("_");
			}
			return buffer.ToString();
		}
	}
}
