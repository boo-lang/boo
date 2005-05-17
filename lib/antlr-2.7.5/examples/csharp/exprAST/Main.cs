using System;
//import java.io.*;
//import ExprLexer;
//import ExprParser;
using AST				= antlr.collections.AST;
using CharBuffer		= antlr.CharBuffer;

// wh: bug(?) in DotGNU 0.6 - "using antlr" will workaround the problem.
#if __CSCC__
using antlr;
#endif

class AppMain {
	public static void Main(string[] args) 
	{
		try {
			ExprLexer lexer = new ExprLexer(new CharBuffer(Console.In));
			ExprParser parser = new ExprParser(lexer);

			// set the type of tree node to create; this is default action
			// so it is unnecessary to do it here, but demos capability.
			parser.setASTNodeClass("antlr.CommonAST");

			parser.expr();
			antlr.CommonAST ast = (antlr.CommonAST)parser.getAST();
			if (ast != null) {
				Console.Out.WriteLine(ast.ToStringList());
			} else {
				Console.Out.WriteLine("null AST");
			}
		} catch(Exception e) {
			Console.Error.WriteLine("exception: "+e);
		}
	}
}

