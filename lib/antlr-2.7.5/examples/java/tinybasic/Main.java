package tinybasic;

import java.io.*;
import antlr.collections.AST;
import antlr.collections.impl.*;
import antlr.debug.misc.*;
import antlr.*;

class Main {

    Context theContext=new Context();
    static boolean showTree = false;
    
    Main(String f){
	try {
	    doFile(new File(f));
	}
	catch(Exception e) {
		System.err.println("exception: "+e);
		e.printStackTrace(System.err);   // so we can get stack trace
	}
    }
    
    public static void main(String[] args) {
		// Use a try/catch block for parser exceptions
		//try {
			// if we have at least one command-line argument
			if (args.length > 0 ) {
				System.err.println("Parsing...");

				// for each directory/file specified on the command line
				for(int i=0; i< args.length;i++) {
					if ( args[i].equals("-showtree") ) {
						showTree = true;
					}
					else {
						new Main(args[i]); // parse it
					}
				} }
			else
				System.err.println("Usage: java TinyBasicParser [-showtree] "+
                                   "<directory or file name>");
		//}
		//catch(Exception e) {
		//	System.err.println("exception: "+e);
		//	e.printStackTrace(System.err);   // so we can get stack trace
		//}
	}


	// This method decides what action to take based on the type of
	//   file we are looking at
	public void doFile(File f)
							  throws Exception {
		// If this is a directory, walk each file/dir in that directory
		if (f.isDirectory()) {
			String files[] = f.list();
			for(int i=0; i < files.length; i++)
				doFile(new File(f, files[i]));
		}

		// otherwise, if this is a java file, parse it!
		else if ((f.getName().length()>4) &&
				f.getName().substring(f.getName().length()-4).equals(".bas")) {
			System.err.println("   "+f.getAbsolutePath());
			parseFile(f.getName(), new FileInputStream(f));
		}
	}

	// Here's where we do the real work...
	public void parseFile(String f, InputStream s)
								 throws Exception {
		try {
			// Create a scanner that reads from the input stream passed to us
			TinyBasicLexer lexer = new TinyBasicLexer(s);

			// Create a parser that reads from the scanner
			TinyBasicParser parser = new TinyBasicParser(lexer);

			// start parsing at the compilationUnit rule
			parser.compilationUnit(theContext);
			
			// do something with the tree
			doTreeAction(f, parser.getAST(), parser.getTokenNames());
			//System.out.println(parser.getAST().toStringList());
		}
		catch (Exception e) {
			System.err.println("parser exception: "+e);
			e.printStackTrace();   // so we can get stack trace		
		}
	}
	
	public void doTreeAction(String f, AST t, String[] tokenNames) {
		if ( t==null ) return;
		if ( showTree ) {
			((CommonAST)t).setVerboseStringConversion(true, tokenNames);
			ASTFactory factory = new ASTFactory();
			AST r = factory.create(0,"AST ROOT");
			r.setFirstChild(t);
			ASTFrame frame = new ASTFrame("TinyBasic AST", r);
			frame.setVisible(true);
			//System.out.println(t.toStringList());
		}
		TinyBasicTreeWalker tparse = new TinyBasicTreeWalker();
		try {
			tparse.compilationUnit(t,theContext);
			//System.out.println("successful walk of result AST for "+f);
		}
		catch (ANTLRException e) {
			System.err.println(e.getMessage());
			e.printStackTrace();
		}

	}
}
