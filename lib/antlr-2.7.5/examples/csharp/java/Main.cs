using System;
using FileInfo		= System.IO.FileInfo;
using Directory		= System.IO.Directory;
using FileStream	= System.IO.FileStream;
using FileMode		= System.IO.FileMode;
using FileAccess	= System.IO.FileAccess;
using Stream		= System.IO.Stream;
using StreamReader	= System.IO.StreamReader;

using BaseAST				= antlr.BaseAST;
using CommonAST				= antlr.CommonAST;
using ASTFactory			= antlr.ASTFactory;
using RecognitionException	= antlr.RecognitionException;
using AST					= antlr.collections.AST;
using ASTFrame				= antlr.debug.misc.ASTFrame;

// bug(?) in DotGNU 0.6 - "using antlr" will workaround the problem.
#if __CSCC__
using antlr;
#endif

class AppMain
{
	
	internal static bool showTree = false;

	public static void  Main(string[] args)
	{
		// Use a try/catch block for parser exceptions
		try
		{
			// if we have at least one command-line argument
			if (args.Length > 0)
			{
				Console.Error.WriteLine("Parsing...");				
				// for each directory/file specified on the command line
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].Equals("-showtree"))
					{
						showTree = true;
					}
					else
					{
						doFile(new FileInfo(args[i])); // parse it
					}
				}
			}
			else
				Console.Error.WriteLine("Usage: java Main [-showtree] " + "<directory or file name>");
		}
		catch (System.Exception e)
		{
			Console.Error.WriteLine("exception: " + e);
			Console.Error.WriteLine(e.StackTrace); // so we can get stack trace
		}
/*
		finally
		{
			Console.ReadLine();
		}
*/		
	}
	
	
	// This method decides what action to take based on the type of
	//   file we are looking at
	public static void  doFile(FileInfo f)
	{
		// If this is a directory, walk each file/dir in that directory
		if (Directory.Exists(f.FullName))
		{
			string[] files = Directory.GetFileSystemEntries(f.FullName);
			 for (int i = 0; i < files.Length; i++)
				doFile(new FileInfo(f.FullName + "\\" + files[i]));
		}
		else if ((f.Name.Length > 5) && f.Name.Substring(f.Name.Length - 5).Equals(".java"))
		{
			Console.Error.WriteLine("   " + f.FullName);
			parseFile(f.Name, new FileStream(f.FullName, FileMode.Open, FileAccess.Read));
		}
	}
	
	// Here's where we do the real work...
	public static void  parseFile(string f, Stream s)
	{
		try
		{
			// Create a scanner that reads from the input stream passed to us
			JavaLexer lexer = new JavaLexer(new StreamReader(s));
			lexer.setFilename(f);
			
			// Create a parser that reads from the scanner
			JavaRecognizer parser = new JavaRecognizer(lexer);
			parser.setFilename(f);
			
			// start parsing at the compilationUnit rule
			parser.compilationUnit();
			
			// do something with the tree
			doTreeAction(f, parser.getAST(), parser.getTokenNames());
		}
		catch (System.Exception e)
		{
			Console.Error.WriteLine("parser exception: " + e);
			Console.Error.WriteLine(e.StackTrace); // so we can get stack trace		
		}
	}
	
	public static void  doTreeAction(string f, AST t, string[] tokenNames)
	{
		if (t == null)
			return ;
		if (showTree)
		{
			BaseAST.setVerboseStringConversion(true, tokenNames);
			ASTFactory factory = new ASTFactory();
			AST r = factory.create(0, "AST ROOT");
			r.setFirstChild(t);
			ASTFrame frame = new ASTFrame("Java AST", r);
			frame.ShowDialog();
			//frame.Visible = true;
			// System.out.println(t.toStringList());
		}
		JavaTreeParser tparse = new JavaTreeParser();
		try
		{
			tparse.compilationUnit(t);
			// System.out.println("successful walk of result AST for "+f);
		}
		catch (RecognitionException e)
		{
			Console.Error.WriteLine(e.Message);
			Console.Error.WriteLine(e.StackTrace);
		}
		
	}
}