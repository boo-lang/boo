#region license
// Copyright (c) the Boo contributors
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
using System.IO;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;
using Boo.Lang.Environments;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;

namespace Boo.Lang.ParserA4;

/// <summary>
/// With this parser indentation is not used as
/// a block delimiter but COLON end.
///
/// class Foo:
/// def foo():
///    print 'Hello'
/// end
/// end
/// </summary>
public class WSABooParser : BooParser
{	
	public WSABooParser(ITokenStream lexer) : base(lexer)
	{
	}

	public static Module ParseModule(int tabSize, CompileUnit cu, string readerName, TextReader reader)
	{
		return ParseModule(tabSize, cu, readerName, reader, null);
	}

	public static Module ParseModule(int tabSize, CompileUnit cu, string readerName, TextReader reader, ParserErrorHandler eh)
	{
		if (Readers.IsEmpty(reader))
		{
			Module emptyModule = new Module(new LexicalInfo(readerName), ModuleNameFrom(readerName));
			cu.Modules.Add(emptyModule);
			return emptyModule;
		}

		var parser = CreateParser(tabSize, readerName, reader, eh);
		parser.BuildParseTree = true;
		var tree = parser.start();
		var visitor = new BooParserAstBuilderVisitor(cu, readerName);
		var module = visitor.VisitStart(tree);
		module.Name = ModuleNameFrom(readerName);
		return module;
	}

	private static string ModuleNameFrom(string readerName)
	{
		return Boo.Lang.Parser.CodeFactory.ModuleNameFrom(readerName);
	}

	public static WSABooParser CreateParser(int tabSize, string readerName, TextReader reader)
	{
		return CreateParser(tabSize, readerName, reader, null);
	}

	public static WSABooParser CreateParser(int tabSize, string readerName, TextReader reader, ParserErrorHandler eh)
	{
		var lexer = CreateBooLexer(tabSize, readerName, reader, eh);
		var parser = new WSABooParser(new CommonTokenStream(lexer));
		parser.Interpreter.PredictionMode = PredictionMode.LL;
		// Without a listener of its own the parser reports to the console and
		// the compiler never learns the parse failed.
		parser.RemoveErrorListeners();
		if (eh != null)
			parser.AddErrorListener(new BooErrorListener(eh, readerName));
		return parser;
	}
	
	public static ITokenSource CreateBooLexer(int tabSize, string readerName, TextReader reader)
	{
		return CreateBooLexer(tabSize, readerName, reader, null);
	}

	public static ITokenSource CreateBooLexer(int tabSize, string readerName, TextReader reader, ParserErrorHandler eh)
	{
		var lexer = new BooLexer(new AntlrInputStream(reader)) { TokenFactory = BooTokenA4.CreateTokenFactory(tabSize) } ;
		if (eh != null)
		{
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(new BooLexerErrorListener(eh, readerName));
		}

		var filter = new WSATokenStreamFilterA4(lexer);

		return filter;
	}
}
