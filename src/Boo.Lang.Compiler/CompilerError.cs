#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
		
		public CompilerError(string code, LexicalInfo lexicalInfo, string message, Exception cause) : base(message, cause)
		{
			if (null == lexicalInfo)
			{
				throw new ArgumentNullException("lexicalInfo");
			}
			_code = code;
			_lexicalInfo = lexicalInfo;
		}
		
		public CompilerError(LexicalInfo lexicalInfo, string message, Exception cause) : this("BCE0000", lexicalInfo, message, cause)
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

		public CompilerError(string message) : this(LexicalInfo.Empty, message, null)
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
