namespace Test
{
	using System;
	using System.IO;
	using antlr;

	class main {
		public static void Main(string[] args) {
			try {
				TinyCLexer lexer = new TinyCLexer(new CharBuffer(Console.In));
				MyCParser parser = new MyCParser(lexer);
				parser.program();
			} catch(Exception e) {
				Console.Error.WriteLine("exception: "+e);
			}
		}
	}
}
