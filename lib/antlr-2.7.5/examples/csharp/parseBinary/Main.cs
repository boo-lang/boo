using System;
using System.IO;
using antlr;

class AppMain {
	public static void Main(string[] args) {
		try {
			DataLexer lexer = new DataLexer(new ByteBuffer(Console.OpenStandardInput()));
			DataParser parser = new DataParser(lexer);
			parser.file();
		} catch(Exception e) {
			Console.Error.WriteLine("exception: "+e);
		}
	}
}

