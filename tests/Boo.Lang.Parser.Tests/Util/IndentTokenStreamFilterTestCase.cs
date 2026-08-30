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
using Boo.Lang.Parser.Util;
using NUnit.Framework;

namespace Boo.Lang.Parser.Tests.Util
{
	/// Hands the filter a fixed sequence, so each case states its own input.
	class FakeStream : ITokenSource
	{
		private readonly Queue<IToken> _tokens;

		public FakeStream(Queue<IToken> tokens)
		{
			_tokens = tokens;
		}

		public IToken NextToken() => _tokens.Count > 0 ? _tokens.Dequeue() : null;

		public int Line => 0;
		public int Column => 0;
		public ICharStream InputStream => null;
		public string SourceName => "fake";
		public ITokenFactory TokenFactory { get; set; } = CommonTokenFactory.Default;
	}

	/// The filter builds INDENT, DEDENT and EOS tokens from whichever token it
	/// was looking at, so these need a real stream behind them.
	public class SimpleToken : CommonToken
	{
		private static readonly ICharStream Source = new AntlrInputStream(string.Empty) { name = "fake" };

		public SimpleToken(int type, string txt)
			: base(Tuple.Create((ITokenSource)null, Source), type, TokenConstants.DefaultChannel, 0, 0)
		{
			Text = txt;
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

		const int END = 27; // end keyword

		const int ID = 81; // id keyword

		[Test]
		public void TestClass()
		{			
			IToken[] tokens = new IToken[]
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
				new SimpleToken(TokenConstants.EOF, "<EOF>") // eos, d, d
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
							TokenConstants.EOF);			
		}

		[Test]
		public void TestTrailingWhiteSpace()
		{			
			IToken[] tokens = new IToken[] {
				new SimpleToken(TEXT, "package"),
				new SimpleToken(WS, " "),
				new SimpleToken(TEXT, "Empty"),
				new SimpleToken(WS, "\n\n\n"), // 1
				new SimpleToken(TokenConstants.EOF, "<EOF>") // 2
			};
			
			AssertTokenSequence(tokens, TEXT, TEXT, EOS, EOS, TokenConstants.EOF);
		}

		[Test]
		public void TestMultipleDedent()
		{
			IToken[] tokens = new IToken[] {
				new SimpleToken(TEXT, "class Math:"),
				new SimpleToken(WS, "\n\t"),
				new SimpleToken(TEXT, "def foo:"),
				new SimpleToken(WS, "\n\t\t"),
				new SimpleToken(TEXT, "pass"),
				new SimpleToken(WS, "\n"),
				new SimpleToken(TEXT, "print(3)"),
				new SimpleToken(TokenConstants.EOF, "<EOF>")
			};
			
			AssertTokenSequence(tokens,
					TEXT, INDENT, TEXT, INDENT, TEXT,
					EOS, DEDENT, DEDENT, TEXT, EOS, TokenConstants.EOF);
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
			IToken[] tokens = new IToken[] {
				new SimpleToken(TEXT, "a:"),
				new SimpleToken(WS, "\n\t"),
				new SimpleToken(TEXT, "b:"),
				new SimpleToken(WS, "\n\t\t"),
				new SimpleToken(TEXT, "c"),
				new SimpleToken(WS, "\n"),
				new SimpleToken(WS, "\n\t"),
				new SimpleToken(TEXT, "d"),
				new SimpleToken(WS, "\n"),
				new SimpleToken(TokenConstants.EOF, "<EOF>")
			};
			
			AssertTokenSequence(tokens,
							TEXT, INDENT, TEXT,
							INDENT, TEXT, EOS,
							DEDENT, TEXT, EOS, DEDENT, EOS, TokenConstants.EOF);
			
		}
		
		void AssertTokenSequence(IToken[] tokens, params int[] expectedSequence)
		{
			var queue = new Queue<IToken>();
			foreach (var token in tokens)
				queue.Enqueue(token);

			// These cases carry the newline inside the whitespace token, as the 2.7
			// lexer did, so the filter is told both types are the same one.
			var stream = new IndentTokenStreamFilter(new FakeStream(queue), WS, WS, INDENT, DEDENT, EOS, END, ID);

			// The filter keeps whitespace on the hidden channel rather than dropping
			// it, and a CommonTokenStream hides that from the parser. Assert what
			// the parser is given.
			var index = 0;
			foreach (var expected in expectedSequence)
			{
				IToken actual;
				do
				{
					actual = stream.NextToken();
				}
				while (actual.Channel != TokenConstants.DefaultChannel);

				Assert.AreEqual(expected, actual.Type, "sequence item: " + (index++));
			}
		}
	}
}
