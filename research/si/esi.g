options
{
	language="CSharp";
}
class ExpressionLexer extends Lexer;
options
{
	importVocab = SI;
	defaultErrorHandler=false;
}
{
	
	public override void uponEOF()
	{
		Error();
	}

	void Error()
	{		
		throw new SemanticException("Invalid formatting expression!", getFilename(), getLine(), getColumn());
	}
}
ID: ID_LETTER (ID_LETTER | DIGIT)*;

INT: (DIGIT)+;

ASSIGN: '=';

SUM_OPERATOR : '+' | '-' ;

MULT_OPERATOR: '/' | '*';

WS: (' ' | '\t' | '\r' | '\n' { newline(); })+ { $setType(Token.SKIP); };

RBRACE: '}';

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';
