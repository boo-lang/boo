class L extends Lexer;

options {
	// Allow any char but \uFFFF (16 bit -1)
	charVocabulary='\u0000'..'\uFFFE';
}

{
	private static boolean done = false;

    public void uponEOF() throws TokenStreamException, CharStreamException {
		done=true;
    }
	
	public static void main(String[] args) throws Exception {
		L lexer = new L(System.in);
		while ( !done ) {
			Token t = lexer.nextToken();
			System.out.println("Token: "+t);
		}
	}
}

ID	:	ID_START_LETTER ( ID_LETTER )*
	;

WS	:	(' '|'\n') {$setType(Token.SKIP);}
	;

protected
ID_START_LETTER
	:	'$'
	|	'_'
	|	'a'..'z'
	|	'\u0080'..'\ufffe'
	;

protected
ID_LETTER
	:	ID_START_LETTER
	|	'0'..'9'
	;
