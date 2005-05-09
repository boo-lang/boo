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
using Boo.Lang.Parser.Util;
using NUnit.Framework;
using antlr;

namespace Boo.Lang.Parser.Tests.Util
{
	class FakeStream : antlr.TokenStream
	{
		protected Queue _tokens;

		public FakeStream(Queue tokens)
		{
			_tokens = tokens;
		}

		public antlr.IToken nextToken()
		{
			if (_tokens.Count > 0)
			{
				return _tokens.Dequeue() as antlr.IToken;
			}
			return null;
		}
	}
	
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
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class IndentTokenStreamFilterTestCase
	{
		const int TEXT = 5;

		const int WS = 6;

		const int INDENT = 7;

		const int DEDENT = 8;

		const int EOS = 9; // end of statement

		[Test]
		public void TestClass()
		{			
			Token[] tokens = new Token[]
			{
				new SimpleToken(TEXT, "class"),
				new SimpleToken(WS, "   \t"),
				new SimpleToken(TEXT, "foo:"),			
				new SimpleToken(WS, "\n\t"),// i
				new SimpleToken(TEXT, "def foo():"),
				new SimpleToken(WS, "\n\t\t"), // i
				new SimpleToken(TEXT, "pass"),
				new SimpleToken(WS, "\n\t\n\n\t"), // eos, d
				new SimpleToken(TEXT, "def bar():"),
				new SimpleToken(WS, "\n\t\t"), // i
				new SimpleToken(TEXT, "pass"),
				new Token(Token.EOF_TYPE) // eos, d, d
			};
			
			AssertTokenSequence(tokens,
							TEXT,
							TEXT,
							INDENT,
							TEXT,
							INDENT,
							TEXT,
							EOS,
							DEDENT,
							TEXT, 
							INDENT,
							TEXT,
							EOS,
							DEDENT,
							DEDENT,
							Token.EOF_TYPE);			
		}

		[Test]
		public void TestTrailingWhiteSpace()
		{			
			Token[] tokens = new Token[] {
				new SimpleToken(TEXT, "package"),
				new SimpleToken(WS, " "),
				new SimpleToken(TEXT, "Empty"),
				new SimpleToken(WS, "\n\n\n"), // 1
				new Token(Token.EOF_TYPE) // 2
			};
			
			AssertTokenSequence(tokens, TEXT, TEXT, EOS, EOS, Token.EOF_TYPE);
		}

		[Test]
		public void TestMultipleDedent()
		{
			Token[] tokens = new Token[] {
				new SimpleToken(TEXT, "class Math:"),
				new SimpleToken(WS, "\n\t"),
				new SimpleToken(TEXT, "def foo:"),
				new SimpleToken(WS, "\n\t\t"),
				new SimpleToken(TEXT, "pass"),
				new SimpleToken(WS, "\n"),
				new SimpleToken(TEXT, "print(3)"),
				new Token(Token.EOF_TYPE)
			};
			
			AssertTokenSequence(tokens,
					TEXT, INDENT, TEXT, INDENT, TEXT,
					EOS, DEDENT, DEDENT, TEXT, EOS, Token.EOF_TYPE);
		}
		
		[Test]
		public void TestWhitespaceWithSkipInBetween()
		{
			/*
			a:
				b:
					c
			// comment
				d
			*/
			Token[] tokens = new Token[] {
				new SimpleToken(TEXT, "a:"),
				new SimpleToken(WS, "\n\t"),
				new SimpleToken(TEXT, "b:"),
				new SimpleToken(WS, "\n\t\t"),
				new SimpleToken(TEXT, "c"),
				new SimpleToken(WS, "\n"),
				new SimpleToken(WS, "\n\t"),
				new SimpleToken(TEXT, "d"),
				new SimpleToken(WS, "\n"),
				new Token(Token.EOF_TYPE)
			};
			
			AssertTokenSequence(tokens,
							TEXT, INDENT, TEXT,
							INDENT, TEXT, EOS,
							DEDENT, TEXT, EOS, DEDENT, EOS, Token.EOF_TYPE);
			
		}
		
		void AssertTokenSequence(Token[] tokens, params int[] expectedSequence)
		{
			Queue queue = new Queue();
			foreach (Token token in tokens)
			{
				queue.Enqueue(token);
			}
			
			IndentTokenStreamFilter stream = new IndentTokenStreamFilter(new FakeStream(queue), WS, INDENT, DEDENT, EOS);
			
			int index=0;
			foreach (int expected in expectedSequence)
			{
				Assert.AreEqual(expected, stream.nextToken().Type, "sequence item: " + (index++));
			}
		}
	}
}
