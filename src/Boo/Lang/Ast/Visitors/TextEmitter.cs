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

namespace Boo.Lang.Ast.Visitors
{
	/// <summary>	
	/// </summary>
	public class TextEmitter : Boo.Lang.Ast.DepthFirstSwitcher
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
