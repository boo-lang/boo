#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Compiler.Ast
{
	public partial class Block
	{	
		public Block()
		{
 		}	
		
		public Block(LexicalInfo lexicalInfo) : base(lexicalInfo)
		{
		}
		
		public void Clear()
		{
			if (null != _statements) _statements = null;
		}
		
		public bool IsEmpty
		{
			get { return !HasStatements; }
		}
		
		public bool HasStatements
		{
			get
			{
				if (_statements == null) return false;
				return _statements.Count > 0;
			}
		}

		public override Block ToBlock()
		{
			return this;
		}
		
		public void Add(Statement stmt)
		{
			this.Statements.Add(stmt);
		}
		
		public void Add(Block block)
		{
			this.Statements.Extend(block.Statements);
		}
		
		public void Add(Expression expression)
		{
			this.Statements.Add(new ExpressionStatement(expression));
		}
		
		public void Insert(int index, Expression expression)
		{
			this.Statements.Insert(index, new ExpressionStatement(expression));
		}
		
		public void Insert(int index, Statement stmt)
		{
			this.Statements.Insert(index, stmt);
		}
	}
}
