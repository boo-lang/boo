#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
