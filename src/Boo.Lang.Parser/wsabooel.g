// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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
//     * Neither the name of the Rodrigo B. de Oliveira nor the names of its
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

options
{
	language="CSharp";
	namespace = "Boo.Lang.Parser";
}
class WSABooExpressionLexer extends Lexer;
options
{
	defaultErrorHandler = false;
	testLiterals = false;
	importVocab = WSABoo;	
	k = 3;
	charVocabulary='\u0003'..'\uFFFE';
	// without inlining some bitset tests, ANTLR couldn't do unicode;
	// They need to make ANTLR generate smaller bitsets;
	codeGenBitsetTestThreshold=20;	
	classHeaderPrefix="public";
}
{
	
	public override void uponEOF()
	{
		Error();
	}

	void Error()
	{		
		throw new SemanticException("Unterminated expression interpolation!", getFilename(), getLine(), getColumn());
	}
}
ID options { testLiterals = true; }:
	(ID_PREFIX)? ID_LETTER (ID_LETTER | DIGIT)*
	;

INT : 
  	("0x"(HEXDIGIT)+)(('l' | 'L') { $setType(LONG); })? |
  	DIGIT_GROUP
 	(('e'|'E')('+'|'-')? DIGIT_GROUP)?
  	(
  		('l' | 'L') { $setType(LONG); } |
		(('f' | 'F') { $setType(FLOAT); }) |
  		(
 			(
 				{WSABooLexer.IsDigit(LA(2))}? 
 				(
 					'.' REVERSE_DIGIT_GROUP
 					(('e'|'E')('+'|'-')? DIGIT_GROUP)?
 				)
				(
					(('f' | 'F') { $setType(FLOAT); }) |
					{ $setType(DOUBLE); }
				)
 			)?
  			(("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); })?
  		)
  	)
;
  
DOT : '.' 
	(
		REVERSE_DIGIT_GROUP (('e'|'E')('+'|'-')? DIGIT_GROUP)?
		(
			(('f' | 'F')  { $setType(FLOAT); }) |
			(("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); }) |
			{$setType(DOUBLE);}
		)
	)?
;

COLON : ':';

COMMA : ',';

BITWISE_OR: '|';

BITWISE_AND: '&';

EXCLUSIVE_OR: '^' ('=' { $setType(ASSIGN); })?;

LPAREN : '(';
	
RPAREN : ')';

LBRACK : '[';

RBRACK : ']';

LBRACE : '{';
	
RBRACE : '}';

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


LESS_THAN: '<';

SHIFT_LEFT: "<<";

INPLACE_SHIFT_LEFT: "<<=";

GREATER_THAN: '>';

SHIFT_RIGHT: ">>";

INPLACE_SHIFT_RIGHT: ">>=";

ONES_COMPLEMENT: '~';

CMP_OPERATOR :  "<=" | ">=" | "!~" | "!=";

ASSIGN : '=' ( ('=' | '~') { $setType(CMP_OPERATOR); } )?;

WS:
	(
		' '
		| '\t' { tab(); }
		|
		(
			(('\r' ('\n')?)
			| '\n')
			{ newline(); }
		)
	)+
	{ $setType(Token.SKIP); }
;

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
SESC: 
				( 'r'! {$setText("\r"); }) |
				( 'n'! {$setText("\n"); }) |
				( 't'! {$setText("\t"); }) |
				( 'a'! {text.Length = _begin; text.Append("\a"); }) |
				( 'b'! {text.Length = _begin; text.Append("\b"); }) |
				( 'f'! {text.Length = _begin; text.Append("\f"); }) |
				( '0'! {text.Length = _begin; text.Append("\0"); }) |
				( 'u'!
					HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT
					{
						char ch = (char)int.Parse(text.ToString(_begin, 4), System.Globalization.NumberStyles.HexNumber);
						text.Length = _begin;
						text.Append(ch);
					}
				) |
				( '\\'! {$setText("\\"); });

protected
RE_LITERAL : '/' (RE_CHAR)+ '/';

protected
RE_CHAR : RE_ESC | ~('/' | '\\' | '\r' | '\n' | ' ' | '\t');

protected
RE_ESC : '\\' (				
				'+' |
				'/' |
				'(' |
				')' |
				'|' |
				'.' |
				'*' |
				'?' |
				'$' |
				'^' |
				'['	|
				']' |
				'{' |
				'}' |
	
	// character scapes
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterescapes.htm
	
				'a' |
				'b' |
				('c' 'A'..'Z') |
				't' |
				'r' |
				'v' |
				'f' |
				'n' |
				'e' |
				(DIGIT)+ |
				('x' HEXDIGIT HEXDIGIT) |
				('u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT) |
				'\\' |
				
	// character classes
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterclasses.htm
	// /\w\W\s\S\d\D/
	
				'w' |
				'W' |
				's' |
				'S' |
				'd' |
				'D' |
				'p' |
				'P' |
				
	// atomic zero-width assertions
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconatomiczero-widthassertions.htm
				'A' |
				'z' |
				'Z' |
				'g' |
				'B' |
				'k'			
			 )
			 ;

protected
DIGIT_GROUP : DIGIT (('_'! DIGIT DIGIT DIGIT) | DIGIT)*;

protected
REVERSE_DIGIT_GROUP : (DIGIT DIGIT DIGIT ({WSABooLexer.IsDigit(LA(2))}? '_'!)? | DIGIT)+;

protected
ID_PREFIX : '@' | '?';

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' | {System.Char.IsLetter(LA(1))}? '\u0080'..'\uFFFE');

protected
DIGIT : '0'..'9';

protected
HEXDIGIT : ('a'..'f' | 'A'..'F' | '0'..'9');

NULLABLE_SUFFIX: '?';

