options {
	language = "Boo";
	namespace = "Boo.Lang.Useful.IO.Impl";
}

class PreProcessorExpressionParser extends Parser;
options {
	buildAST = true;
}

expr
	:	mexpr (OR^ mexpr)*
	;

mexpr
	:	atom (AND^ atom)*
	;

atom:	ID | NOT^ ID | paren_expr
	;
	
paren_expr: LPAREN! expr RPAREN!
	;

class PreProcessorExpressionLexer extends Lexer;

WS	:	(' '
	|	'\t'
	|	'\n'
	|	'\r')
		{ _ttype = Token.SKIP; }
	;

AND:	"&&"
	;

OR:	"||"
	;

NOT: '!'
	;
	
LPAREN: '('
	;
	
RPAREN: ')'
	;
	
ID: ID_START (ID_PART)*
	;
	
COMMENT: "//" (~('\r'|'\n'))* { $setType(Token.SKIP); }
	;


protected
ID_START: '_' | LETTER
	;
	
protected
ID_PART: ID_START | DIGIT
	;
	
protected
LETTER: 'a'..'z' | 'A'..'Z'
	;
	
protected
DIGIT
	:	'0'..'9'
	;

class PreProcessorExpressionEvaluator extends TreeParser;
{
	[property(SymbolTable)]
	_symbolTable as System.Collections.IDictionary
}
expr returns [bool value]
{
a as bool
b as bool
value = false
}:
	#(OR a=expr b=expr)	{ value = a or b; }
	| #(AND a=expr b=expr) { value = a and b; }
	| #(NOT a=expr) { value = not a; }
	| id:ID { value = id.getText() in _symbolTable; }
;

