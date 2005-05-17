using System;
//import java.io.*;

using CommonAST				= antlr.CommonAST;
using AST					= antlr.collections.AST;
//using DumpASTVisitor		= antlr.DumpASTVisitor;
using CharBuffer			= antlr.CharBuffer;
using RecognitionException	= antlr.RecognitionException;
using TokenStreamException	= antlr.TokenStreamException;

// wh: bug(?) in DotGNU 0.6 - "using antlr" will workaround the problem.
#if __CSCC__
using antlr;
#endif

class Calc 
{
	public static void Main(string[] args) 
	{
		try 
		{
			CalcLexer lexer = new CalcLexer(new CharBuffer(Console.In));
			lexer.setFilename("<stdin>");
			CalcParser parser = new CalcParser(lexer);
			parser.setFilename("<stdin>");

			// Parse the input expression
			parser.expr();
			CommonAST t = (CommonAST)parser.getAST();
			
			// Print the resulting tree out in LISP notation
			Console.Out.WriteLine(t.ToStringTree());
			CalcTreeWalker walker = new CalcTreeWalker();
			
			// Traverse the tree created by the parser
			float r = walker.expr(t);
			Console.Out.WriteLine("value is "+r);
		}
		catch(TokenStreamException e) 
		{
			Console.Error.WriteLine("exception: "+e);
		}
		catch(RecognitionException e) 
		{
			Console.Error.WriteLine("exception: "+e);
		}
	}  
}
