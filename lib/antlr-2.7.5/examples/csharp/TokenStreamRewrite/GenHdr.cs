using System;
using System.IO;
using antlr;

class GenHdr 
{
	public static void Main(string[] args) 
	{
		try {
			TinyCLexer lexer = null;

			if ( args.Length > 0 ) 
			{
				lexer = new TinyCLexer(new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read)));
			}
			else 
			{
				lexer = new TinyCLexer(new ByteBuffer(Console.OpenStandardInput()));
			}
			lexer.setTokenObjectClass(typeof(TokenWithIndex).FullName);
			TokenStreamRewriteEngine rewriteEngine = new TokenStreamRewriteEngine(lexer);
			rewriteEngine.discard(TinyCLexer.WS);
			TinyCParser parser = new TinyCParser(rewriteEngine);
			parser.program();
			Console.Out.Write(rewriteEngine.ToString());
		} 
		catch(Exception e) 
		{
			Console.Error.WriteLine("exception: "+e);
		}
	}
}
