using System;
using System.IO;
using antlr;
using antlr.collections;

class HeteroMain {
	public static void Main(string[] args) {
		new INTNode();
		try {
			CalcLexer lexer = new CalcLexer(new ByteBuffer(Console.OpenStandardInput()));
			CalcParser parser = new CalcParser(lexer);
			// Parse the input expression
			parser.expr();
			CalcAST t = (CalcAST)parser.getAST();

			// Print the resulting tree out in LISP notation
			Console.Out.WriteLine(t.ToStringTree());

			// XML serialize the tree, showing
			// different physical node class types
			TextWriter w = Console.Out;
			t.xmlSerialize(w);
			w.Write("\n");
			w.Flush();

			// Compute value and return
			int r = t.Value();
			Console.Out.WriteLine("value is "+r);
		} catch(Exception e) {
			Console.Error.WriteLine("exception: "+e);
			Console.Error.WriteLine(e.StackTrace);
		}
	}
}
