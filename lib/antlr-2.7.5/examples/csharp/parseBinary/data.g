options
{
	language = "CSharp";
}

class DataParser extends Parser;

file:	(	sh:SHORT	{Console.Out.WriteLine(sh.getText());}
		|	st:STRING	{Console.Out.WriteLine(@""""+st.getText()+@"""");}
		)+
	;

class DataLexer extends Lexer;
options {
	charVocabulary = '\u0000'..'\u00FF';
}

SHORT
	:	'\u0000' high:. lo:.
		{
		int v = (((int)high)<<8) + lo;
		$setText(""+v);
		}
	;

STRING
	:	'\u0001'!	// begin string (discard)
		( ~'\u0002' )*
		'\u0002'!	// end string (discard)
	;
