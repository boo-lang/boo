namespace ImagNodeTest
{
	using System;
	using System.IO;
	using antlr;
	
	class AppMain {
		public static void Main(string[] args) {
			try {
				LangLexer lexer = new LangLexer(new CharBuffer(Console.In));
				LangParser parser = new LangParser(lexer);
				parser.block();
				CommonAST a = (CommonAST) parser.getAST();
				Console.Out.WriteLine(a.ToStringList());
				LangWalker walker = new LangWalker();
				walker.block(a);	// walk tree
				Console.Out.WriteLine("done walking");
			} catch(Exception e) {
				Console.Error.WriteLine("exception: "+e);
			}
		}
	}
}