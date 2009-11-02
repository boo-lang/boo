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

		public Block(params Statement[] statements)
		{
			Statements.Extend(statements);
		}
		
		public void Clear()
		{
			if (null != _statements) _statements = null;
		}
		
		public bool IsEmpty
		{
			get { return (null == _statements || Statements.IsEmpty); }
		}

		[Obsolete("HasStatements is Obsolete, use IsEmpty instead")]
		public bool HasStatements
		{
			get { return !IsEmpty; }
		}

		public bool StartsWith<T>() where T : Statement
		{
			return !IsEmpty && (Statements.StartsWith<T>()
			       || (FirstStatement is Block && ((Block) FirstStatement).StartsWith<T>()));
		}

		public bool EndsWith<T>() where T : Statement
		{
			return !IsEmpty && (Statements.EndsWith<T>()
			       || (LastStatement is Block && ((Block) LastStatement).EndsWith<T>()));
		}

		public Statement FirstStatement
		{
			get { return IsEmpty ? null : Statements.First; }
		}

		public Statement LastStatement
		{
			get { return IsEmpty ? null : Statements.Last; }
		}


		public override Block ToBlock()
		{
			return this;
		}
		
		public void Add(Statement stmt)
		{
			Block block = stmt as Block;
			if (null != block)
				Add(block);
			else
				this.Statements.Add(stmt);
		}
		
		public void Add(Block block)
		{
			if (block.HasAnnotations)
			{
				this.Statements.Add(block);
				return;
			}
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

		public Statement Simplify()
		{
			if (IsEmpty)
				return this;
			if (Statements.Count > 1 || HasAnnotations)
				return this;
			return Statements[0];
		}
	}
}
