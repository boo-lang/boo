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

namespace Boo.Lang.Parser.Tests.Util

import System
import System.Collections.Generic
import Antlr4.Runtime
import Boo.Lang.Parser.Util
import NUnit.Framework

// Hands the filter a fixed sequence, so each case states its own input.
class FakeStream(ITokenSource):
	private final _tokens as Queue[of IToken]

	private _tokenFactory as ITokenFactory = CommonTokenFactory.Default

	def constructor(tokens as Queue[of IToken]):
		_tokens = tokens

	def NextToken() as IToken:
		return (_tokens.Dequeue() if _tokens.Count > 0 else null)

	Line as int:
		get: return 0

	Column as int:
		get: return 0

	InputStream as ICharStream:
		get: return null

	SourceName as string:
		get: return "fake"

	TokenFactory as ITokenFactory:
		get: return _tokenFactory
		set: _tokenFactory = value

// The filter builds INDENT, DEDENT and EOS tokens from whichever token it
// was looking at, so these need a real stream behind them.
class SimpleToken(CommonToken):
	private static final Source as ICharStream = AntlrInputStream(string.Empty, name: "fake")

	def constructor(type as int, txt as string):
		super(Tuple.Create[of ITokenSource, ICharStream](null, Source), type, TokenConstants.DefaultChannel, 0, 0)
		Text = txt

[TestFixture]
class IndentTokenStreamFilterTestCase:
	"""
	Summary description for Class1.
	"""
	final TEXT = 5

	final WS = 6

	final INDENT = 7

	final DEDENT = 8

	final EOS = 9 // end of statement

	final END = 27 // end keyword

	final ID = 81 // id keyword

	[Test]
	def TestClass():
		tokens = (of IToken:
			SimpleToken(TEXT, "class"),
			SimpleToken(WS, "   \t"),
			SimpleToken(TEXT, "foo:"),
			SimpleToken(WS, "\n\t"),// i
			SimpleToken(TEXT, "def foo():"),
			SimpleToken(WS, "\n\t\t"), // i
			SimpleToken(TEXT, "pass"),
			SimpleToken(WS, "\n\t\n\n\t"), // eos, d
			SimpleToken(TEXT, "def bar():"),
			SimpleToken(WS, "\n\t\t"), // i
			SimpleToken(TEXT, "pass"),
			SimpleToken(TokenConstants.EOF, "<EOF>") // eos, d, d
		)

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
						TokenConstants.EOF)

	[Test]
	def TestTrailingWhiteSpace():
		tokens = (of IToken:
			SimpleToken(TEXT, "package"),
			SimpleToken(WS, " "),
			SimpleToken(TEXT, "Empty"),
			SimpleToken(WS, "\n\n\n"), // 1
			SimpleToken(TokenConstants.EOF, "<EOF>") // 2
		)

		AssertTokenSequence(tokens, TEXT, TEXT, EOS, EOS, TokenConstants.EOF)

	[Test]
	def TestMultipleDedent():
		tokens = (of IToken:
			SimpleToken(TEXT, "class Math:"),
			SimpleToken(WS, "\n\t"),
			SimpleToken(TEXT, "def foo:"),
			SimpleToken(WS, "\n\t\t"),
			SimpleToken(TEXT, "pass"),
			SimpleToken(WS, "\n"),
			SimpleToken(TEXT, "print(3)"),
			SimpleToken(TokenConstants.EOF, "<EOF>")
		)

		AssertTokenSequence(tokens,
				TEXT, INDENT, TEXT, INDENT, TEXT,
				EOS, DEDENT, DEDENT, TEXT, EOS, TokenConstants.EOF)

	[Test]
	def TestWhitespaceWithSkipInBetween():
		/*
		a:
			b:
				c
		// comment
			d
		*/
		tokens = (of IToken:
			SimpleToken(TEXT, "a:"),
			SimpleToken(WS, "\n\t"),
			SimpleToken(TEXT, "b:"),
			SimpleToken(WS, "\n\t\t"),
			SimpleToken(TEXT, "c"),
			SimpleToken(WS, "\n"),
			SimpleToken(WS, "\n\t"),
			SimpleToken(TEXT, "d"),
			SimpleToken(WS, "\n"),
			SimpleToken(TokenConstants.EOF, "<EOF>")
		)

		AssertTokenSequence(tokens,
						TEXT, INDENT, TEXT,
						INDENT, TEXT, EOS,
						DEDENT, TEXT, EOS, DEDENT, EOS, TokenConstants.EOF)

	def AssertTokenSequence(tokens as (IToken), *expectedSequence as (int)):
		queue = Queue[of IToken]()
		for token in tokens:
			queue.Enqueue(token)

		// These cases carry the newline inside the whitespace token, as the 2.7
		// lexer did, so the filter is told both types are the same one.
		stream = IndentTokenStreamFilter(FakeStream(queue), WS, WS, INDENT, DEDENT, EOS, END, ID)

		// The filter keeps whitespace on the hidden channel rather than dropping
		// it, and a CommonTokenStream hides that from the parser. Assert what
		// the parser is given.
		index = 0
		for expected in expectedSequence:
			actual as IToken
			while true:
				actual = stream.NextToken()
				break if actual.Channel == TokenConstants.DefaultChannel

			Assert.AreEqual(expected, actual.Type, "sequence item: " + index)
			index += 1
