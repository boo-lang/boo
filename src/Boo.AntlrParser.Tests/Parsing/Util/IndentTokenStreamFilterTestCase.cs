#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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
	public class IndentTokenStreamFilterTestCase : Assertion
	{
		const int TEXT = 5;

		const int WS = 6;

		const int INDENT = 7;

		const int DEDENT = 8;

		const int EOS = 9; // end of statement

		[Test]
		public void TestClass()
		{			
			Queue tokens = new Queue();			
			tokens.Enqueue(new SimpleToken(TEXT, "class"));
			tokens.Enqueue(new SimpleToken(WS, "   \t"));
			tokens.Enqueue(new SimpleToken(TEXT, "foo:"));			
			tokens.Enqueue(new SimpleToken(WS, "\n\t")); // i
			tokens.Enqueue(new SimpleToken(TEXT, "def foo():"));
			tokens.Enqueue(new SimpleToken(WS, "\n\t\t")); // i
			tokens.Enqueue(new SimpleToken(TEXT, "pass"));
			tokens.Enqueue(new SimpleToken(WS, "\n\t\n\n\t")); // eos, d
			tokens.Enqueue(new SimpleToken(TEXT, "def bar():"));
			tokens.Enqueue(new SimpleToken(WS, "\n\t\t")); // i
			tokens.Enqueue(new SimpleToken(TEXT, "pass"));
			tokens.Enqueue(new Token(Token.EOF_TYPE)); // eos, d, d
			
			TokenStream stream = new IndentTokenStreamFilter(new FakeStream(tokens), WS, INDENT, DEDENT, EOS);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(INDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(INDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(EOS, stream.nextToken().Type);
			AssertEquals(DEDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(INDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(EOS, stream.nextToken().Type);
			AssertEquals(DEDENT, stream.nextToken().Type);
			AssertEquals(DEDENT, stream.nextToken().Type);
			AssertEquals(Token.EOF_TYPE, stream.nextToken().Type);
		}

		[Test]
		public void TestTrailingWhiteSpace()
		{
			Queue queue = new Queue();
			queue.Enqueue(new SimpleToken(TEXT, "package"));
			queue.Enqueue(new SimpleToken(WS, " "));
			queue.Enqueue(new SimpleToken(TEXT, "Empty"));
			queue.Enqueue(new SimpleToken(WS, "\n\n\n")); // 1
			queue.Enqueue(new Token(Token.EOF_TYPE)); // 2

			IndentTokenStreamFilter stream = new IndentTokenStreamFilter(new FakeStream(queue), WS, INDENT, DEDENT, EOS);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(EOS, stream.nextToken().Type); // 1)
			AssertEquals(EOS, stream.nextToken().Type); // 2)
			AssertEquals(Token.EOF_TYPE, stream.nextToken().Type);
		}

		[Test]
		public void TestMultipleDedent()
		{
			Queue queue = new Queue();
			queue.Enqueue(new SimpleToken(TEXT, "class Math:"));
			queue.Enqueue(new SimpleToken(WS, "\n\t"));
			queue.Enqueue(new SimpleToken(TEXT, "def foo:"));
			queue.Enqueue(new SimpleToken(WS, "\n\t\t"));
			queue.Enqueue(new SimpleToken(TEXT, "pass"));
			queue.Enqueue(new SimpleToken(WS, "\n"));
			queue.Enqueue(new SimpleToken(TEXT, "print(3)"));
			queue.Enqueue(new Token(Token.EOF_TYPE));

			IndentTokenStreamFilter stream = new IndentTokenStreamFilter(new FakeStream(queue), WS, INDENT, DEDENT, EOS);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(INDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(INDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(EOS, stream.nextToken().Type);
			AssertEquals(DEDENT, stream.nextToken().Type);
			AssertEquals(DEDENT, stream.nextToken().Type);
			AssertEquals(TEXT, stream.nextToken().Type);
			AssertEquals(EOS, stream.nextToken().Type);
			AssertEquals(Token.EOF_TYPE, stream.nextToken().Type);
		}
	}
}
