#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using Boo.AntlrParser.Util;
using NUnit.Framework;
using antlr;

namespace Boo.AntlrParser.Tests.Util
{
	class FakeStream : antlr.TokenStream
	{
		protected Queue _tokens;

		public FakeStream(Queue tokens)
		{
			_tokens = tokens;
		}

		public antlr.Token nextToken()
		{
			if (_tokens.Count > 0)
			{
				return _tokens.Dequeue() as antlr.Token;
			}
			return null;
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
