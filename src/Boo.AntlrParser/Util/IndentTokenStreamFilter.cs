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
using System.Collections;

namespace Boo.AntlrParser.Util
{
	public class SimpleToken : antlr.Token
	{
		protected string _buffer;

		public SimpleToken(int type, string txt) : base(type, txt)
		{
		}

		override public void setText(string txt)
		{
			_buffer = txt;
		}

		override public string getText()
		{
			return _buffer;
		}
	}

	/// <summary>
	/// Process whitespace tokens and generate INDENT, DEDENT
	/// virtual tokens as needed.
	/// </summary>
	public class IndentTokenStreamFilter : antlr.TokenStream
	{
		/// <summary>
		/// token input stream.
		/// </summary>
		protected antlr.TokenStream _istream;

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
		/// stack of indent levels.
		/// </summary>
		protected Stack _indentStack;

		/// <summary>
		/// tokens waiting to be consumed
		/// </summary>
		protected Queue _pendingTokens;
		
		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();

		public IndentTokenStreamFilter(antlr.TokenStream istream, int wsTokenType, int indentTokenType, int dedentTokenType, int eosTokenType)
		{
			if (null == istream)
			{
				throw new ArgumentNullException("istream");
			}

			_istream = istream;
			_wsTokenType = wsTokenType;
			_indentTokenType = indentTokenType;
			_dedentTokenType = dedentTokenType;
			_eosTokenType = eosTokenType;
			_indentStack = new Stack();
			_pendingTokens = new Queue();

			_indentStack.Push(0); // current indent level is zero
		}

		protected int CurrentIndentLevel
		{
			get
			{
				return (int)_indentStack.Peek();
			}
		}

		public antlr.Token nextToken()
		{
			if (0 == _pendingTokens.Count)
			{
				ProcessNextTokens();
			}
			return (antlr.Token)_pendingTokens.Dequeue();
		}
		
		void ProcessNextTokens()
		{		
			_buffer.Length = 0;
				
			antlr.Token token = null;
			while (true)
			{			
				token = _istream.nextToken();
				
				int ttype = token.Type;
				if (antlr.Token.SKIP == ttype)
				{
					continue;
				}
				
				if (_wsTokenType == ttype)
				{			
					_buffer.Append(token.getText());
					continue;
				}
				
				break;
			}
			
			if (0 != _buffer.Length)
			{
				string text = _buffer.ToString();
				string[] lines = text.Split('\r', '\n');					

				if (lines.Length > 1)
				{
					string lastLine = lines[lines.Length-1];
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
							EnqueueDedent(token);
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
			
			if (antlr.Token.EOF_TYPE == token.Type)
			{					
				EnqueueEOS(token);	
				while (CurrentIndentLevel > 0)
				{
					EnqueueDedent(token);
					_indentStack.Pop();					
				}
			}
			
			Enqueue(token);
		}
		
		void Enqueue(antlr.Token token)
		{
			_pendingTokens.Enqueue(token);
		}

		void EnqueueIndent(antlr.Token originalToken)
		{
			_pendingTokens.Enqueue(CreateToken(originalToken, _indentTokenType, "<INDENT>"));
		}

		void EnqueueDedent(antlr.Token originalToken)
		{
			_pendingTokens.Enqueue(CreateToken(originalToken, _dedentTokenType, "<DEDENT>"));
		}		

		void EnqueueEOS(antlr.Token originalToken)
		{
			_pendingTokens.Enqueue(CreateToken(originalToken, _eosTokenType, "<EOS>"));
		}

		antlr.Token CreateToken(antlr.Token originalToken, int newTokenType, string newTokenText)
		{
			return new BooToken(originalToken, newTokenType, newTokenText);
		}
	}
}
