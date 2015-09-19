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
	/// Process white space agnostic tokens to generate INDENT, DEDENT
	/// virtual tokens as expected by the standard grammar.
	/// </summary>
	public class WSATokenStreamFilterV4 : ITokenSource
	{
		static readonly char[] NewLineCharArray = new char[] { '\r', '\n' };
		
		/// <summary>
		/// token input stream.
		/// </summary>
		protected ITokenSource _source;

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


		public WSATokenStreamFilterV4(ITokenSource source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			_source = source;
			_pendingTokens = new Queue<IToken>();
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

		public ITokenSource Source
		{
			get { return _source; }
		}
		
		void ResetBuffer()
		{
			_buffer.Length = 0;
		}

		public IToken NextToken()
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

			if (token.Type == Boo.Lang.ParserV4.BooLexer.COLON) {

				Enqueue(token);

				// If whitespace is not being skiped assume it's a block
				var next = BufferUntilNextNonWhiteSpaceToken();
				if (_buffer.Length > 0) {
					// Special case for docstrings
					if (next.Type == Boo.Lang.ParserV4.BooLexer.TRIPLE_QUOTED_STRING) {
						while (next.Type != Boo.Lang.ParserV4.BooLexer.TQS_END && next.Type != TokenConstants.Eof)
						{
							ProcessNextToken(next);
							next = BufferUntilNextNonWhiteSpaceToken();
						}
						ProcessNextToken(next);
						if (next.Type != TokenConstants.Eof)
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
					if (next.Type != Boo.Lang.ParserV4.BooLexer.COLON) {
						// Not an ending keyword, just process it as normal
						Enqueue(token);
						ProcessNextToken(next);
						return;
					}
				}

				// Inject a `pass` if there are no statements in a block
				if (IsLastIndent()) {
					Enqueue(CreateToken(token, Boo.Lang.ParserV4.BooLexer.PASS, "pass"));
				}

				// Dedent the block
				EnqueueEOS(token);
				EnqueueDedent(token);

				if (token.Type != Boo.Lang.ParserV4.BooLexer.END)
					Enqueue(token);

				// Process the look-ahead token we used to disambiguate
				if (null != next)
					ProcessNextToken(next);

			}
			else if (token.Type == Boo.Lang.ParserV4.BooLexer.QQ_BEGIN)
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
			else if (token.Type == Boo.Lang.ParserV4.BooLexer.QQ_END)
			{
				if (_lastQQIndented)
					EnqueueDedent(token);
				Enqueue(token);
			}
			else if (token.Type == TokenConstants.Eof)
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
			return _lastEnqueuedToken != null && _lastEnqueuedToken.Type == Boo.Lang.ParserV4.BooLexer.INDENT;
		}

		bool IsLastDot()
		{
			return _lastEnqueuedToken != null && _lastEnqueuedToken.Type == Boo.Lang.ParserV4.BooLexer.DOT;
		}

		static bool IsAmbiguous(int type)
		{
			return type == Boo.Lang.ParserV4.BooLexer.OR || type == Boo.Lang.ParserV4.BooLexer.ELSE;
		}

		static bool IsEnding(int type)
		{
			return type == Boo.Lang.ParserV4.BooLexer.END || 
				   type == Boo.Lang.ParserV4.BooLexer.ELSE ||
				   type == Boo.Lang.ParserV4.BooLexer.ELIF ||
				   type == Boo.Lang.ParserV4.BooLexer.EXCEPT ||
				   type == Boo.Lang.ParserV4.BooLexer.ENSURE ||
				   type == Boo.Lang.ParserV4.BooLexer.THEN ||
				   type == Boo.Lang.ParserV4.BooLexer.OR;
		}

		IToken BufferUntilNextNonWhiteSpaceToken()
		{
			ResetBuffer();

			IToken token = null;
			while (true)
			{	
				token = _source.NextToken();

				if (token.Channel != TokenConstants.DefaultChannel)
				{
					Enqueue(token);
				}
				else if (token.Type == Boo.Lang.ParserV4.BooLexer.WS || token.Type == Boo.Lang.ParserV4.BooLexer.NEWLINE)
				{
					_buffer.Append(token.Text);
				}
				else break;
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
			Enqueue(CreateToken(prototype, Boo.Lang.ParserV4.BooLexer.INDENT, "<INDENT>"));
		}

		void EnqueueDedent(IToken prototype)
		{
			Enqueue(CreateToken(prototype, Boo.Lang.ParserV4.BooLexer.DEDENT, "<DEDENT>"));
		}		

		void EnqueueEOS(IToken prototype)
		{
			Enqueue(CreateToken(prototype, Boo.Lang.ParserV4.BooLexer.EOL, "<EOL>"));
		}
				
		static IToken CreateToken(IToken prototype, int newTokenType, string newTokenText)
		{
			return new BooTokenV4(Tuple.Create(prototype.TokenSource, prototype.InputStream), newTokenType, newTokenText,
				prototype.InputStream.SourceName,
				prototype.StartIndex,
				prototype.StartIndex - 1,
				prototype.Line,
				prototype.Column + SafeGetLength(prototype.Text),
				true);
		}

		static int SafeGetLength(string s)
		{
			return s == null ? 0 : s.Length;
		}
	}
}

