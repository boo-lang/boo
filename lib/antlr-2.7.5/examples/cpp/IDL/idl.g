options {
	language="Cpp";
}

/**
 *  This is a complete parser for the IDL language as defined
 *  by the CORBA 2.0 specification.  It will allow those who
 *  need an IDL parser to get up-and-running very quickly.
 *  Though IDL's syntax is very similar to C++, it is also
 *  much simpler, due in large part to the fact that it is
 *  a declarative-only language.
 *
 *  Some things that are not included are: Symbol table construction
 *  (it is not necessary for parsing, btw) and preprocessing (for
 *  IDL compiler #pragma directives). You can use just about any
 *  C or C++ preprocessor, but there is an interesting semantic
 *  issue if you are going to generate code: In C, #include is
 *  a literal include, in IDL, #include is more like Java's import:
 *  It adds definitions to the scope of the parse, but included
 *  definitions are not generated.
 *
 *  Jim Coker, jcoker@magelang.com
 */
class IDLParser extends Parser;
options {
	exportVocab=IDL;
}

specification
	:   (definition)+
  	;

definition
	:   (   type_dcl SEMI!
	    |   const_dcl SEMI!
 	    |   except_dcl SEMI!
	    |   interf SEMI!
	    |   module SEMI!
	    )
	;

module
	:    "module"
	     identifier
 	     LCURLY d:definition_list RCURLY
	;

definition_list
	:   (definition)+
	;

interf
	:   "interface"
	    identifier
	    inheritance_spec
 	    (interface_body)?
	;

interface_body
	:   LCURLY! (export_spec)*  RCURLY!
	;

export_spec
	:   (   type_dcl SEMI
	    |   const_dcl SEMI
	    |   except_dcl SEMI
	    |   attr_dcl SEMI
	    |   op_dcl SEMI
            )
	;

inheritance_spec
	:   COLON scoped_name_list
	|
	;

scoped_name_list
	:    scoped_name (COMMA scoped_name)*
	;

scoped_name
	:   opt_scope_op identifier (SCOPEOP identifier)*
	;

opt_scope_op
	:   SCOPEOP
	|
	;

const_dcl
	:   "const" const_type identifier ASSIGN const_exp
	;

const_type
	:   integer_type
	|   char_type
	|   boolean_type
	|   floating_pt_type
	|   string_type
	|   scoped_name
	;

/*   EXPRESSIONS   */

const_exp
	:   or_expr
	;

or_expr
	:   xor_expr
	     (   or_op
		xor_expr
             )*
	;

or_op
	:    OR
	;

xor_expr
	:    and_expr
	     (  xor_op
		and_expr
             )*
	;

xor_op
	:    XOR
	;

and_expr
	:    shift_expr
	     (  and_op
		shift_expr
             )*
	;

and_op
	:    AND
	;

shift_expr
	:    add_expr
	     (  shift_op
	     	add_expr
	     )*
	;

shift_op
	:    LSHIFT
	|    RSHIFT
	;

add_expr
	:    mult_expr
	     (  add_op
		mult_expr
             )*
	;

add_op
	:    PLUS
	|    MINUS
	;

mult_expr
	:   unary_expr
	     (  mult_op
		unary_expr
             )*
	;

mult_op
	:    STAR
	|    DIV
	|    MOD
	;

unary_expr
	:    unary_operator primary_expr
	|    primary_expr
	;

unary_operator
	:   MINUS
	|   PLUS
	|   TILDE
	;

// Node of type TPrimaryExp serves to avoid inf. recursion on tree parse
primary_expr
	:   scoped_name
	|   literal
	|   LPAREN const_exp RPAREN
	;

literal
	:   integer_literal
	|   string_literal
	|   character_literal
	|   floating_pt_literal
	|   boolean_literal
	;

boolean_literal
	:   "TRUE"
	|   "FALSE"
	;

positive_int_const
	:    const_exp
	;

type_dcl
	:   "typedef" type_declarator
	|   struct_type
	|   union_type
	|   enum_type
	|
	|   "native" simple_declarator
	;

type_declarator
	:   type_spec declarators
	;

type_spec
	:   simple_type_spec
	|   constr_type_spec
	;

simple_type_spec
	:   base_type_spec
	|   template_type_spec
	|   scoped_name
	;

base_type_spec
	:   integer_type
	|   char_type
	|   boolean_type
	|   floating_pt_type
	|   "octet"
	|   "any"
	;

integer_type
	:  ("unsigned")? ("short" | "long")
	;

char_type
	:   "char"
	;

floating_pt_type
	:   "float"
	|   "double"
	;

