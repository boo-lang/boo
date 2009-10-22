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
using antlr;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;

namespace Boo.Lang.Parser
{
	/// <summary>
	/// Step 1. Parses any input fed to the compiler.
	/// </summary>
	public class BooParsingStep : ICompilerStep
	{
		CompilerContext _context;
		
		int _tabSize = BooParser.DefaultTabSize;
		
		protected CompilerContext Context
		{
			get { return _context; }
		}
		
		public int TabSize
		{
			get { return _tabSize; }
			
			set
			{
				if (value < 1) throw new ArgumentOutOfRangeException("TabSize");
				_tabSize = value;
			}
		}
		
		public void Initialize(CompilerContext context)
		{
			_context = context;
		}
		
		public void Dispose()
		{
			_context = null;
		}

		public void Run()
		{		
			ParserErrorHandler errorHandler = OnParserError;
				
			foreach (ICompilerInput input in _context.Parameters.Input)
			{
				try
				{
					using (System.IO.TextReader reader = input.Open())
					{
						ParseModule(input.Name, reader, errorHandler);
					}
				}				
				catch (CompilerError error)
				{
					_context.Errors.Add(error);
				}
				catch (antlr.TokenStreamRecognitionException x)
				{
					OnParserError(x.recog);
				}
				catch (Exception x)
				{
					_context.Errors.Add(CompilerErrorFactory.InputError(input.Name, x));
				}
			}
		}
		
		protected virtual void ParseModule(string inputName, System.IO.TextReader reader, ParserErrorHandler errorHandler)
		{
			BooParser.ParseModule(_tabSize, _context.CompileUnit, inputName, reader, errorHandler); 
		}

		void OnParserError(antlr.RecognitionException error)
		{			
			LexicalInfo data = new LexicalInfo(error.getFilename(), error.getLine(), error.getColumn());

			antlr.NoViableAltException nvae = error as antlr.NoViableAltException;
			if (null != nvae)
			{
				ParserError(data, nvae);
			}
			else
			{
				GenericParserError(data, error);
			}
		}

		private void GenericParserError(LexicalInfo data, RecognitionException error)
		{
			_context.Errors.Add(CompilerErrorFactory.GenericParserError(data, error));
		}

		void ParserError(LexicalInfo data, antlr.NoViableAltException error)
		{			
			_context.Errors.Add(CompilerErrorFactory.UnexpectedToken(data, error, error.token.getText()));
		}

	}
}
