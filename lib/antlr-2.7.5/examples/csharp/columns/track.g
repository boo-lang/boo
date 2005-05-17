options
{
	language = "CSharp";
}

class L extends Lexer;

{
private static bool done = false;

public override void uponEOF() //throws TokenStreamException, CharStreamException 
{
	done=true;
}

/** set tabs to 4, just round column up to next tab + 1
12345678901234567890
    x   x   x   x
 */
public override void tab() {
	int t = 4;
	int c = getColumn();
	int nc = (((c-1)/t)+1)*t+1;
	setColumn( nc );
}

public static void Main(string[] args) //throws Exception 
{
	L lexer = new L(new CharBuffer(Console.In));
	while ( !done ) {
		IToken t = lexer.nextToken();
		Console.Out.WriteLine("Token: "+t);
	}
}
}

INT : ('0'..'9')+ ;

ID : ('a'..'z')+ ;

WS	:	(	' '
		|	'\t'
		// handle newlines
		|	(	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ newline(); }
		)+
		{ $setType(Token.SKIP); }
	;
