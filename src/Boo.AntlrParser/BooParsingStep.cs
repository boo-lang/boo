#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;

namespace Boo.AntlrParser
{
	/// <summary>
	/// Step 1. Parses any input fed to the compiler.
	/// </summary>
	public class BooParsingStep : ICompilerStep
	{
		CompilerContext _context;

		public BooParsingStep()
		{
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
			try
			{
				ParserErrorHandler errorHandler = new ParserErrorHandler(OnParserError);

				foreach (ICompilerInput input in _context.Parameters.Input)
				{
					try
					{
						using (System.IO.TextReader reader = input.Open())
						{
							Module module = BooParser.ParseModule(input.Name, reader, errorHandler);
							if (null != module)
							{
								_context.CompileUnit.Modules.Add(module);
							}
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
			finally
			{
				_context = null;
			}
		}

		void OnParserError(antlr.RecognitionException error)
		{			
			LexicalInfo data = new LexicalInfo(error.getFilename(), error.getLine(), error.getColumn(), error.getColumn());

			antlr.NoViableAltException nvae = error as antlr.NoViableAltException;
			if (null != nvae)
			{
				ParserError(data, nvae);
			}
			else
			{
				_context.Errors.Add(CompilerErrorFactory.GenericParserError(data, error));
			}
		}
		
		void ParserError(LexicalInfo data, antlr.NoViableAltException error)
		{			
			_context.Errors.Add(CompilerErrorFactory.UnexpectedToken(data, error, error.token.getText()));
		}

	}
}
