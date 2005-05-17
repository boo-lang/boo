using System;
using antlr;

class Test 
{
	public static void Main(string[] args) 
	{
		try 
		{
			TinyCLexer  lexer  = new TinyCLexer(new ByteBuffer(Console.OpenStandardInput()));
			TinyCParser parser = new TinyCParser(lexer);
			parser.program();

			ParseTree tree = parser.getParseTree();
			Console.Out.WriteLine("parse tree: \n"+tree.ToStringTree());
			/*
			Console.Out.WriteLine("derivation steps: "+parser.getNumberOfDerivationSteps());
			Console.Out.WriteLine("derivation step 0: "+tree.getLeftmostDerivationStep(0));
			Console.Out.WriteLine("derivation step 1: "+tree.getLeftmostDerivationStep(1));
			Console.Out.WriteLine("derivation step 2: "+tree.getLeftmostDerivationStep(2));
			Console.Out.WriteLine("derivation step 3: "+tree.getLeftmostDerivationStep(3));
			*/

			int nSteps = parser.getNumberOfDerivationSteps();
			Console.Out.WriteLine("derivation:\n" + tree.getLeftmostDerivation(nSteps));

		}
		catch(Exception e) 
		{
			Console.Error.WriteLine("exception: "+e);
		}
	}
}
