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

namespace Boo.Lang.Useful.IO

import System.IO
import System.Collections
import Boo.Lang.Useful.IO.Impl

class PreProcessor:
"""
Implements a c# style preprocessor with support for #if, #else and #endif
directives.

The operators ||, && and ! are supported.

Example:
	text = "#if SPAM\nSPAM, SPAM, SPAM!\n#endif"
	pp = PreProcessor()
	pp.Define("SPAM")
	resultingText = pp.Process(text)
	print resultingText # SPAM, SPAM, SPAM!
"""
	_symbols = Hashtable()
	
	[property(PreserveLines)] _preserveLines = false
	
	def constructor():
		pass
		
	def constructor(symbols as string*):
		for symbol in symbols:
			Define(symbol)
		
	def Define([required] symbol as string):
		_symbols[symbol] = symbol
		
	def IsDefined([required] symbol as string):
		return symbol in _symbols
		
	def Process([required] text as string):
		writer = StringWriter()
		Process(StringReader(text), writer)
		return writer.ToString()
		
	def Process([required] reader as TextReader, [required] writer as TextWriter):
		Parser(_symbols, reader, writer, _preserveLines).Parse()
		
	class Parser:
		IfPattern = /^\s*#if\s+((.|\s)+)$/	
		ElsePattern = /^\s*#else\s*$/	
		EndIfPattern = /^\s*#endif\s*$/
		
		_reader as TextReader
		_writer as TextWriter
		_evaluator = PreProcessorExpressionEvaluator()
		_preserveLines as bool
		
		def constructor(symbolTable, reader, writer, preserveLines as bool):
			_evaluator.SymbolTable = symbolTable
			_reader = reader
			_writer = writer
			_preserveLines = preserveLines
		
		def Parse():
			while (line = _reader.ReadLine()) is not null:
				ParseLine(true, line)
				
		private def ParseLine(context as bool, [required] line as string):
			m = IfPattern.Match(line)
			if m.Success:
				SkippedLine()
				expression = m.Groups[1].Value
				ParseIfBlock(context, expression)
				return
				
			if context:
				_writer.WriteLine(line)
			else:
				SkippedLine()
				
		private def SkippedLine():
			if _preserveLines: _writer.WriteLine()
					
		private def ParseIfBlock(context as bool, expression as string):
			localContext = context and Evaluate(expression)
			while (line = _reader.ReadLine()) is not null:
				if EndIfPattern.IsMatch(line):
					SkippedLine()
					break			
							
				if ElsePattern.IsMatch(line):
					localContext = context and not localContext
					SkippedLine()
				else:
					ParseLine(localContext, line)
					
		private def Evaluate(expression as string):
			return _evaluator.expr(ParseExpression(expression))
			
		private def ParseExpression(expression as string) as antlr.CommonAST:
			lexer = PreProcessorExpressionLexer(antlr.CharBuffer(StringReader(expression)))
			lexer.setFilename("<expression>")
			parser = PreProcessorExpressionParser(lexer)
			parser.setFilename("<expression>")
			parser.expr()
			return parser.getAST()
