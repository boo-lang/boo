header {
	using System.IO;
}

options {
    language="CSharp";
}

class PParser extends Parser;
{
	public override void traceOut(string rname) {
		Console.Out.WriteLine("exit "+rname+"; LT(1)="+LT(1));
	}
	public override void traceIn(string rname) {
		Console.Out.WriteLine("enter "+rname+"; LT(1)="+LT(1));
	}
/*
	public override void consume() {
		try {
			Console.Out.WriteLine(LT(1));
		}
		catch (IOException ignore) {}
		base.consume();
	}
*/
}

startRule
	:	( decl )+
	;

decl:	INT a:ID 		{ Console.Out.WriteLine("decl "+a.getText()); }
		( COMMA b:ID 	{ Console.Out.WriteLine("decl "+b.getText()); } )*
		SEMI
	;

class PLexer extends Lexer;
options {
	charVocabulary = '\3'..'\377';
	k=2;
}

tokens {
	INT="int";
}

{
	public override void uponEOF() {
		if ( Test.AppMain.selector.getCurrentStream() != Test.AppMain.mainLexer ) {
			// don't allow EOF until main lexer.  Force the
			// selector to retry for another token.
			Test.AppMain.selector.pop(); // return to old lexer/stream
			Test.AppMain.selector.retry();
		}
		else {
			Console.Out.WriteLine("Hit EOF of main file");
		}
	}
}

SEMI:	';'
	;

COMMA
	:	','
	;

ID
	:	('a'..'z')+
	;

INCLUDE
	:	"#include" (WS)? f:STRING
		{
			// create lexer to handle include
			String name = f.getText();
			StreamReader fi = null;
			try {
				fi = new StreamReader(name);
			}
			catch (FileNotFoundException fnf) {
				Console.Error.WriteLine("cannot find file "+name);
			}
			PLexer sublexer = new PLexer(fi);
			// make sure errors are reported in right file
			sublexer.setFilename(name);
			Test.AppMain.parser.setFilename(name);
	
			// you can't just call nextToken of sublexer
			// because you need a stream of tokens to
			// head to the parser.  The only way is
			// to blast out of this lexer and reenter
			// the nextToken of the sublexer instance
			// of this class.
	
			Test.AppMain.selector.push(sublexer);
			// ignore this as whitespace; ask selector to try
			// to get another token.  It will call nextToken()
			// of the new instance of this lexer.
			Test.AppMain.selector.retry(); // throws TokenStreamRetryException
		}
	;

STRING
	:	'"'! ( ~'"' )* '"'!
	;

WS	:	(	' '
		|	'\t'
		|	'\f'
			// handle newlines
		|	(	options {generateAmbigWarnings=false;}
			:	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ newline(); }
		)+
		{ $setType(Token.SKIP); }
	;
