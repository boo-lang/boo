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
using System.Text;
using Boo.Ast;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// Representa um erro de compilao.
	/// </summary>
	[Serializable]
	public class Error : ApplicationException
	{
		LexicalInfo _ldata;

		public Error(LexicalInfo data, string message, Exception cause) : base(message, cause)
		{
			if (null == data)
			{
				throw new ArgumentNullException("data");
			}

			if (null == message)
			{
				throw new ArgumentNullException("message");
			}

			_ldata = data;
		}

		public Error(Node node, string message, Exception cause) : this(node.LexicalInfo, message, cause)
		{
		}

		public Error(Node node, string message) : this(node, message, null)
		{
		}

		public Error(LexicalInfo data, string message) : this(data, message, null)
		{
		}

		public Error(LexicalInfo data, Exception cause) : this(data, cause.Message, cause)
		{
		}

		public LexicalInfo LexicalInfo
		{
			get
			{
				return _ldata;
			}
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool verbose)
		{
			StringBuilder sb = new StringBuilder();
			if (_ldata.Line > 0)
			{
				sb.Append(_ldata);
				sb.Append(": ");
			}
			if (verbose)
			{
				sb.Append(base.ToString());
			}
			else
			{
				sb.Append(Message);			
			}
			return sb.ToString();
		}
	}
}
