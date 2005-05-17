//
// Pascal Grammar
//
// Adapted from,
// Pascal User Manual And Report (Second Edition-1978)
// Kathleen Jensen - Niklaus Wirth
//
// By
//
// Hakki Dogusan dogusanh@tr-net.net.tr
//
// Then significantly enhanced by Piet Schoutteten
// with some guidance by Terence Parr.  Piet added tree
// construction, and some tree walkers.
//

{
import java.util.*;
import java.io.*;
import antlr.collections.AST;
import antlr.collections.impl.*;
import antlr.debug.misc.*;
import antlr.*;
}


class PascalParser extends Parser;
options {
  k = 2;                           // two token lookahead
  exportVocab=Pascal;              // Call its vocabulary "Pascal"
  codeGenMakeSwitchThreshold = 2;  // Some optimizations
  codeGenBitsetTestThreshold = 3;
  defaultErrorHandler = false;     // Don't generate parser error handlers
  buildAST = true;
  ASTLabelType = "PascalAST";
}

/* Define imaginary tokens used to organize tree
 *
 * One of the principles here is that any time you have a list of
 * stuff, you usually want to treat it like one thing (a list) a some
 * point in the grammar.  You want trees to have a fixed number of children
 * as much as possible.  For example, the definition of a procedure should
 * be something like #(PROCEDURE ID #(ARGDECLS ARG1 ARG2...)) not
 * #(PROCEDURE ID ARG1 ARG2 ... ) since this is harder to parse and
 * harder to manipulate.  Same is true for statement lists (BLOCK) etc...
 */
tokens {
	BLOCK; 		// list of statements
	IDLIST;		// list of identifiers; e.g., #(PROGRAM #(IDLIST ID ID...))
	ELIST;		// expression list for proc args etc...
	FUNC_CALL;
	PROC_CALL;
	SCALARTYPE; // IDLIST that is really a scalar type like (Mon,Tue,Wed)
	TYPELIST;	// list of types such as for array declarations
	VARIANT_TAG;// for CASEs in a RECORD
	VARIANT_TAG_NO_ID;// for CASEs in a RECORD (no id, just a type)
	VARIANT_CASE;// a case of the variant
	CONSTLIST;	// List of constants
	FIELDLIST;	// list of fields in a record
	ARGDECLS;	// overall group of declarations of args for proc/func.
	VARDECL;	// declaration of a variable
	ARGDECL;	// declaration of a parameter
	ARGLIST;	// list of actual arguments (expressions)
	TYPEDECL;	// declaration of a type
	FIELD;		// the root a RECORD field
}

