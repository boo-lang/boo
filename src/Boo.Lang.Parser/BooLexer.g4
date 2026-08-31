// Copyright (c) the Boo contributors
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

lexer grammar BooLexer;

tokens {
	INDENT,
	DEDENT,
	// expression list
	ELIST,
	// declaration list
	DLIST,
	// expression separator (imaginary token)
	ESEPARATOR,
	EOL
}

ABSTRACT	:	'abstract';
AND			:	'and';
AS			:	'as';
BREAK		:	'break';
CONTINUE	:	'continue';
CALLABLE	:	'callable';
CAST		:	'cast';
CHAR		:	'char';
CLASS		:	'class';
CONSTRUCTOR	:	'constructor';
DEF			:	'def';
DESTRUCTOR	:	'destructor';
DO			:	'do';
ELIF		:	'elif';
ELSE		:	'else';
END			:	'end';
ENSURE		:	'ensure';
ENUM		:	'enum';
EVENT		:	'event';
EXCEPT		:	'except';
FAILURE		:	'failure';
FINAL		:	'final';
FROM		:	'from';
FOR			:	'for';
FALSE		:	'false';
GET			:	'get';
GOTO		:	'goto';
IMPORT		:	'import';
INTERFACE	:	'interface';
INTERNAL	:	'internal';
IS			:	'is';
ISA			:	'isa';
IF			:	'if';
IN			:	'in';
NAMESPACE	:	'namespace';
NEW			:	'new';
NOT			:	'not';
NULL		:	'null';
OF			:	'of';
OR			:	'or';
OVERRIDE	:	'override';
PASS		:	'pass';
PARTIAL		:	'partial';
PUBLIC		:	'public';
PROTECTED	:	'protected';
PRIVATE		:	'private';
RAISE		:	'raise';
REF			:	'ref';
RETURN		:	'return';
SET			:	'set';
SELF		:	'self';
SUPER		:	'super';
STATIC		:	'static';
STRUCT		:	'struct';
THEN		:	'then';
TRY			:	'try';
TRANSIENT	:	'transient';
TRUE		:	'true';
TYPEOF		:	'typeof';
UNLESS		:	'unless';
VIRTUAL		:	'virtual';
WHILE		:	'while';
YIELD		:	'yield';

ID
	:	AT_SYMBOL ID_SUFFIX?
	|	ID_SUFFIX
	;

fragment
ID_SUFFIX
	:	ID_LETTER
		(	ID_LETTER
		|	DIGIT
		)*
	;

LINE_CONTINUATION
	:	'\\'
		(	NEWLINE
		|	[ \t]
		|	SL_COMMENT
		|	ML_COMMENT
		)+
		-> channel(HIDDEN)
	;

INT
	:	DIGIT_GROUP
		(	[eE] [+-]? DIGIT_GROUP
		)?
	;

HEX_INT
	:	'0x' HEXDIGIT+
		-> type(INT)
	;

LONG
	:	HEX_INT [lL]
	|	INT	[lL]
	;

FLOAT
	:	(	INT
		|	DOUBLE
		)
		[fF]
	;

DOUBLE
	:	DIGIT_GROUP
		(	[eE] [+-]? DIGIT_GROUP
		)?
		'.' REVERSE_DIGIT_GROUP
		(	[eE] [+-]? DIGIT_GROUP
		)?
	|	'.' REVERSE_DIGIT_GROUP
		(	[eE] [+-]? DIGIT_GROUP
		)?
	;

TIMESPAN
	:	(	INT
		|	DOUBLE
		)
		(	'ms'
		|	's'
		|	'm'
		|	'h'
		|	'd'
		)
	;

DOT
	:	'.'
	;

COLON : ':';

BITWISE_OR: '|';
INPLACE_BITWISE_OR: '|=';

BITWISE_AND: '&';
INPLACE_BITWISE_AND: '&=';

EXCLUSIVE_OR: '^';
INPLACE_EXCLUSIVE_OR: '^=';

