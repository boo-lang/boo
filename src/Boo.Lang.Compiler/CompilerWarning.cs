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
	public class CompilerWarning
	{
		private readonly string _code;
		
		private readonly string _message;
		
		private readonly LexicalInfo _lexicalInfo;
		
		public CompilerWarning(LexicalInfo lexicalInfo, string message)
			: this("BCW0000", lexicalInfo, message)
		{
		}

		public CompilerWarning(LexicalInfo lexicalInfo, string message, string code)
		{
			if (null == message) throw new ArgumentNullException("message");
			if (null == code) throw new ArgumentNullException("code");
			_lexicalInfo = lexicalInfo;
			_message = message;
			_code = code;
		}
		
		public CompilerWarning(string message) : this(LexicalInfo.Empty, message)
		{
		}		
		public CompilerWarning(string code, LexicalInfo lexicalInfo,  params object[] args)
			: this(lexicalInfo, Boo.Lang.ResourceManager.Format(code, args), code)
		{
		}
		
		public string Message
		{
			get { return _message; }
		}
		
		public LexicalInfo LexicalInfo
		{
			get { return _lexicalInfo; }
		}
		
		public string Code
		{
			get { return _code; }
		}
		
		override public string ToString()
		{
			StringBuilder writer = new StringBuilder();
			if (_lexicalInfo.Line > 0)
			{
				writer.Append(_lexicalInfo);
				writer.Append(": ");
			}
			writer.Append(_code);
			writer.Append(": ");
			writer.Append(_message);
			return writer.ToString();
		}
	}
}
