#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.IO;
using System.CodeDom.Compiler;

namespace Boo.Ast.Visiting
{
	/// <summary>	
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
			Write(string.Format(format, args));
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