// Define some methods and variables to use in the generated parser.
{
    /** Overall symbol table for translator */
    public static SymbolTable symbolTable = new SymbolTable();

    // This method decides what action to take based on the type of
    //   file we are looking at
    public  static void doFile(File f) throws Exception {
      // If this is a directory, walk each file/dir in that directory
      translateFilePath = f.getParent();
      if (f.isDirectory()) {
        String files[] = f.list();
        for(int i=0; i < files.length; i++)
        {
          doFile(new File(f, files[i]));
        }
      }
      // otherwise, if this is a Pascal file, parse it!
      else if ((f.getName().length()>4) &&
             f.getName().substring(f.getName().length()-4).toLowerCase().equals(".pas")) {
        System.err.println("   "+f.getAbsolutePath());

        if (translateFileName == null) {
          translateFileName = f.getName(); //set this file as the one to translate
          currentFileName = f.getName();
        }

        parseFile(f.getName(),new FileInputStream(f));
      }
      else {
        System.err.println("Can not parse:   "+f.getAbsolutePath());
      }
    }

    // Here's where we do the real work...
    public  static void parseFile(String f,InputStream s) throws Exception {
      try {
        currentFileName = f; // set this File as the currentFileName

        // Create a scanner that reads from the input stream passed to us
         PascalLexer lexer = new PascalLexer(s);

        // Create a parser that reads from the scanner
         PascalParser parser = new PascalParser(lexer);

        // set AST type to PascalAST (has symbol)
        parser.setASTNodeClass("PascalAST");

        // start parsing at the program rule
        parser.program(); 

        CommonAST t = (CommonAST)parser.getAST();

        // do something with the tree
        parser.doTreeAction(f, parser.getAST(), parser.getTokenNames());
        //System.out.println(parser.getAST().toStringList());
            


// build symbol table
        
        // Get the tree out of the parser
        AST resultTree1 = parser.getAST();

        // Make an instance of the tree parser
        // PascalTreeParserSuper treeParser1 = new PascalTreeParserSuper();
        SymtabPhase treeParser1 = new SymtabPhase();

        treeParser1.setASTNodeClass("PascalAST");

        // Begin tree parser at only rule
        treeParser1.program(resultTree1);



//        parser.doTreeAction(f, treeParser1.getAST(), treeParser1.getTokenNames());



       
      }
      catch (Exception e) {
        System.err.println("parser exception: "+e);
        e.printStackTrace();   // so we can get stack trace
      }
    }
    public void doTreeAction(String f, AST t, String[] tokenNames) {
      if ( t==null ) return;
      if ( showTree ) {
         ((CommonAST)t).setVerboseStringConversion(true, tokenNames);
         ASTFactory factory = new ASTFactory();
         AST r = factory.create(0,"AST ROOT");
         r.setFirstChild(t);
         ASTFrame frame = new ASTFrame("Pascal AST", r);
         frame.setVisible(true);
         //System.out.println(t.toStringList());
      }

    }
  static boolean showTree = true;
  public static String translateFilePath;
  public static String translateFileName;
  public static String currentFileName; // not static, recursive USES ... other FileName in currentFileName
  public static String oldtranslateFileName;


// main
  public static void main(String[] args) {
    // Use a try/catch block for parser exceptions
    try {
      // if we have at least one command-line argument
      if (args.length > 0 ) {

        // for each directory/file specified on the command line
        for(int i=0; i< args.length;i++)
{
	  if ( args[i].equals("-showtree") ) {
             showTree = true;
          }
          else {
            System.err.println("Parsing...");
            doFile(new File(args[i])); // parse it
          }
        }
      }
      else
        System.err.println("Usage: java PascalParser <file/directory name>");

    }
    catch(Exception e) {
      System.err.println("exception: "+e);
      e.printStackTrace(System.err);   // so we can get stack trace
    }
  }

}


program
    : programHeading (INTERFACE!)?
      block
      DOT!
    ;

programHeading
    : PROGRAM^ identifier LPAREN! identifierList RPAREN! SEMI!
    | UNIT^ identifier SEMI!
	;

identifier
    : IDENT
    ;

block
    : ( labelDeclarationPart
      | constantDefinitionPart
      | typeDefinitionPart
      | variableDeclarationPart
      | procedureAndFunctionDeclarationPart
      | usesUnitsPart
      | IMPLEMENTATION
      )*
      compoundStatement
    ;

usesUnitsPart
    : USES^ identifierList SEMI!
    ;

labelDeclarationPart
    : LABEL^ label ( COMMA! label )* SEMI!
    ;

label
    : unsignedInteger
    ;

constantDefinitionPart
    : CONST^ constantDefinition ( SEMI! constantDefinition )* SEMI!
    ;

constantDefinition
    : identifier EQUAL^ constant
    ;

constantChr
    : CHR^ LPAREN! unsignedInteger RPAREN!
    ;

