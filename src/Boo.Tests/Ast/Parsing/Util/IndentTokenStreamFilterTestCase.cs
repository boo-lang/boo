#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
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
