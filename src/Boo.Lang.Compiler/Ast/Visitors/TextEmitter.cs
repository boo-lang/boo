#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
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
			_writer.WriteLine();
			_needsIndenting = true;
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
	}
}
