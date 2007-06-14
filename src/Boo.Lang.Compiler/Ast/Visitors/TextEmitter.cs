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
using System.Collections.Generic;
using System.IO;

namespace Boo.Lang.Compiler.Ast.Visitors
{
	/// <summary>
	/// </summary>
	public class TextEmitter : Boo.Lang.Compiler.Ast.DepthFirstVisitor
	{
		protected TextWriter _writer;
		
		protected int _indent = 0;
		
		protected string _indentText = "\t";
		
		protected bool _needsIndenting = true;
		
		protected int _disableNewLine = 0;

		public TextEmitter(TextWriter writer)
		{
			if (null == writer)
			{
				throw new ArgumentNullException("writer");
			}

			_writer = writer;
		}
		
		public string IndentText
		{
			get
			{
				return _indentText;
			}
			
			set
			{
				_indentText = value;
			}
		}
		
		public TextWriter Writer
		{
			get
			{
				return _writer;
			}
		}

		public void Indent()
		{
			_indent += 1;
		}

		public void Dedent()
		{
			_indent -= 1;
		}

		public virtual void WriteIndented()
		{
			if (_needsIndenting)
			{
				for (int i=0; i<_indent; ++i)
				{
					_writer.Write(_indentText);
				}
				_needsIndenting = false;
			}
		}
		
		public virtual void WriteLine()
		{
			if (0 == _disableNewLine)
			{
				_writer.WriteLine();
				_needsIndenting = true;
			}
		}
		
		protected void DisableNewLine()
		{
			++_disableNewLine;
		}
		
		protected void EnableNewLine()
		{
			if (0 == _disableNewLine)
			{
				throw new InvalidOperationException();
			}
			--_disableNewLine;
		}
		
		public virtual void Write(string s)
		{
			_writer.Write(s);
		}

		public void WriteIndented(string format, params object[] args)
		{
			WriteIndented();
			Write(format, args);
		}

		public void Write(string format, params object[] args)
		{
			Write(string.Format(format, args));
		}

		public void WriteLine(string s)
		{
			WriteIndented(s);
			WriteLine();
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteIndented(format, args);
			WriteLine();
		}

		protected void WriteCommaSeparatedList<T>(IEnumerable<T> items) where T : Node
		{
			int i = 0;
			foreach (T node in items)
			{
				if (i++ > 0) Write(", ");
				Visit(node);
			}
		}

		protected void WriteArray<T>(NodeCollection<T> items)  where T : Node
		{
			WriteArray(items, null);
		}

		protected void WriteArray<T>(NodeCollection<T> items, ArrayTypeReference type) where T : Node
		{
			Write("(");
			if (null != type)
			{
				Write("of ");
				type.ElementType.Accept(this);
				Write(": ");
			}
			if (items.Count > 1)
			{
				for (int i=0; i<items.Count; ++i)
				{
					if (i>0)
					{
						Write(", ");
					}
					Visit(items[i]);
				}
			}
			else
			{
				if (items.Count > 0)
				{
					Visit(items[0]);
				}
				//don't write trailing comma for "of" arrays with 1 item
				if (items.Count == 0 || null == type)
				{
					Write(",");
				}
			}
			Write(")");
		}
	}
}
