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

namespace Boo.Lang.Compiler.Ast
{
	public class LexicalInfo
	{
		public static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1, -1);

		protected int _line;

		protected int _startColumn;
		
		protected int _endColumn;

		protected string _filename;
		
		public LexicalInfo(string filename, int line, int startColumn, int endColumn)
		{
			if (endColumn < startColumn)
			{
				throw new ArgumentException("endColum must be >= startColumn", "endColumn");
			}
			_filename = filename;
			_line = line;
			_startColumn = startColumn;
			_endColumn = endColumn;
		}

		public LexicalInfo(string filename) : this(filename, 0, 0, 0)
		{
		}

		public string FileName
		{
			get
			{
				return _filename;
			}
		}

		public int Line
		{
			get
			{
				return _line;
			}
		}

		public int StartColumn
		{
			get
			{
				return _startColumn;
			}
		}
		
		public int EndColumn
		{
			get
			{
				return _endColumn;
			}
		}

		override public string ToString()
		{
			return string.Format("{0}({1},{2})", _filename, _line, _startColumn);
		}
	}
}