constant
    : unsignedNumber
    |! s:sign n:unsignedNumber { #constant=#(s,n); }
    | identifier
    |! s2:sign id:identifier { #constant=#(s2,id); }
    | string
    | constantChr
    ;

unsignedNumber
    : unsignedInteger
    | unsignedReal
    ;

unsignedInteger
    : NUM_INT
    ;

unsignedReal
    : NUM_REAL
    ;

sign
    : PLUS | MINUS
    ;

string
    : STRING_LITERAL
    ;

typeDefinitionPart
    : TYPE^ typeDefinition ( SEMI! typeDefinition )* SEMI!
    ;

//PSPSPS
typeDefinition
    : identifier e:EQUAL^ {#e.setType(TYPEDECL);}
      ( type
      | functionType 
//      | FUNCTION^ (formalParameterList)? COLON! resultType
      | procedureType
//      | PROCEDURE^ (formalParameterList)?
      )
    ;

functionType
    : FUNCTION^ (formalParameterList)? COLON! resultType
    ;

procedureType
    : PROCEDURE^ (formalParameterList)?
    ;

type
    : simpleType
    | structuredType
    | pointerType
    ;

simpleType
    : scalarType
    | subrangeType
    | typeIdentifier
    | stringtype
    ;

scalarType
    : LPAREN^ identifierList RPAREN! {#scalarType.setType(SCALARTYPE);}
    ;

subrangeType
    : constant DOTDOT^ constant
    ;

typeIdentifier
    : identifier
    | CHAR
    | BOOLEAN
    | INTEGER
    | REAL
    | STRING // as in return type: FUNCTION ... (...): string;
    ;

structuredType
    : PACKED^ unpackedStructuredType
 	| unpackedStructuredType
    ;

unpackedStructuredType
    : arrayType
    | recordType
    | setType
    | fileType
    ;

stringtype
    : STRING^ LBRACK! (identifier|unsignedNumber) RBRACK!
    ;

arrayType
    : ARRAY^ LBRACK! typeList RBRACK! OF! componentType
    | ARRAY^ LBRACK2! typeList RBRACK2! OF! componentType
	;

typeList
	: indexType ( COMMA! indexType )*
	  {#typeList = #(#[TYPELIST],#typeList);}
	;

indexType
    : simpleType
    ;

componentType
    : type
    ;

recordType
    : RECORD^ fieldList END!
    ;

fieldList
    : (	fixedPart ( SEMI! variantPart | SEMI! )?
      | variantPart
      )
      {#fieldList=#([FIELDLIST],#fieldList);}
    ;

fixedPart
    : recordSection ( SEMI! recordSection )*
    ;

recordSection
    : identifierList COLON! type
      {#recordSection = #([FIELD],#recordSection);}
    ;

variantPart
    : CASE^ tag OF! variant ( SEMI! variant | SEMI! )*
    ;

tag!
    : id:identifier COLON t:typeIdentifier {#tag=#([VARIANT_TAG],id,t);}
    | t2:typeIdentifier                    {#tag=#([VARIANT_TAG_NO_ID],t2);}
    ;

variant
    : constList c:COLON^ {#c.setType(VARIANT_CASE);}
	  LPAREN! fieldList RPAREN!
    ;

setType
    : SET^ OF! baseType
    ;

baseType
    : simpleType
    ;

fileType
    : FILE^ OF! type
    | FILE
    ;

pointerType
    : POINTER^ typeIdentifier
    ;

/** Yields a list of VARDECL-rooted subtrees with VAR at the overall root */
variableDeclarationPart
    : VAR^ variableDeclaration ( SEMI! variableDeclaration )* SEMI!
    ;

variableDeclaration
    : identifierList c:COLON^ {#c.setType(VARDECL);} type
    ;

procedureAndFunctionDeclarationPart
    : procedureOrFunctionDeclaration SEMI!
    ;

procedureOrFunctionDeclaration
    : procedureDeclaration
    | functionDeclaration
    ;

procedureDeclaration
    : PROCEDURE^ identifier (formalParameterList)? SEMI!
      block
    ;

formalParameterList
    : LPAREN^ formalParameterSection ( SEMI! formalParameterSection )* RPAREN!
	  {#formalParameterList.setType(ARGDECLS);}
    ;

formalParameterSection
    : parameterGroup
    | VAR^ parameterGroup
    | FUNCTION^ parameterGroup
    | PROCEDURE^ parameterGroup
    ;

parameterGroup!
    : ids:identifierList COLON! t:typeIdentifier
	  {#parameterGroup = #([ARGDECL],ids,t);}
    ;

identifierList
    : identifier ( COMMA! identifier )*
	  {#identifierList = #(#[IDLIST],#identifierList);}
    ;

constList
    : constant ( COMMA! constant )*
	  {#constList = #([CONSTLIST],#constList);}
    ;

functionDeclaration
    : FUNCTION^ identifier (formalParameterList)? COLON! resultType SEMI!
      block
    ;

resultType
    : typeIdentifier
    ;

statement
    : label COLON^ unlabelledStatement
    | unlabelledStatement
    ;

unlabelledStatement
    : simpleStatement
    | structuredStatement
    ;

simpleStatement
    : assignmentStatement
    | procedureStatement
    | gotoStatement
    | emptyStatement
    ;

assignmentStatement
    : variable ASSIGN^ expression
    ;

/** A variable is an id with a suffix and can look like:
 *  id
 *  id[expr,...]
 *  id.id
 *  id.id[expr,...]
 *  id^
 *  id^.id
 *  id^.id[expr,...]
 *  ...
 *
 *  LL has a really hard time with this construct as it's naturally
 *  left-recursive.  We have to turn into a simple loop rather than
 *  recursive loop, hence, the suffixes.  I keep in the same rule
 *  for easy tree construction.
 */
variable
    : ( AT^ identifier // AT is root of identifier; then other op becomes root
      | identifier
      )
      (	LBRACK^ expression ( COMMA! expression)* RBRACK!
      | LBRACK2^ expression ( COMMA! expression)* RBRACK2!
      | DOT^ identifier
      | POINTER^
      )*
    ;

expression
    : simpleExpression
	  ( (EQUAL^ | NOT_EQUAL^ | LT^ | LE^ | GE^ | GT^ | IN^) simpleExpression )*
    ;

simpleExpression
    : term ( (PLUS^ | MINUS^ | OR^) term )*
    ;

term
	: signedFactor ( (STAR^ | SLASH^ | DIV^ | MOD^ | AND^) signedFactor )*
    ;

signedFactor
    : (PLUS^|MINUS^)? factor
    ;

factor
    : variable
    | LPAREN! expression RPAREN!
    | functionDesignator
    | unsignedConstant
    | set
    | NOT^ factor
    ;

unsignedConstant
    : unsignedNumber
    | constantChr         //pspsps added
    | string
    | NIL
    ;

functionDesignator!
    : id:identifier LPAREN! args:parameterList RPAREN!
      {#functionDesignator = #([FUNC_CALL],id,args);}
    ;

parameterList
    : actualParameter ( COMMA! actualParameter )*
	  {#parameterList = #([ARGLIST],#parameterList);}
    ;

set
    : LBRACK^ elementList RBRACK!   {#set.setType(SET);}
    | LBRACK2^ elementList RBRACK2! {#set.setType(SET);}
    ;

elementList
    : element ( COMMA! element )*
    |
    ;

element
    : expression ( DOTDOT^ expression )?
    ;

procedureStatement!
    : id:identifier ( LPAREN! args:parameterList RPAREN! )?
      {#procedureStatement = #([PROC_CALL],id,args);}
    ;

actualParameter
    : expression
    ;

gotoStatement
    : GOTO^ label
    ;

emptyStatement
    :
    ;

empty
    : /* empty */
    ;

structuredStatement
    : compoundStatement
    | conditionalStatement
    | repetetiveStatement
    | withStatement
    ;

compoundStatement
    : BEGIN!
		statements
      END!
    ;

statements
    : statement ( SEMI! statement )* {#statements = #([BLOCK],#statements);}
    ;

conditionalStatement
    : ifStatement
    | caseStatement
    ;

ifStatement
    : IF^ expression THEN! statement
      (
		// CONFLICT: the old "dangling-else" problem...
		//           ANTLR generates proper code matching
		//			 as soon as possible.  Hush warning.
		options {
			generateAmbigWarnings=false;
		}
		: ELSE! statement
	  )?
    ;

caseStatement //pspsps ???
    : CASE^ expression OF!
        caseListElement ( SEMI! caseListElement )*
      ( SEMI! ELSE! statements )?
      END!
    ;

caseListElement
    : constList COLON^ statement
    ;

repetetiveStatement
    : whileStatement
    | repeatStatement
    | forStatement
    ;

whileStatement
    : WHILE^ expression DO! statement
    ;

repeatStatement
    : REPEAT^ statements UNTIL! expression
    ;

forStatement
    : FOR^ identifier ASSIGN! forList DO! statement
    ;

forList
    : initialValue (TO^ | DOWNTO^) finalValue
    ;

initialValue
    : expression
    ;

finalValue
    : expression
    ;

withStatement
    : WITH^ recordVariableList DO! statement
    ;

recordVariableList
    : variable ( COMMA! variable )*
    ;

//----------------------------------------------------------------------------
// The Pascal scanner
//----------------------------------------------------------------------------
class PascalLexer extends Lexer;

options {
  charVocabulary = '\0'..'\377';
  exportVocab = Pascal;   // call the vocabulary "Pascal"
  testLiterals = false;   // don't automatically test for literals
  k = 4;                  // four characters of lookahead
  caseSensitive = false;
  caseSensitiveLiterals = false;
}

tokens {
  AND              = "and"             ;
  ARRAY            = "array"           ;
  BEGIN            = "begin"           ;
  BOOLEAN          = "boolean"         ;
  CASE             = "case"            ;
  CHAR             = "char"            ;
  CHR              = "chr"             ;
  CONST            = "const"           ;
  DIV              = "div"             ;
  DO               = "do"              ;
  DOWNTO           = "downto"          ;
  ELSE             = "else"            ;
  END              = "end"             ;
  FILE             = "file"            ;
  FOR              = "for"             ;
  FUNCTION         = "function"        ;
  GOTO             = "goto"            ;
  IF               = "if"              ;
  IN               = "in"              ;
  INTEGER          = "integer"         ;
  LABEL            = "label"           ;
  MOD              = "mod"             ;
  NIL              = "nil"             ;
  NOT              = "not"             ;
  OF               = "of"              ;
  OR               = "or"              ;
  PACKED           = "packed"          ;
  PROCEDURE        = "procedure"       ;
  PROGRAM          = "program"         ;
  REAL             = "real"            ;
  RECORD           = "record"          ;
  REPEAT           = "repeat"          ;
  SET              = "set"             ;
  THEN             = "then"            ;
  TO               = "to"              ;
  TYPE             = "type"            ;
  UNTIL            = "until"           ;
  VAR              = "var"             ;
  WHILE            = "while"           ;
  WITH             = "with"            ;
  METHOD                               ;
  ADDSUBOR                             ;
  ASSIGNEQUAL                          ;
  SIGN                                 ;
  FUNC                                 ;
  NODE_NOT_EMIT                        ;
  MYASTVAR                             ;
  LF                                   ;
  UNIT             = "unit"            ;
  INTERFACE        = "interface"       ;
  USES             = "uses"            ;
  STRING           = "string"          ;
  IMPLEMENTATION   = "implementation"  ;
//pspsps ???
}

//----------------------------------------------------------------------------
// OPERATORS
//----------------------------------------------------------------------------
PLUS            : '+'   ;
MINUS           : '-'   ;
STAR            : '*'   ;
SLASH           : '/'   ;
ASSIGN          : ":="  ;
COMMA           : ','   ;
SEMI            : ';'   ;
COLON           : ':'   ;
EQUAL           : '='   ;
NOT_EQUAL       : "<>"  ;
LT              : '<'   ;
LE              : "<="  ;
GE              : ">="  ;
GT              : '>'   ;
LPAREN          : '('   ;
RPAREN          : ')'   ;
LBRACK          : '['   ; // line_tab[line]
LBRACK2         : "(."  ; // line_tab(.line.)
RBRACK          : ']'   ;
RBRACK2         : ".)"  ;
POINTER         : '^'   ;
AT              : '@'   ;
DOT             : '.' ('.' {$setType(DOTDOT);})?  ;
LCURLY          : "{" ;
RCURLY          : "}" ;


// Whitespace -- ignored
WS      : ( ' '
		|	'\t'
		|	'\f'
		// handle newlines
		|	(	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ newline(); }
		)
		{ _ttype = Token.SKIP; }
	;


COMMENT_1
        : "(*"
		   ( options { generateAmbigWarnings=false; }
		   :	{ LA(2) != ')' }? '*'
		   |	'\r' '\n'		{newline();}
		   |	'\r'			{newline();}
		   |	'\n'			{newline();}
           |   ~('*' | '\n' | '\r')
		   )*
          "*)"
		{$setType(Token.SKIP);}
	;

COMMENT_2
        :  '{'
		    ( options {generateAmbigWarnings=false;}
            :   '\r' '\n'       {newline();}
		    |	'\r'			{newline();}
		    |	'\n'			{newline();}
            |   ~('}' | '\n' | '\r')
		    )*
           '}'
		{$setType(Token.SKIP);}
	;

// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
IDENT
	options {testLiterals=true;}
	:	('a'..'z') ('a'..'z'|'0'..'9'|'_')*   //pspsps
	;

// string literals
STRING_LITERAL
	: '\'' ("\'\'" | ~('\''))* '\''   //pspsps   * in stead of + because of e.g. ''
	;

/** a numeric literal.  Form is (from Wirth)
 *  digits
 *  digits . digits
 *  digits . digits exponent
 *  digits exponent
 */
NUM_INT
	:	('0'..'9')+ // everything starts with a digit sequence
		(	(	{(LA(2)!='.')&&(LA(2)!=')')}?				// force k=2; avoid ".."
//PSPSPS example ARRAY (.1..99.) OF char; // after .. thinks it's a NUM_REAL
				'.' {$setType(NUM_REAL);}	// dot means we are float
				('0'..'9')+ (EXPONENT)?
			)?
		|	EXPONENT {$setType(NUM_REAL);}	// 'E' means we are float
		)
	;

// a couple protected methods to assist in matching floating point numbers
protected
EXPONENT
	:	('e') ('+'|'-')? ('0'..'9')+
	;
