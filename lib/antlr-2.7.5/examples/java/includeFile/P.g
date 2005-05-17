class PParser extends Parser;
{
	public void traceOut(String rname) throws TokenStreamException  {
		System.out.println("exit "+rname+"; LT(1)="+LT(1));
	}
	public void traceIn(String rname) throws TokenStreamException  {
		System.out.println("enter "+rname+"; LT(1)="+LT(1));
	}
/*
	public void consume() throws IOException {
		try {
			System.out.println(LT(1));
		}
		catch (IOException ignore) {}
		super.consume();
	}
*/
}

startRule
	:	( decl )+
	;

decl:	INT a:ID {System.out.println("decl "+a.getText());}
		( COMMA b:ID {System.out.println("decl "+b.getText());} )*
		SEMI
	;

{
import java.io.*;
}

class PLexer extends Lexer;
options {
	charVocabulary = '\3'..'\377';
	k=2;
}

tokens {
	INT="int";
}

{
	public void uponEOF() throws TokenStreamException, CharStreamException {
		if ( Main.selector.getCurrentStream() != Main.mainLexer ) {
			// don't allow EOF until main lexer.  Force the
			// selector to retry for another token.
			Main.selector.pop(); // return to old lexer/stream
			Main.selector.retry();
		}
		else {
			System.out.println("Hit EOF of main file");
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
		DataInputStream input=null;
		try {
			FileInputStream fi = new FileInputStream(name);
		    input = new DataInputStream(fi);
		}
		catch (FileNotFoundException fnf) {
			System.err.println("cannot find file "+name);
		}
		PLexer sublexer = new PLexer(input);
		// make sure errors are reported in right file
		sublexer.setFilename(name);
		Main.parser.setFilename(name);

		// you can't just call nextToken of sublexer
		// because you need a stream of tokens to
		// head to the parser.  The only way is
		// to blast out of this lexer and reenter
		// the nextToken of the sublexer instance
		// of this class.

		Main.selector.push(sublexer);
		// ignore this as whitespace; ask selector to try
		// to get another token.  It will call nextToken()
		// of the new instance of this lexer.
		Main.selector.retry(); // throws TokenStreamRetryException
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
