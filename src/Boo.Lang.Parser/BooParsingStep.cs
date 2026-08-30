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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Environments;
using Boo.Lang.Parser.Util;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Boo.Lang.Parser;

/// <summary>
/// Step 1. Parses any input fed to the compiler.
/// 
/// Parsing behaviour can be customized by providing a specific <see cref="ParserSettings"/> instance through
/// <see cref="CompilerParameters.Environment" />.
/// </summary>
public class BooParsingStep : ICompilerStep
{
	CompilerContext _context;
	
	protected CompilerContext Context => _context;
	
	public void Initialize(CompilerContext context)
	{
		_context = context;
	}
	
	public void Dispose()
	{
		_context = null;
	}

	protected int TabSize => My<Boo.Lang.Parser.ParserSettings>.Instance.TabSize;

	public void Run()
	{
		// Parser errors are reported through the ambient settings, so the handler
		// that was there before this step ran has to come back afterwards.
		var settings = My<Boo.Lang.Parser.ParserSettings>.Instance;
		var previousHandler = settings.ErrorHandler;
		settings.ErrorHandler = OnParserError;

		try
		{
			ParseInputs();
		}
		finally
		{
			settings.ErrorHandler = previousHandler;
		}
	}

	private void ParseInputs()
	{
		foreach (var input in _context.Parameters.Input)
		{
			// Each input is its own run of errors. WSABooParsingStep overrides
			// ParseModule, so the reset belongs here rather than in it.
			_lastErrorLine = -1;

			try
			{
				using (var reader = input.Open())
					ParseModule(input.Name, reader);
			}				
			catch (CompilerError error)
			{
				_context.Errors.Add(error);
			}
			catch (Exception x)
			{
				_context.Errors.Add(CompilerErrorFactory.InputError(input.Name, x));
			}
		}
	}

	protected virtual void ParseModule(string inputName, System.IO.TextReader reader)
	{
		var settings = My<Boo.Lang.Parser.ParserSettings>.Instance;
		var stream = new AntlrInputStream(reader);
		BooParser.StartContext tree;

		// SLL first, as BooParser.ParseModule does. It reports nothing, because it
		// still calls the listener before bailing and the LL retry repeats it.
		try
		{
			tree = BooParser.CreateParser(settings.TabSize, inputName, stream, true, null).start();
		}
		catch (ParseCanceledException)
		{
			stream.Seek(0);
			tree = BooParser.CreateParser(settings.TabSize, inputName, stream, false, OnParserError).start();
		}

		var visitor = new BooParserAstBuilderVisitor(_context.CompileUnit, inputName);
		visitor.VisitStart(tree);
	}

	private int _lastErrorLine = -1;

	/// <summary>
	/// ANTLR 4 reports a missing or extraneous token with no exception at all,
	/// so an error is not conditional on there being one.
	/// </summary>
	protected void OnParserError(IRecognizer recognizer, IToken offendingSymbol, string filename, int line, int charPositionInLine, string msg, RecognitionException e)
	{
		// Errors close behind another are ANTLR 4 resynchronising, not separate
		// problems. boo.g never emitted the cascade, so it needed no such check.
		if (_lastErrorLine != -1 && line - _lastErrorLine < 3)
		{
			_lastErrorLine = line;
			return;
		}
		_lastErrorLine = line;

		var location = new LexicalInfo(filename, line, charPositionInLine);
		var friendly = BooErrorPatterns.Match(recognizer, offendingSymbol, e);
		if (friendly != null)
		{
			_context.Errors.Add(CompilerErrorFactory.GenericParserError(location, new Exception(friendly)));
			return;
		}

		// Name the token, as boo.g did. ANTLR's own text describes its recovery
		// rather than the problem, so it is the last resort.
		if (offendingSymbol != null)
			_context.Errors.Add(CompilerErrorFactory.UnexpectedToken(location, e, offendingSymbol.Text));
		else
			_context.Errors.Add(CompilerErrorFactory.GenericParserError(location, new Exception(msg)));
	}


	void ParserError(LexicalInfo data, NoViableAltException error, IToken offendingSymbol)
	{
		_context.Errors.Add(CompilerErrorFactory.UnexpectedToken(data, error, offendingSymbol.Text));
	}
}
