header {
	package antlr.actions.cpp;
}

{
	import java.io.StringReader;
	import antlr.collections.impl.Vector;
	import antlr.*;
}

/* ANTLR Translator Generator
 * Project led by Terence Parr at http://www.jGuru.com
 * Software rights: http://www.antlr.org/license.html
 *
 * $Id:$
 */

/** Perform the following translations:

    AST related translations

	##				-> currentRule_AST
	#(x,y,z)		-> codeGenerator.getASTCreateString(vector-of(x,y,z))
	#[x]			-> codeGenerator.getASTCreateString(x)
	#x				-> codeGenerator.mapTreeId(x)

	Inside context of #(...), you can ref (x,y,z), [x], and x as shortcuts.

    Text related translations

	$append(x)	  -> text.append(x)
	$setText(x)	  -> text.setLength(_begin); text.append(x)
	$getText		  -> new String(text.getBuffer(),_begin,text.length()-_begin)
	$setToken(x)  -> _token = x
	$setType(x)	  -> _ttype = x
   $FOLLOW(r)    -> FOLLOW set name for rule r (optional arg)
   $FIRST(r)     -> FIRST set name for rule r (optional arg)
 */
class ActionLexer extends Lexer;
options {
	k = 3;
	charVocabulary = '\3'..'\377';
	testLiterals=false;
	interactive=true;
}

{
	protected RuleBlock currentRule;
	protected CodeGenerator generator;
	protected int lineOffset = 0;
	private Tool antlrTool;	// The ANTLR tool
	ActionTransInfo transInfo;

 	public ActionLexer(String s, RuleBlock currentRule,
							 CodeGenerator generator,
							 ActionTransInfo transInfo )
	{
		this(new StringReader(s));
		this.currentRule = currentRule;
		this.generator = generator;
		this.transInfo = transInfo;
	}

	public void setLineOffset(int lineOffset)
	{
		setLine(lineOffset);
	}

	public void setTool(Tool tool)
	{
		this.antlrTool = tool;
	}

	public void reportError(RecognitionException e)
	{
		antlrTool.error("Syntax error in action: "+e,getFilename(),getLine(),getColumn());
	}

	public void reportError(String s)
	{
		antlrTool.error(s,getFilename(),getLine(),getColumn());
	}

	public void reportWarning(String s)
	{
		if ( getFilename()==null )
			antlrTool.warning(s);
		else
			antlrTool.warning(s,getFilename(),getLine(),getColumn());
	}
}

// rules are protected because we don't care about nextToken().
public
ACTION
	:	(	STUFF
		|	AST_ITEM
		|	TEXT_ITEM
		)+
	;

/** stuff in between #(...) and #id items
 * Allow the escaping of the # for C preprocessor stuff.
 */
protected
STUFF
	:	COMMENT
	|	STRING
	|	CHAR
	|	"\r\n" 	{ newline(); }
	|	'\\' '#'	{ $setText("#"); }
	|	'\r' 		{ newline(); }
	|	'\n'		{ newline(); }
	|	'/' ~('/'|'*')	// non-comment start '/'
//	|	( ~('/'|'\n'|'\r'|'$'|'#'|'"'|'\'') )+
	|	~('/'|'\n'|'\r'|'$'|'#'|'"'|'\'')
	;

protected
AST_ITEM
	:	'#'! t:TREE													// #( )
	|	'#'! (WS)? id:ID											// #a_name (=)?
		{
			String idt = id.getText();
			String mapped = generator.mapTreeId(id.getText(), transInfo);

			// verify that it's not a preprocessor macro...
			if( mapped!=null && ! idt.equals( mapped ) )
			{
				$setText(mapped);
			}
			else
			{
				if(idt.equals("if") ||
					idt.equals("define") ||
					idt.equals("ifdef") ||
					idt.equals("ifndef") ||
					idt.equals("else") ||
					idt.equals("elif") ||
					idt.equals("endif") ||
					idt.equals("warning") ||
					idt.equals("error") ||
					idt.equals("ident") ||
					idt.equals("pragma") ||
					idt.equals("include"))
				{
					$setText("#"+idt);
				}
			}
		}
		(WS)?
		( options {greedy=true;} : VAR_ASSIGN )?
	|	'#'! ctor:AST_CONSTRUCTOR								// #[ ]
	|	"##"
		{
			if( currentRule != null )
			{
				String r = currentRule.getRuleName()+"_AST";
				$setText(r);

				if ( transInfo!=null ) {
					transInfo.refRuleRoot=r;	// we ref root of tree
				}
			}
			else
			{
				reportWarning("\"##\" not valid in this context");
				$setText("##");
			}
		}
		(WS)?
		( options {greedy=true;} : VAR_ASSIGN )?
	;

