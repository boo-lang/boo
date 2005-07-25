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
Implements a c# style preprocessor.

Example:
	text = "#if SPAM\nSPAM, SPAM, SPAM!\n#endif"
	pp = PreProcessor()
	pp.Define("SPAM")
	resultingText = pp.Process(text)
	print resultingText # SPAM, SPAM, SPAM!
"""
	
	callable StringPredicate(s as string) as bool
	
	IfPattern = /^\s*#if\s+((.|\s)+)$/
	
	ElsePattern = /^\s*#else\s*$/
	
	EndIfPattern = /^\s*#endif\s*$/
	
	NullPredicate as StringPredicate = def (s as string):
		return false
		
	EndIfCondition as StringPredicate = def (s as string):
		return EndIfPattern.IsMatch(s)
	
	_symbols = Hashtable()
	
	_evaluator = PreProcessorExpressionEvaluator(SymbolTable: _symbols)
		
	def Define([required] symbol as string):
		_symbols[symbol] = symbol
		
	def IsDefined([required] symbol as string):
		return symbol in _symbols
		
	def Process([required] text as string):
		writer = StringWriter()
		Process(StringReader(text), writer)
		return writer.ToString()
		
	def Process([required] reader as TextReader, [required] writer as TextWriter):
		Parse(reader, writer, true, NullPredicate)
		
	private def Parse(reader as TextReader,
				writer as TextWriter,
				printLines as bool,
				endCondition as StringPredicate):
					
		line as string
		while (line = reader.ReadLine()) is not null:
			break if endCondition(line)
			
			m = IfPattern.Match(line)
			if m.Success:
				expression = m.Groups[1].Value
				if Evaluate(expression):
					Parse(reader, writer, true, EndIfCondition)
				else:
					Parse(reader, writer, false, EndIfCondition)
			elif ElsePattern.IsMatch(line):
				Parse(reader, writer, not printLines, endCondition)
			else:
				writer.WriteLine(line) if printLines
				
	private def Evaluate(expression as string):
		return _evaluator.expr(ParseExpression(expression))
		
	private def ParseExpression(expression as string) as antlr.CommonAST:
		lexer = PreProcessorExpressionLexer(antlr.CharBuffer(StringReader(expression)))
		lexer.setFilename("<expression>")
		parser = PreProcessorExpressionParser(lexer)
		parser.setFilename("<expression>")
		parser.expr()
		return parser.getAST()