LPAREN : '(' { EnterSkipWhitespaceRegion(); HandleInterpolationToken(LPAREN); };

RPAREN : ')' { LeaveSkipWhitespaceRegion(); HandleInterpolationToken(RPAREN); };

LBRACK
	:	'[' { EnterSkipWhitespaceRegion(); }
	;

MODULE_ATTRIBUTE_BEGIN
	:	'[' { EnterSkipWhitespaceRegion(); } 'module:'
	;

ASSEMBLY_ATTRIBUTE_BEGIN
	:	'[' { EnterSkipWhitespaceRegion(); } 'assembly:'
	;

RBRACK : ']' { LeaveSkipWhitespaceRegion(); };

LBRACE : '{' { EnterSkipWhitespaceRegion(); HandleInterpolationToken(LBRACE); };
	
RBRACE : '}' { LeaveSkipWhitespaceRegion(); HandleInterpolationToken(RBRACE); };

SPLICE_BEGIN : '$';

QQ_BEGIN: '[|';

QQ_END: '|]';

INCREMENT: '++';

DECREMENT: '--';

ADD: '+';

SUBTRACT: '-';

MODULUS: '%';

MULTIPLY: '*';

ASSIGN: [+\-%*/]? '=';

EXPONENTIATION: '**';

DIVISION: '/';

LESS_THAN: '<';

SHIFT_LEFT: '<<';

INPLACE_SHIFT_LEFT: '<<=';

GREATER_THAN: '>';

SHIFT_RIGHT: '>>';

INPLACE_SHIFT_RIGHT: '>>=';

ONES_COMPLEMENT: '~';

CMP_OPERATOR
	:	'<='
	|	'>='
	|	'!~'
	|	'!='
	|	'=='
	|	'=~'
	;

COMMA: ',';

TRIPLE_QUOTED_STRING
	:	'"""'
		-> pushMode(TQS)
	;

DOUBLE_QUOTED_STRING
	:	'"'
		-> pushMode(DQS)
	;

