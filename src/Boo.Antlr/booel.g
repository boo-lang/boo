options
{
	language="CSharp";
	namespace = "Boo.Antlr";
}
class BooExpressionLexer extends Lexer;
options
{
	defaultErrorHandler = false;
	testLiterals = false;
	importVocab = Boo;	
	k = 2;
	charVocabulary='\u0003'..'\uFFFF';
	// without inlining some bitset tests, ANTLR couldn't do unicode;
	// They need to make ANTLR generate smaller bitsets;
	codeGenBitsetTestThreshold=20;	
	classHeaderPrefix="internal";
}
{
	
	public override void uponEOF()
	{
		Error();
	}

	void Error()
	{		
		throw new SemanticException("Unterminated formatting expression!", getFilename(), getLine(), getColumn());
	}
}
ID options { testLiterals = true; }: ID_LETTER (ID_LETTER | DIGIT)*;

DOT : '.';

INT : (DIGIT)+;

COMMA : ',';

COLON : ':';

QMARK : '?';

LPAREN : '(';

RPAREN : ')';

LBRACK : '[';

RBRACK : ']';

INCREMENT: "++";

DECREMENT: "--";

SUM_OPERATOR : '+' | '-' ;

MULT_OPERATOR: '%' | '/' | '*';

CMP_OPERATOR : '<' | "<=" | '>' | ">=" | "!~" | "!=";

ASSIGN : '=' ( ('=' | '~') { $setType(CMP_OPERATOR); } )?;

WS: (' ' | '\t' | '\r' | '\n' { newline(); })+ { $setType(Token.SKIP); };

RBRACE: '}';

SINGLE_QUOTED_STRING :
		'\''!
		(
			SQS_ESC |
			~('\'' | '\\' | '\r' | '\n')
		)*
		'\''!
		;

protected
SQS_ESC : '\\'! (
					('\'') |
					('r' {$setText("\r");}) |
					('n' {$setText("\n");}) |
					('t' {$setText("\t");}) |
					('\\')
				);

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';
