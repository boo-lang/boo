using System;
using System.IO;
using System.CodeDom.Compiler;

namespace Boo.Ast.Visiting
{
	/// <summary>
	/// Classe base para visitors que emitem texto como sada.
	/// </summary>
	public class TextEmitter : Boo.Ast.DepthFirstSwitcher
	{
		protected IndentedTextWriter _writer;

		public TextEmitter(TextWriter writer)
		{
			if (null == writer)
			{
				throw new ArgumentNullException("writer");
			}

			_writer = new IndentedTextWriter(writer, "  ");
		}

		public void Indent()
		{
			_writer.Indent += 1;
		}

		public void Dedent()
		{
			_writer.Indent -= 1;
		}

		public void WriteIndented()
		{
			_writer.Write("");
		}

		public void WriteIndented(string format, params object[] args)
		{
			_writer.Write(format, args);
		}

		public void Write(string s)
		{
			_writer.InnerWriter.Write(s);
		}

		public void Write(string format, params object[] args)
		{
			_writer.InnerWriter.Write(format, args);
		}

		public void WriteLine()
		{
			_writer.WriteLine();
		}

		public void WriteLine(string s)
		{
			_writer.WriteLine(s);
		}

		public void WriteLine(string format, params object[] args)
		{
			_writer.WriteLine(format, args);
		}
	}
}