SINGLE_QUOTED_STRING
	:	'\''
		(	SQS_ESC
		|	~['\\\r\n]
		)*
		'\''
	;

BACKTICK_QUOTED_STRING
	:	'`'
		(	~[`\r\n]
		|	NEWLINE
		)*
		'`'
	;

SL_COMMENT
	:	(	'#' ~[\r\n]*
		|	'//' ~[\r\n]*
		)
		-> channel(HIDDEN)
	;

ML_COMMENT
	:	'/*'
		(	'*' {InputStream.LA(1) != '/'}?
		|	ML_COMMENT
		|	NEWLINE
		|	~[*\r\n]
		)*
		'*/'
		-> channel(HIDDEN)
	;

WS
	:	(	[ \t\f]
		)+
		{
			if (SkipWhitespace)
				Channel = Hidden;
		}
	;

EOS: ';';

X_RE_LITERAL
	:	'@/' X_RE_CHAR+ '/'
		-> type(RE_LITERAL)
	;

NEWLINE
	:	(	'\n'
		|	'\r' '\n'?
		)
		{
			// ANTLR 4 counts a line on '\n' alone, so a bare carriage return ends
			// a line for boo.g but not for the runtime.
			if (Text == "\r")
			{
				Interpreter.Line++;
				Interpreter.Column = 0;
			}
			if (SkipWhitespace)
				Channel = Hidden;
		}
	;

fragment
SQS_ESC
	:	'\\'
		(	SESC
		|	'\''
		)
	;

// Escape sequences are recognised here but decoded in the AST builder, by
// UnescapeCharacter. ANTLR 4 never runs actions inside a fragment rule.
fragment
SESC
	:	'r'
	|	'n'
	|	't'
	|	'a'
	|	'b'
	|	'f'
	|	'0'
	|	'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT
	|	'\\'
	;

RE_LITERAL
	:	'/' RE_CHAR+ '/' RE_OPTIONS?
	;

fragment
RE_CHAR
	:	RE_ESC
	|	~[ /\\\r\n\t]
	;

fragment
X_RE_CHAR
	:	RE_CHAR
	|	[ \t]
	;

fragment
RE_OPTIONS
	:	ID_LETTER+
	;

fragment
RE_ESC
	:	'\\'
		(	'+'
		|	'/'
		|	'('
		|	')'
		|	'|'
		|	'.'
		|	'*'
		|	'?'
		|	'$'
		|	'^'
		|	'['
		|	']'
		|	'{'
		|	'}'

		// character scapes
		// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterescapes.htm

		|	'a'
		|	'b'
		|	('c' 'A'..'Z')
		|	't'
		|	'r'
		|	'v'
		|	'f'
		|	'n'
		|	'e'
		|	(DIGIT)+
		|	('x' HEXDIGIT HEXDIGIT)
		|	('u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT)
		|	'\\'

		// character classes
		// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterclasses.htm
		// /\w\W\s\S\d\D/

		|	'w'
		|	'W'
		|	's'
		|	'S'
		|	'd'
		|	'D'
		|	'p'
		|	'P'

		// atomic zero-width assertions
		// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconatomiczero-widthassertions.htm

		|	'A'
		|	'z'
		|	'Z'
		|	'g'
		|	'B'
		|	'k'
		)
	;

fragment
DIGIT_GROUP
	:	DIGIT
		(	'_' DIGIT DIGIT DIGIT
		|	DIGIT
		)*
	;

fragment
REVERSE_DIGIT_GROUP
	:	(	DIGIT DIGIT DIGIT ('_' {IsDigit(InputStream.LA(1))}?)?
		|	DIGIT
		)+
	;

fragment
AT_SYMBOL
	:	'@'
	;

fragment
ID_LETTER
	:	[_a-zA-Z]
	|	[\u0080-\uFFFE] {char.IsLetter((char)InputStream.LA(-1))}?
	;

fragment
DIGIT
	:	[0-9]
	;

fragment
HEXDIGIT
	:	[a-fA-F0-9]
	;

NULLABLE_SUFFIX
	:	'?'
	;

mode TQS;

	TQS_TEXT
		:	(	~["\\$]
			|	'\\' .
			)+
			-> type(TEXT)
		;

	TQS_INTERPOLATED_REFERENCE
		:	'$' ID -> type(INTERPOLATED_REFERENCE)
		;

	TQS_INTERPOLATED_EXPRESSION_LBRACE
		:	'${' {EnterSkipWhitespaceRegion(); HandleInterpolatedExpression(LBRACE, RBRACE);} -> type(INTERPOLATED_EXPRESSION_LBRACE)
		;

	TQS_INTERPOLATED_EXPRESSION_LPAREN
		:	'$(' {EnterSkipWhitespaceRegion(); HandleInterpolatedExpression(LPAREN, RPAREN);} -> type(INTERPOLATED_EXPRESSION_LPAREN)
		;

	TQS_STRAY_DOLLAR
		:	'$' -> type(TEXT)
		;

	TQS_DQUOTE
		:	'"' -> type(TEXT)
		;

	TQS_END
		:	'"""'
			-> popMode
		;

mode DQS;

	DQS_ESC
		:	'\\'
			(	SESC
			|	'"'
			|	'$'
			)
		;

	TEXT
		:	~["\\\r\n$]+
		;

	INTERPOLATED_REFERENCE
		:	'$' ID
		;

	INTERPOLATED_EXPRESSION_LBRACE
		:	'${' {EnterSkipWhitespaceRegion(); HandleInterpolatedExpression(LBRACE, RBRACE);}
		;

	INTERPOLATED_EXPRESSION_LPAREN
		:	'$(' {EnterSkipWhitespaceRegion(); HandleInterpolatedExpression(LPAREN, RPAREN);}
		;

	STRAY_DOLLAR
		:	'$' -> type(TEXT)
		;

	DQS_END
		:	'"'
			-> popMode
		;
