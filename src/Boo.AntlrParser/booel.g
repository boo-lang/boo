// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
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
	k = 3;
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
ID options { testLiterals = true; }:
	ID_LETTER (ID_LETTER | DIGIT)*
	;

INT : (DIGIT)+
	(
		('l' | 'L') { $setType(LONG); } |
		(
	({BooLexer.IsDigit(LA(2))}? ('.' (DIGIT)+) { $setType(DOUBLE); })?
	(("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); })?
		)
	)
	;

DOT : '.' ((DIGIT)+ {$setType(DOUBLE);})?;

COLON : ':';

COMMA : ',';

BITWISE_OR: '|';

LPAREN : '(';
	
RPAREN : ')';

LBRACK : '[';

RBRACK : ']';

LBRACE : '{';
	
RBRACE : '}';

QMARK : '?';

INCREMENT: "++";

DECREMENT: "--";

ADD: ('+') ('=' { $setType(ASSIGN); })?;

SUBTRACT: ('-') ('=' { $setType(ASSIGN); })?;

MODULUS: '%';

MULTIPLY: '*' (
					'=' { $setType(ASSIGN); } |
					'*' { $setType(EXPONENTIATION); } | 
				);

DIVISION: 
	(RE_LITERAL)=> RE_LITERAL { $setType(RE_LITERAL); } |
	'/' ('=' { $setType(ASSIGN); })?
	;


CMP_OPERATOR : '<' | "<=" | '>' | ">=" | "!~" | "!=";

ASSIGN : '=' ( ('=' | '~') { $setType(CMP_OPERATOR); } )?;

WS: (' ' | '\t' { tab(); } | '\r' | '\n' { newline(); })+ { $setType(Token.SKIP); };

SINGLE_QUOTED_STRING :
		'\''!
		(
			SQS_ESC |
			~('\'' | '\\' | '\r' | '\n')
		)*
		'\''!
		;

protected
DQS_ESC : '\\'! ( SESC | '"' | '$') ;	
	
protected
SQS_ESC : '\\'! ( SESC | '\'' );

protected
SESC : 
				( 'r' {$setText("\r"); }) |
				( 'n' {$setText("\n"); }) |
				( 't' {$setText("\t"); }) |
				( '\\' );

protected
RE_LITERAL : '/' (RE_CHAR)+ '/';

protected
RE_CHAR : RE_ESC | ~('/' | '\\' | ' ' | '\t' | '\r' | '\n');

protected
RE_ESC : '\\' ('\\' | '/' | 'r' | 'n' | 't' | '(' | ')' | '.' | '*' | '?' | '[' | ']');

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';