boolean_type
        :   "boolean"
	;

template_type_spec
	:   sequence_type
	|   string_type
	;

constr_type_spec
	:   struct_type
	|   union_type
	|   enum_type
	;

declarators
	:   declarator (COMMA declarator)*
	;

declarator
	:   identifier opt_fixed_array_size
	;

opt_fixed_array_size
	:	(fixed_array_size)*
	;

simple_declarator
	:   identifier
	;

struct_type
	:   "struct"
	    identifier
	    LCURLY member_list RCURLY
	;

member_list
	:   (member)+
	;

member
	:   type_spec declarators SEMI
	;

union_type
	:   "union"
 	    identifier
	         "switch" LPAREN switch_type_spec RPAREN
                  LCURLY switch_body RCURLY
	;

switch_type_spec
	:   integer_type
	|   char_type
	|   boolean_type
	|   enum_type
	|   scoped_name
	;

switch_body
	:   case_stmt_list
	;

case_stmt_list
	:  (case_stmt)+
	;

case_stmt
	:   case_label_list element_spec SEMI
	;

case_label_list
	:   (case_label)+
	;

case_label
	:   "case" const_exp COLON
	|   "default" COLON
	;

element_spec
	:   type_spec declarator
	;

enum_type
	:   "enum" identifier LCURLY enumerator_list RCURLY
	;

enumerator_list
	:    enumerator (COMMA enumerator)*
	;

enumerator
	:   identifier
	;

sequence_type
	:   "sequence"
	     LT_ simple_type_spec opt_pos_int GT
	;

opt_pos_int
	:    (COMMA positive_int_const)?
	;

string_type
	:   "string" opt_pos_int_br
	;

opt_pos_int_br
	:    (LT_ positive_int_const GT)?
	;

fixed_array_size
	:   LBRACK positive_int_const RBRACK
	;

attr_dcl
	:   ("readonly")?
            "attribute" param_type_spec
            simple_declarator_list
	;

simple_declarator_list
	:     simple_declarator (COMMA simple_declarator)*
	;

except_dcl
	:   "exception"
	    identifier
	     LCURLY opt_member_list RCURLY
	;

opt_member_list
	:    (member)*
	;

op_dcl
	:   op_attribute op_type_spec
            identifier
	    parameter_dcls
            opt_raises_expr c:opt_context_expr
	;

opt_raises_expr
	:   (raises_expr)?
	;

opt_context_expr
	:   (context_expr)?
	;

op_attribute
	:   "oneway"
	|
	;

op_type_spec
	:   param_type_spec
	|   "void"
	;

parameter_dcls
	:  LPAREN (param_dcl_list)? RPAREN!
	;

param_dcl_list
	:    param_dcl (COMMA param_dcl)*
	;

param_dcl
	:   param_attribute param_type_spec simple_declarator
	;

param_attribute
	:   "in"
	|   "out"
	|   "inout"
	;

raises_expr
	:   "raises" LPAREN scoped_name_list RPAREN
	;

context_expr
	:   "context" LPAREN string_literal_list RPAREN
	;

string_literal_list
	:    string_literal (COMMA! string_literal)*
	;

param_type_spec
	:   base_type_spec
	|   string_type
	|   scoped_name
	;

integer_literal
 	:   INT
	|   OCTAL
	|   HEX
        ;

string_literal
	:  (STRING_LITERAL)+
	;

character_literal
	:   CHAR_LITERAL
	;

floating_pt_literal
	:   f:FLOAT
     	;

identifier
	:   IDENT
  	;

/* IDL LEXICAL RULES  */
class IDLLexer extends Lexer;
options {
	exportVocab=IDL;
	k=4;
}

SEMI
options {
  paraphrase = ";";
}
	:	';'
	;

QUESTION
options {
  paraphrase = "?";
}
	:	'?'
	;

LPAREN
options {
  paraphrase = "(";
}
	:	'('
	;

RPAREN
options {
  paraphrase = ")";
}
	:	')'
	;

LBRACK
options {
  paraphrase = "[";
}
	:	'['
	;

RBRACK
options {
  paraphrase = "]";
}
	:	']'
	;

LCURLY
options {
  paraphrase = "{";
}
	:	'{'
	;

RCURLY
options {
  paraphrase = "}";
}
	:	'}'
	;

OR
options {
  paraphrase = "|";
}
	:	'|'
	;

XOR
options {
  paraphrase = "^";
}
	:	'^'
	;

AND
options {
  paraphrase = "&";
}
	:	'&'
	;

COLON
options {
  paraphrase = ":";
}
	:	':'
	;

