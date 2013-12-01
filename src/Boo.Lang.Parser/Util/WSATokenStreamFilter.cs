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
using antlr;

namespace Boo.Lang.Parser.Util
{
	/// <summary>
	/// Process white space agnostic tokens to generate INDENT, DEDENT
	/// virtual tokens as expected by the standard grammar.
	/// </summary>
	public class WSATokenStreamFilter : TokenStream
	{
		static readonly char[] NewLineCharArray = new char[] { '\r', '\n' };
		
		/// <summary>
		/// token input stream.
		/// </summary>
		protected TokenStream _istream;

		/// <summary>
		/// last non whitespace token for accurate location information
		/// </summary>
		protected IToken _lastEnqueuedToken;

		/// <summary>
		/// Flags the current QQ expression to need a DEDENT when closing it.
		/// </summary>
		protected bool _lastQQIndented;

		/// <summary>
		/// tokens waiting to be consumed
		/// </summary>
		protected Queue<IToken> _pendingTokens;

	
		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();


		public WSATokenStreamFilter(TokenStream istream)
		{
			if (null == istream)
			{
				throw new ArgumentNullException("istream");
			}

			_istream = istream;
			_pendingTokens = new Queue<IToken>();
		}

		public TokenStream InnerStream
		{
			get { return _istream; }
		}
		
		void ResetBuffer()
		{
			_buffer.Length = 0;
		}

		public IToken nextToken()
		{
			IToken token;
			if (_pendingTokens.Count == 0)
			{
				token = BufferUntilNextNonWhiteSpaceToken();
				ProcessNextToken(token);
			}
			token = _pendingTokens.Dequeue();
			return token;
		}

		bool BufferHasNewLine()
		{
			if (_buffer.Length == 0)
				return false;

			var text = _buffer.ToString();
			string[] lines = text.Split(NewLineCharArray);
			return lines.Length > 1;
		}

		void ProcessNextToken(IToken token)
		{
			// New lines are converted to EOS unless they come after 
			// indents or a dot (member reference)
			if (!IsLastIndent() && !IsLastDot() && BufferHasNewLine())
			{
				EnqueueEOS(token);
			}

			if (token.Type == BooLexer.COLON) {

				Enqueue(token);

				// If whitespace is not being skiped assume it's a block
				var next = BufferUntilNextNonWhiteSpaceToken();
				if (_buffer.Length > 0) {
					// Special case for docstrings
					if (next.Type == BooLexer.TRIPLE_QUOTED_STRING) {
						ProcessNextToken(next);
						EnqueueIndent(next);
						return;
					}
					EnqueueIndent(token);
				}

				ProcessNextToken(next);

			} else if (IsEnding(token.Type)) {

				IToken next = null;

				// Dissambiguate OR/ELSE
				if (IsAmbiguous(token.Type)) {
					next = BufferUntilNextNonWhiteSpaceToken();
					if (next.Type != BooLexer.COLON) {
						// Not an ending keyword, just process it as normal
						Enqueue(token);
						ProcessNextToken(next);
						return;
					}
				}

				// Inject a `pass` if there are no statements in a block
				if (IsLastIndent()) {
					Enqueue(CreateToken(token, BooLexer.PASS, "pass"));
				}

				// Dedent the block
				EnqueueEOS(token);
				EnqueueDedent(token);

				if (token.Type != BooLexer.END)
					Enqueue(token);

				// Process the look-ahead token we used to disambiguate
				if (null != next)
					ProcessNextToken(next);

			}
			else if (token.Type == BooLexer.QQ_BEGIN)
			{
				Enqueue(token);

				// If follows a new line we handle it as a block
				var next = BufferUntilNextNonWhiteSpaceToken();
				_lastQQIndented = BufferHasNewLine();
				if (_lastQQIndented) {
					EnqueueIndent(token);
				}

				ProcessNextToken(next);

			} 
			else if (token.Type == BooLexer.QQ_END)
			{
				if (_lastQQIndented)
					EnqueueDedent(token);
				Enqueue(token);
			}
			else if (token.Type == Token.EOF_TYPE)
			{
				// EOF also signals the end of any running statement 
				EnqueueEOS(token);
				Enqueue(token);
			}
			else
			{
				Enqueue(token);
			}
		}

		bool IsLastIndent()
		{
			return _lastEnqueuedToken != null && _lastEnqueuedToken.Type == BooLexer.INDENT;
		}

		bool IsLastDot()
		{
			return _lastEnqueuedToken != null && _lastEnqueuedToken.Type == BooLexer.DOT;
		}

		static bool IsAmbiguous(int type)
		{
			return type == BooLexer.OR || type == BooLexer.ELSE;
		}

		static bool IsEnding(int type)
		{
			return type == BooLexer.END || 
				   type == BooLexer.ELSE ||
				   type == BooLexer.ELIF ||
				   type == BooLexer.EXCEPT ||
				   type == BooLexer.ENSURE ||
				   type == BooLexer.THEN ||
				   type == BooLexer.OR;
		}

		IToken BufferUntilNextNonWhiteSpaceToken()
		{
			ResetBuffer();

			IToken token = null;
			while (true)
			{	
				token = _istream.nextToken();

				if (token.Type == Token.SKIP)
					continue;

				if (token.Type == BooLexer.WS)
				{
					_buffer.Append(token.getText());
					continue;
				}

				break;
			}
			return token;
		}

		void Enqueue(IToken token)
		{
			_pendingTokens.Enqueue(token);
			_lastEnqueuedToken = token;
		}

		void EnqueueIndent(IToken prototype)
		{
			Enqueue(CreateToken(prototype, BooLexer.INDENT, "<INDENT>"));
		}

		void EnqueueDedent(IToken prototype)
		{
			Enqueue(CreateToken(prototype, BooLexer.DEDENT, "<DEDENT>"));
		}		

		void EnqueueEOS(IToken prototype)
		{
			Enqueue(CreateToken(prototype, BooLexer.EOL, "<EOL>"));
		}
				
		static IToken CreateToken(IToken prototype, int newTokenType, string newTokenText)
		{
			return new BooToken(newTokenType, newTokenText,
			                    prototype.getFilename(),
			                    prototype.getLine(),
			                    prototype.getColumn()+SafeGetLength(prototype.getText()));
		}

		static int SafeGetLength(string s)
		{
			return s == null ? 0 : s.Length;
		}
	}
}

