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
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// A compilation error.
	/// </summary>
	[Serializable]
	public class CompilerError : ApplicationException
	{
		LexicalInfo _lexicalInfo;
		
		string _code;
		
		public CompilerError(string code, LexicalInfo lexicalInfo, Exception cause, params object[] args) : base(ResourceManager.Format(code, args), cause)
		{
			if (null == lexicalInfo)
			{
				throw new ArgumentNullException("lexicalInfo");
			}
			_code = code;
			_lexicalInfo = lexicalInfo;
		}
		
		public CompilerError(string code, Exception cause, params object[] args) : this(code, LexicalInfo.Empty, cause, args)
		{
		}
		
		public CompilerError(string code, LexicalInfo lexicalInfo, params object[] args) : base(ResourceManager.Format(code, args))
		{
			if (null == lexicalInfo)
			{
				throw new ArgumentNullException("lexicalInfo");
			}
			_code = code;
			_lexicalInfo = lexicalInfo;
		}
		
		public CompilerError(string code, LexicalInfo lexicalInfo) : base(ResourceManager.GetString(code))
		{
			if (null == lexicalInfo)
			{
				throw new ArgumentNullException("lexicalInfo");
			}
			_code = code;
			_lexicalInfo = lexicalInfo;
		}
		
		public CompilerError(string code, LexicalInfo lexicalInfo, string message, Exception cause) : base(message, cause)
		{
			if (null == lexicalInfo)
			{
				throw new ArgumentNullException("lexicalInfo");
			}
			_code = code;
			_lexicalInfo = lexicalInfo;
		}
		
		public CompilerError(LexicalInfo lexicalInfo, string message, Exception cause) : this("BCE0040", lexicalInfo, message, cause)
		{
		}
		
		public CompilerError(Node node, string message, Exception cause) : this(node.LexicalInfo, message, cause)
		{
		}

		public CompilerError(Node node, string message) : this(node, message, null)
		{
		}

		public CompilerError(LexicalInfo data, string message) : this(data, message, null)
		{
		}

		public CompilerError(LexicalInfo data, Exception cause) : this(data, cause.Message, cause)
		{
		}
		
		/// <summary>
		/// Error code.
		/// </summary>
		public string Code
		{
			get
			{
				return _code;
			}
		}

		public LexicalInfo LexicalInfo
		{
			get
			{
				return _lexicalInfo;
			}
		}

		override public string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool verbose)
		{
			StringBuilder sb = new StringBuilder();
			if (_lexicalInfo.Line > 0)
			{
				sb.Append(_lexicalInfo);
				sb.Append(": ");
			}
			sb.Append(_code);
			sb.Append(": ");
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