COMMA
options {
  paraphrase = ",";
}
	:	','
	;

DOT
options {
  paraphrase = ".";
}
	:	'.'
	;

ASSIGN
options {
  paraphrase = "=";
}
	:	'='
	;

NOT
options {
  paraphrase = "!";
}
	:	'!'
	;

LT_
options {
  paraphrase = "<";
}
	:	'<'
	;

LSHIFT
options {
  paraphrase = "<<";
}
	: "<<"
	;

GT
options {
  paraphrase = ">";
}
	:	'>'
	;

RSHIFT
options {
  paraphrase = ">>";
}
	: ">>"
	;

DIV
options {
  paraphrase = "/";
}
	:	'/'
	;

PLUS
options {
  paraphrase = "+";
}
	:	'+'
	;

MINUS
options {
  paraphrase = "-";
}
	:	'-'
	;

TILDE
options {
  paraphrase = "~";
}
	:	'~'
	;

STAR
options {
  paraphrase = "*";
}
	:	'*'
	;

MOD
options {
  paraphrase = "%";
}
	:	'%'
	;

SCOPEOP
options {
  paraphrase = "::";
}
	:  	"::"
	;

WS_
options {
  paraphrase = "white space";
}
	:	(' '
	|	'\t'
	|	'\n'  { newline(); }
	|	'\r')
		{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
	;

PREPROC_DIRECTIVE
options {
  paraphrase = "a preprocessor directive";
}

	:
	'#'
	(~'\n')* '\n'
	{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
	;

SL_COMMENT
options {
  paraphrase = "a comment";
}

	:
	"//"
	(~'\n')* '\n'
	{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); newline(); }
	;

ML_COMMENT
options {
  paraphrase = "a comment";
}
	:
	"/*"
	(
			STRING_LITERAL
		|	CHAR_LITERAL
		|	'\n' { newline(); }
		|	'*' ~'/'
		|	~'*'
	)*
	"*/"
	{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP);  }
	;

CHAR_LITERAL
options {
  paraphrase = "a character literal";
}
	:
	'\''
	( ESC | ~'\'' )
	'\''
	;

STRING_LITERAL
options {
  paraphrase = "a string literal";
}
	:
	'"'
	(ESC|~'"')*
	'"'
	;

protected
ESC
options {
  paraphrase = "an escape sequence";
}
	:	'\\'
		(	'n'
		|	't'
		|	'v'
		|	'b'
		|	'r'
		|	'f'
		|	'a'
		|	'\\'
		|	'?'
		|	'\''
		|	'"'
		|	('0' | '1' | '2' | '3')
			(
				/* Since a digit can occur in a string literal,
				 * which can follow an ESC reference, ANTLR
				 * does not know if you want to match the digit
				 * here (greedy) or in string literal.
				 * The same applies for the next two decisions
				 * with the warnWhenFollowAmbig option.
				 */
				options {
					warnWhenFollowAmbig = false;
				}
			:	OCTDIGIT
				(
					options {
						warnWhenFollowAmbig = false;
					}
				:	OCTDIGIT
				)?
			)?
		|   'x' HEXDIGIT
			(
				options {
					warnWhenFollowAmbig = false;
				}
			:	HEXDIGIT
			)?
		)
	;

protected
VOCAB
options {
  paraphrase = "an escaped character value";
}
	:	'\3'..'\377'
	;

protected
DIGIT
options {
  paraphrase = "a digit";
}
	:	'0'..'9'
	;

protected
OCTDIGIT
options {
  paraphrase = "an octal digit";
}
	:	'0'..'7'
	;

protected
HEXDIGIT
options {
  paraphrase = "a hexadecimal digit";
}
	:	('0'..'9' | 'a'..'f' | 'A'..'F')
	;

/* octal literals are detected by checkOctal */

HEX
options {
  paraphrase = "a hexadecimal value value";
}

	:    ("0x" | "0X") (HEXDIGIT)+
	;

INT
options {
  paraphrase = "an integer value";
}
	:    (DIGIT)+                  // base-10
             (  '.' (DIGIT)*                      	{$setType(FLOAT);}
	         (('e' | 'E') ('+' | '-')? (DIGIT)+)?
	     |   ('e' | 'E') ('+' | '-')? (DIGIT)+   	{$setType(FLOAT);}
             )?
	;

FLOAT
options {
  paraphrase = "an floating point value";
}

	:    '.' (DIGIT)+ (('e' | 'E') ('+' | '-')? (DIGIT)+)?
     	;

IDENT
options {
  testLiterals = true;
  paraphrase = "an identifer";
}

	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;
