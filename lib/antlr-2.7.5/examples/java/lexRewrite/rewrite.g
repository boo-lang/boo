class Rewrite extends Lexer;

protected
START
options {
	ignore=WS;
}
	:	id:ID ":="! '('! expr:EXPR ')'!
		{
			// can access text matched for any rule
			System.out.println("found "+id.getText()+","+expr.getText());
			// text will be ID+EXPR minus whitespace
		}
	;

protected
ID	:	( let:LETTER {System.out.println("letter "+let.getText());} )+
	;

protected
LETTER
	:	'a'..'z'
		{
		String s = $getText;		// get access text of this rule
		$setText(s.toUpperCase());	// can reset it too
		}
	;

protected
EXPR:	i:INT!	// don't include, but i.getText() has access
		{$setText(i.getText());} // effect is if no "!" and no "i:"
	|	ID
	;

protected
INT	:	('0'..'9')+
	;

// what if ! on rule itself and invoker had !...should
// rule return anything in the token to the invoker?  NO!
// make sure 'if' is in the right spot
// What about no ! on caller but ! on called rule?
protected
WS!	:	(	' '			// whitespace not saved
		|	'\t'
		|	'\n' {newline();}
		)+
		{$setType(Token.SKIP);}		// way to set token type
	;


