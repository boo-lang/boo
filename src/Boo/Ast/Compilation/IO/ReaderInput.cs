using System;

namespace Boo.Ast.Compilation.IO
{
	/// <summary>
	/// TextReader based compiler input.
	/// </summary>
	public class ReaderInput : ICompilerInput
	{
		string _name;

		System.IO.TextReader _reader;

		public ReaderInput(string name, System.IO.TextReader reader)
		{
			if (null == name)
			{
				throw new ArgumentNullException("name");
			}

			if (null == reader)
			{
				throw new ArgumentNullException("reader");
			}

			_name = name;
			_reader = reader;
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public System.IO.TextReader Open()
		{
			return _reader;
		}
	}
}
