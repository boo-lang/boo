#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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

//
// DO NOT EDIT THIS FILE!
//
// This file was generated automatically by astgen.boo.
//

namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]
	public partial class SliceCollection : NodeCollection<Boo.Lang.Compiler.Ast.Slice>
	{
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public static SliceCollection FromArray(params Boo.Lang.Compiler.Ast.Slice[] items)
		{
			var collection = new SliceCollection();
			collection.AddRange(items);
			return collection;
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public Boo.Lang.Compiler.Ast.SliceCollection PopRange(int begin)
		{
			var range = new Boo.Lang.Compiler.Ast.SliceCollection(ParentNode);
			range.InnerList.AddRange(InternalPopRange(begin));
			return range;
		}
	}
}

