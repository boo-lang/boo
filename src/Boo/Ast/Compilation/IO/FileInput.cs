using System;

namespace Boo.Ast.Compilation.IO
{
	/// <summary>
	/// File based compiler input.
	/// </summary>
	public class FileInput : Boo.Ast.Compilation.ICompilerInput
	{
		string _fname;

		public FileInput(string fname)
		{
			if (null == fname)
			{
				throw new ArgumentNullException("fname");
			}
			_fname = fname;
		}

		public string Name
		{
			get
			{
				return _fname;
			}
		}

		public System.IO.TextReader Open()
		{
			return System.IO.File.OpenText(_fname);
		}
	}
}
