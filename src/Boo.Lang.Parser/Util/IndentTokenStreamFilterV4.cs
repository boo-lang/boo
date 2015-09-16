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
using System.Collections.Generic;
using Antlr4.Runtime;
using Boo.Lang.ParserV4;

namespace Boo.Lang.Parser.Util
{
	/// <summary>
	/// Process whitespace tokens and generate INDENT, DEDENT
	/// virtual tokens as needed.
	/// </summary>
	public class IndentTokenStreamFilterV4 : Antlr4.Runtime.ITokenSource
	{
		static readonly char[] NewLineCharArray = new char[] { '\r', '\n' };
		
		/// <summary>
		/// token input stream.
		/// </summary>
		protected ITokenSource _source;

		/// <summary>
		/// whitespace token type.
		/// </summary>
		protected int _wsTokenType;

		/// <summary>
		/// singleton indent token.
		/// </summary>
		protected int _indentTokenType;

		/// <summary>
		/// singleton dedent token.
		/// </summary>
		protected int _dedentTokenType;

		/// <summary>
		/// singleton EOS token.
		/// </summary>
		protected int _eosTokenType;

		/// <summary>
		/// singleton END token.
		/// </summary>
		protected int _endTokenType;

		/// <summary>
		/// singleton ID token.
		/// </summary>
		protected int _idTokenType;

		/// <summary>
		/// stack of indent levels.
		/// </summary>
		protected Stack<int> _indentStack = new Stack<int> ();

		/// <summary>
		/// tokens waiting to be consumed
		/// </summary>
		protected Queue<IToken> _pendingTokens = new Queue<IToken>();
		
		/// <summary>
		/// last non whitespace token for accurate location information
		/// </summary>
		protected IToken _lastNonWsToken;

		/// <summary>
		/// first detected indentation character
		/// </sumary>
		protected string _expectedIndent;

		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();

		public IndentTokenStreamFilterV4(ITokenSource source, int wsType, int indentType, int dedentType, int eosType, int endType, int idType)
		{
			if (null == source)
			{
				throw new ArgumentNullException("istream");
			}

			_source = source;
			_wsTokenType = wsType;
			_indentTokenType = indentType;
			_dedentTokenType = dedentType;
			_eosTokenType = eosType;
			_endTokenType = endType;
			_idTokenType = idType;

			_indentStack.Push(0); // current indent level is zero
		}

		public ITokenSource InnerStream
		{
			get { return _source; }
		}

		ITokenFactory ITokenSource.TokenFactory
		{
			get { return _source.TokenFactory; }
			set { _source.TokenFactory = value; }
		}

		int ITokenSource.Line
		{
			get { return _source.Line; }
		}

		int ITokenSource.Column
		{
			get { return _source.Column; }
		}

		public ICharStream InputStream
		{
			get { return _source.InputStream; }
		}

		string ITokenSource.SourceName
		{
			get { return _source.SourceName; }
		}

		protected int CurrentIndentLevel
		{
			get { return _indentStack.Peek(); }
		}

		public IToken NextToken()
		{
			if (_pendingTokens.Count == 0)
				ProcessNextTokens();
			IToken token = _pendingTokens.Dequeue();
			// In non-wsa mode `end` is just another identifier
			if (token.Type == _endTokenType) {
				((IWritableToken)token).Type = _idTokenType;
			}
			return token;
		}
		
		void ResetBuffer()
		{
			_buffer.Length = 0;
		}
		
		IToken BufferUntilNextNonWhiteSpaceToken()
		{
			IToken token = null;
			while (true)
			{	
				token = _source.NextToken();
				
				int ttype = token.Type;
				if (token.Channel != TokenConstants.DefaultChannel)
					continue;

				if (ttype == _wsTokenType)
				{			
					_buffer.Append(token.Text);
					continue;
				}

				break;
			}
			return token;
		}
		
		void FlushBuffer(IToken token)
		{
			if (0 == _buffer.Length) return;
			
			string text = _buffer.ToString();
			string[] lines = text.Split(NewLineCharArray);					

			if (lines.Length > 1)
			{
				string lastLine = lines[lines.Length-1];

				// Protect against mixed indentation issues
				if (String.Empty != lastLine) {
					if (null == _expectedIndent) {
						_expectedIndent = lastLine.Substring(0, 1);
					}

					if (lastLine.Replace(_expectedIndent, String.Empty) != String.Empty)
					{
						string literal = _expectedIndent == "\t"
						               ? "tabs"
						               : _expectedIndent == "\f"
						               ? "form feeds"  // The lexer allows them :p
						               : "spaces";

						throw new Antlr4.Runtime.RecognitionException(
							_source as Lexer,
							this.InputStream
						);
					}
				}

				if (lastLine.Length > CurrentIndentLevel)
				{
					EnqueueIndent(token);
					_indentStack.Push(lastLine.Length);
				}
				else if (lastLine.Length < CurrentIndentLevel)
				{
					EnqueueEOS(token);
					do 
					{
						EnqueueDedent();
						_indentStack.Pop();
					}
					while (lastLine.Length < CurrentIndentLevel);
				}
				else
				{
					EnqueueEOS(token);
				}
			}
		}
		
		void CheckForEOF(IToken token)
		{
			if (token.Type != TokenConstants.Eof) return;
			
			EnqueueEOS(token);	
			while (CurrentIndentLevel > 0)
			{
				EnqueueDedent();
				_indentStack.Pop();					
			}
		}
		
		void ProcessNextNonWhiteSpaceToken(IToken token)
		{
			_lastNonWsToken = token;
			Enqueue(token);
		}
		
		void ProcessNextTokens()
		{		
			ResetBuffer();
				
			IToken token = BufferUntilNextNonWhiteSpaceToken();
			FlushBuffer(token);			
			CheckForEOF(token);
			ProcessNextNonWhiteSpaceToken(token);
		}
		
		void Enqueue(IToken token)
		{
			_pendingTokens.Enqueue(token);
		}

		void EnqueueIndent(IToken prototype)
		{
			_pendingTokens.Enqueue(CreateToken(prototype, _indentTokenType, "<INDENT>"));
		}

		void EnqueueDedent()
		{
			_pendingTokens.Enqueue(CreateToken(_lastNonWsToken, _dedentTokenType, "<DEDENT>"));
		}		

		void EnqueueEOS(IToken prototype)
		{
			_pendingTokens.Enqueue(CreateToken(prototype, _eosTokenType, "<EOL>"));
		}

		IToken CreateToken(IToken prototype, int newTokenType, string newTokenText)
		{
			return new BooTokenV4(newTokenType, newTokenText,
				prototype.InputStream.SourceName,
				prototype.StartIndex,
				prototype.StartIndex - 1,
				prototype.Line,
				prototype.Column + SafeGetLength(prototype.Text));
		}
		
		int SafeGetLength(string s)
		{
			return s == null ? 0 : s.Length;
		}
	}
}
