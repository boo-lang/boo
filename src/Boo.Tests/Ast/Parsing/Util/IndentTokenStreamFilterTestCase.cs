using System;
using System.Collections;
using Boo.Ast.Parsing.Util;
using NUnit.Framework;
using antlr;

namespace Boo.Tests.Ast.Parsing.Util
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
