using System;

namespace Boo.Ast
{
	public class LexicalInfo
	{
		public static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1, -1);

		protected int _line;

		protected int _startColumn;
		
		protected int _endColumn;

		protected string _filename;

		internal LexicalInfo(antlr.Token token)
		{
			if (null == token)
			{
				throw new ArgumentNullException("token");
			}

			_line = token.getLine();
			_startColumn = token.getColumn();
			_endColumn = token.getColumn() + token.getText().Length;
			_filename = token.getFilename();
		}

		public LexicalInfo(string filename, int line, int startColumn, int endColumn)
		{
			if (endColumn < startColumn)
			{
				throw new ArgumentException("endColum must be greater than startColumn");
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

		public override string ToString()
		{
			return string.Format("{0}({1},{2})", _filename, _line, _startColumn);
		}
	}
}
