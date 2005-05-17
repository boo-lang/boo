class DataParser extends Parser;

file:	(	sh:SHORT	{System.out.println(sh.getText());}
		|	st:STRING	{System.out.println("\""+st.getText()+"\"");}
		)+
	;

class DataLexer extends Lexer;
options {
	charVocabulary = '\u0000'..'\u00FF';
}

SHORT
	:	'\0' high:. lo:.
		{
		int v = (((int)high)<<8) + lo;
		$setText(""+v);
		}
	;

STRING
	:	'\1'!	// begin string (discard)
		( ~'\2' )*
		'\2'!	// end string (discard)
	;
