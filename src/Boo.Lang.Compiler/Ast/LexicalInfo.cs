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
	public class SourceLocation : IComparable<SourceLocation>, IEquatable<SourceLocation>
	{
		protected int _line;

		protected int _column;
		
		public SourceLocation(int line, int column)
		{
			_line = line;
			_column = column;
		}
		
		public int Line
		{
			get
			{
				return _line;
			}
		}

		public int Column
		{
			get
			{
				return _column;
			}
		}
		
		public virtual bool IsValid
		{
			get
			{
				return (_line > 0) && (_column > 0);
			}
		}
		
		override public string ToString()
		{
			return string.Format("({0},{1})", _line, _column);
		}
		
		public int CompareTo(SourceLocation other)
		{
			int comp = _line.CompareTo(other._line);
			if(comp != 0) 
			{
				return comp;
			}
			comp = _column.CompareTo(other._column);
			if(comp != 0)
			{
				return comp;
			}
			return 0;
		}
		
		public bool Equals(SourceLocation other)
		{
			return CompareTo(other) == 0;
		}
	}
	
	public class LexicalInfo : SourceLocation, IEquatable<LexicalInfo>, IComparable<LexicalInfo>
	{
		public static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1);
		
		protected string _filename;
		
		private string _fullPath;
		
		public LexicalInfo(string filename, int line, int column)
			: base(line, column)
		{
			_filename = filename;			
		}

		public LexicalInfo(string filename) : this(filename, -1, -1)
		{
		}
		
		override public bool IsValid
		{
			get
			{
				return null != _filename && base.IsValid;					
			}
		}

		public string FileName
		{
			get
			{
				return _filename;
			}
		}
		
		public string FullPath
		{
			get
			{
				if (null != _fullPath) return _fullPath;
				_fullPath = SafeGetFullPath(_filename);
				return _fullPath;
			}
		}

		override public string ToString()
		{
			return string.Format("{0}({1},{2})", _filename, _line, _column);
		}
		
		private static string SafeGetFullPath(string fname)
		{
			try
			{
				return System.IO.Path.GetFullPath(fname);
			}
			catch (Exception)
			{
			}
			return fname;
		}
		
		public int CompareTo(LexicalInfo other)
		{
			int comp = base.CompareTo(other);
			if(comp != 0) 
			{
				return comp;
			}
			comp = string.Compare(_filename, other._filename);
			if(comp != 0)
			{
				return comp;
			}
			return 0;
		}
		
		public bool Equals(LexicalInfo other)
		{
			return CompareTo(other) == 0;
		}
	}
}