protected
TEXT_ITEM
	:	"$append" (WS)? '(' a1:TEXT_ARG ')'
		{
			String t = "text += "+a1.getText();
			$setText(t);
		}
	|	"$set"
		(	"Text" (WS)? '(' a2:TEXT_ARG ')'
			{
				String t;
				t = "{ text.erase(_begin); text += "+a2.getText()+"; }";
				$setText(t);
			}
		|	"Token" (WS)? '(' a3:TEXT_ARG ')'
			{
				String t="_token = "+a3.getText();
				$setText(t);
			}
		|	"Type" (WS)? '(' a4:TEXT_ARG ')'
			{
				String t="_ttype = "+a4.getText();
				$setText(t);
			}
		)
	|	"$getText"
		{
			$setText("text.substr(_begin,text.length()-_begin)");
		}
	|	"$FOLLOW" ( (WS)? '(' a5:TEXT_ARG ')' )?
		{
			String rule = currentRule.getRuleName();
			if ( a5!=null ) {
				rule = a5.getText();
			}
			String setName = generator.getFOLLOWBitSet(rule, 1);
			// System.out.println("FOLLOW("+rule+")="+setName);
			if ( setName==null ) {
				reportError("$FOLLOW("+rule+")"+
							": unknown rule or bad lookahead computation");
			}
			else {
				$setText(setName);
			}
		}
	|	"$FIRST" ( (WS)? '(' a6:TEXT_ARG ')' )?
		{
			String rule = currentRule.getRuleName();
			if ( a6!=null ) {
				rule = a6.getText();
			}
			String setName = generator.getFIRSTBitSet(rule, 1);
			// System.out.println("FIRST("+rule+")="+setName);
			if ( setName==null ) {
				reportError("$FIRST("+rule+")"+
							": unknown rule or bad lookahead computation");
			}
			else {
				$setText(setName);
			}
		}
	;

protected
TREE!
{
	StringBuffer buf = new StringBuffer();
	int n=0;
	Vector terms = new Vector(10);
}
	:	'('
		(WS)?
		t:TREE_ELEMENT
		{
			terms.appendElement(
				generator.processStringForASTConstructor(t.getText())
									 );
		}
		(WS)?
		(	','	(WS)?
			t2:TREE_ELEMENT
	 		{
				terms.appendElement(
					generator.processStringForASTConstructor(t2.getText())
										  );
			}
			(WS)?
		)*
		{$setText(generator.getASTCreateString(terms));}
		')'
	;

protected
TREE_ELEMENT { boolean was_mapped; }
	:	'#'! TREE
	|	'#'! AST_CONSTRUCTOR
	|	'#'! was_mapped=id:ID_ELEMENT
		{	// RK: I have a queer feeling that this maptreeid is redundant..
			if ( ! was_mapped )
			{
				String t = generator.mapTreeId(id.getText(), null);
//				System.out.println("mapped: "+id.getText()+" -> "+t);
				if ( t!=null ) {
					$setText(t);
				}
			}
		}
	|	"##"
		{
			if( currentRule != null )
			{
				String t = currentRule.getRuleName()+"_AST";
				$setText(t);
			}
			else
			{
				reportError("\"##\" not valid in this context");
				$setText("##");
			}
		}
	|	TREE
	|	AST_CONSTRUCTOR
	|	ID_ELEMENT
	|	STRING
	;

// FIXME: RK - the getASTCreateString here is broken.
// getASTCreateString can not cleanly see if a constructor like
// tokens { FOR<AST=ForNode>; }
// forLoop:! "for" bla bla
//  { #forLoop = #([FOR,"for"], bla bla ) }
// should use ForNode as AST.
//
protected
AST_CONSTRUCTOR!
	:	'[' (WS)? x:AST_CTOR_ELEMENT (WS)?
		(',' (WS)? y:AST_CTOR_ELEMENT (WS)? )? ']'
		{
//			System.out.println("AST_CONSTRUCTOR: "+((x==null)?"null":x.getText())+
//									 ", "+((y==null)?"null":y.getText()));
			String ys = generator.processStringForASTConstructor(x.getText());

			// the second does not need processing coz it's a string
			// (eg second param of astFactory.create(x,y)
			if ( y!=null )
				ys += ","+y.getText();

			$setText( generator.getASTCreateString(null,ys) );
		}
	;

