using System;

namespace Boo.Ast
{
	public class LexicalInfo
	{
		public static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1);

		protected int _line;

		protected int _column;

		protected string _filename;

		public LexicalInfo(antlr.Token token)
		{
			if (null == token)
			{
				throw new ArgumentNullException("token");
			}

			_line = token.getLine();
			_column = token.getColumn();
			_filename = token.getFilename();
		}

		public LexicalInfo(string filename, int line, int column)
		{
			_filename = filename;
			_line = line;
			_column = column;
		}

		public LexicalInfo(string filename) : this(filename, 0, 0)
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

		public int Column
		{
			get
			{
				return _column;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1},{2})", _filename, _line, _column);
		}
	}
}
