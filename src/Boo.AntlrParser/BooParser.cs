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
using System.IO;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.AntlrParser.Util;

namespace Boo.AntlrParser
{
	public class BooParser : BooParserBase
	{	
		const int TabSize = 4;
		
		const string TokenObjectClass = "Boo.AntlrParser.BooToken";
		
		protected ParserErrorHandler Error;

		public BooParser(antlr.TokenStream lexer) : base(lexer)
		{
		}

		public static CompileUnit ParseFile(string fname)
		{
			if (null == fname)
			{
				throw new ArgumentNullException("fname");
			}
	
			using (StreamReader reader = File.OpenText(fname))
			{
				return ParseReader(fname, reader);
			}
		}
		
		public static CompileUnit ParseString(string name, string text)
		{
			return ParseReader(name, new StringReader(text));
		}

		public static CompileUnit ParseReader(string readerName, TextReader reader)
		{		
			CompileUnit cu = new CompileUnit();
			cu.Modules.Add(ParseModule(readerName, reader, null));
			return cu;
		}
	
		public static Module ParseModule(string readerName, TextReader reader, ParserErrorHandler errorHandler)
		{		
			BooParser parser = new BooParser(CreateBooLexer(readerName, reader));
			parser.setFilename(readerName);
			parser.Error += errorHandler;
		
			Module module = parser.start();
			module.Name = CreateModuleName(readerName);
			return module;
		}
		
		public static antlr.TokenStream CreateBooLexer(string readerName, TextReader reader)
		{
			antlr.TokenStreamSelector selector = new antlr.TokenStreamSelector();
		
			BooLexer lexer = new BooLexer(reader);
			lexer.setFilename(readerName);
			lexer.Initialize(selector, TabSize, BooToken.Creator);
		
			IndentTokenStreamFilter filter = new IndentTokenStreamFilter(lexer, WS, INDENT, DEDENT, EOS);
			selector.select(filter);
			
			return selector;
		}

		override public void reportError(antlr.RecognitionException x)
		{
			if (null != Error)
			{
				Error(x);
			}
			else
			{
				base.reportError(x);
			}
		}

		public static string CreateModuleName(string readerName)
		{
			if (readerName.IndexOfAny(Path.InvalidPathChars) > -1)
			{
				return EncodeModuleName(readerName);
			}
			return Path.GetFileNameWithoutExtension(Path.GetFileName(readerName));
		}

		static string EncodeModuleName(string name)
		{
			StringBuilder buffer = new StringBuilder(name.Length);
			foreach (char ch in name)
			{
				if (Char.IsLetterOrDigit(ch))
				{
					buffer.Append(ch);
				}
				else
				{
					buffer.Append("_");
				}
			}
			return buffer.ToString();
		}
	}
}