/** The arguments of a #[...] constructor are text, token type,
 *  or a tree.
 */
protected
AST_CTOR_ELEMENT
	:	STRING
	|	INT
	|	TREE_ELEMENT
	;

/** An ID_ELEMENT can be a func call, array ref, simple var,
 *  or AST label ref.
 */
protected
ID_ELEMENT returns [boolean mapped=false]
	:	id:ID (options {greedy=true;}:WS!)?
		(	('<' (~'>')* '>')? // allow typecast
		 	'(' (options {greedy=true;}:WS!)? ( ARG (',' (WS!)? ARG)* )? (WS!)? ')'	// method call
		|	( '[' (WS!)? ARG (WS!)? ']' )+				// array reference
		|	'.' ID_ELEMENT
		|	"->" ID_ELEMENT
		|	"::" ID_ELEMENT
		|  /* could be a token reference or just a user var */
			{
				mapped = true;
				String t = generator.mapTreeId(id.getText(), transInfo);
//				System.out.println("mapped: "+id.getText()+" -> "+t);
				if ( t!=null ) {
					$setText(t);
				}
			}
			// if #rule referenced, check for assignment
			(	options {greedy=true;}
			:	{transInfo!=null && transInfo.refRuleRoot!=null}?
				(WS)? VAR_ASSIGN
			)?
		)
	;

protected
TEXT_ARG
	:	(WS)? ( TEXT_ARG_ELEMENT (options {greedy=true;}:WS)? )+
	;

protected
TEXT_ARG_ELEMENT
	:	TEXT_ARG_ID_ELEMENT
	|	STRING
	|	CHAR
	|	INT_OR_FLOAT
	|	TEXT_ITEM
	|	'+'
	;

protected
TEXT_ARG_ID_ELEMENT
	:	id:ID (options {greedy=true;}:WS!)?
		(	'(' (options {greedy=true;}:WS!)? ( TEXT_ARG (',' TEXT_ARG)* )* (WS!)? ')'	// method call
		|	( '[' (WS!)? TEXT_ARG (WS!)? ']' )+				// array reference
		|	'.' TEXT_ARG_ID_ELEMENT
		|	"->" TEXT_ARG_ID_ELEMENT
		|	"::" TEXT_ARG_ID_ELEMENT
		|
		)
	;

protected
ARG	:	(	TREE_ELEMENT
		|	STRING
		|	CHAR
		|	INT_OR_FLOAT
		)
		(options {greedy=true;} : (WS)? ( '+'| '-' | '*' | '/' ) (WS)? ARG )*
	;

protected
ID	:	('a'..'z'|'A'..'Z'|'_'|"::")
		(options {greedy=true;} : ('a'..'z'|'A'..'Z'|'0'..'9'|'_'|"::"))*
	;

protected
VAR_ASSIGN
	:	'='
		{
			// inform the code generator that an assignment was done to
			// AST root for the rule if invoker set refRuleRoot.
			if ( LA(1)!='=' && transInfo!=null && transInfo.refRuleRoot!=null ) {
				transInfo.assignToRoot=true;
			}
		}
	;

protected
COMMENT
	:	SL_COMMENT
	|	ML_COMMENT
	;

protected
SL_COMMENT
	:	"//" (options {greedy=false;}:.)* ('\n'|"\r\n"|'\r')
		{newline();}
	;

protected
ML_COMMENT :
	"/*"
	(	options {greedy=false;}
	:	'\r' '\n'	{newline();}
	|	'\r' 		{newline();}
	|	'\n'		{newline();}
	|	.
	)*
	"*/"
	;

protected
CHAR :
	'\''
	( ESC | ~'\'' )
	'\''
	;

protected
STRING :
	'"'
	(ESC|~'"')*
	'"'
	;

protected
ESC	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'v'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('0'..'3')
			(	options {greedy=true;}
			:	DIGIT
				(	options {greedy=true;}
				:	DIGIT
				)?
			)?
		|	('4'..'7') (options {greedy=true;}:DIGIT)?
		)
	;

protected
DIGIT
	:	'0'..'9'
	;

protected
INT	:	(DIGIT)+
	;

protected
INT_OR_FLOAT
	:	(options {greedy=true;}:DIGIT)+
		(	options {greedy=true;}
		:	'.' (options {greedy=true;}:DIGIT)*
		|	'L'
		|	'l'
		)?
	;

protected
WS	:	(	options {greedy=true;}
		: 	' '
		|	'\t'
		|	'\r' '\n'	{newline();}
		|	'\r'		{newline();}
		|	'\n'		{newline();}
		)+
	;
