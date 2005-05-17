header {
package antlr.preprocessor;
}

/* ANTLR Translator Generator
 * Project led by Terence Parr at http://www.jGuru.com
 * Software rights: http://www.antlr.org/license.html
 *
 * $Id: //depot/code/org.antlr/release/antlr-2.7.5/antlr/preprocessor/preproc.g#1 $
 */

{
import antlr.collections.impl.IndexedVector;
import java.util.Hashtable;
import antlr.preprocessor.Grammar;
}

class Preprocessor extends Parser;
options {
	k=1;
	interactive=true;
}

tokens {
	"tokens";
}

{
	// This chunk of error reporting code provided by Brian Smith

    private antlr.Tool antlrTool;

    /** In order to make it so existing subclasses don't break, we won't require
     * that the antlr.Tool instance be passed as a constructor element. Instead,
     * the antlr.Tool instance should register itself via {@link #initTool(antlr.Tool)}
     * @throws IllegalStateException if a tool has already been registered
     * @since 2.7.2
     */
    public void setTool(antlr.Tool tool) {
        if (antlrTool == null) {
            antlrTool = tool;
		}
        else {
            throw new IllegalStateException("antlr.Tool already registered");
		}
    }

    /** @since 2.7.2 */
    protected antlr.Tool getTool() {
        return antlrTool;
    }

    /** Delegates the error message to the tool if any was registered via
     *  {@link #initTool(antlr.Tool)}
     *  @since 2.7.2
     */
    public void reportError(String s) {
        if (getTool() != null) {
            getTool().error(s, getFilename(), -1, -1);
		}
        else {
            super.reportError(s);
		}
    }

    /** Delegates the error message to the tool if any was registered via
     *  {@link #initTool(antlr.Tool)}
     *  @since 2.7.2
     */
    public void reportError(RecognitionException e) {
        if (getTool() != null) {
            getTool().error(e.getErrorMessage(), e.getFilename(), e.getLine(), e.getColumn());
		}
        else {
            super.reportError(e);
		}
    }

    /** Delegates the warning message to the tool if any was registered via
     *  {@link #initTool(antlr.Tool)}
     *  @since 2.7.2
     */
    public void reportWarning(String s) {
        if (getTool() != null) {
            getTool().warning(s, getFilename(), -1, -1);
		}
        else {
            super.reportWarning(s);
		}
    }
}

grammarFile[Hierarchy hier, String file]
{
	Grammar gr;
	IndexedVector opt=null;
}
	:	( hdr:HEADER_ACTION { hier.getFile(file).addHeaderAction(hdr.getText()); } )*
		( opt=optionSpec[null] )?
		(	gr=class_def[file, hier]
			{
			if ( gr!=null && opt!=null ) {
				hier.getFile(file).setOptions(opt);
			}
			if ( gr!=null ) {
				gr.setFileName(file);
				hier.addGrammar(gr);
			}
			}
		)*
		EOF
	;

superClass returns [String sup]
{sup=LT(1).getText();}
	:	SUBRULE_BLOCK // looks like ("mypackage.MyParserClass")
	;


class_def[String file, Hierarchy hier] returns [Grammar gr]
{
	gr=null;
	IndexedVector rules = new IndexedVector(100);
	IndexedVector classOptions = null;
	String sc = null;
}
	:	( preamble:ACTION )?
		"class" sub:ID "extends" sup:ID (sc=superClass)? SEMI
		{
			gr = (Grammar)hier.getGrammar(sub.getText());
			if ( gr!=null ) {
//				antlr.Tool.toolError("redefinition of grammar "+gr.getName()+" ignored");
				gr=null;
				throw new SemanticException("redefinition of grammar "+sub.getText(), file, sub.getLine(), sub.getColumn());
			}
			else {
				gr = new Grammar(hier.getTool(), sub.getText(), sup.getText(), rules);
				gr.superClass=sc;
				if ( preamble!=null ) {
					gr.setPreambleAction(preamble.getText());
				}
			}
		}
		( classOptions = optionSpec[gr] )?
		{
		if ( gr!=null ) {
			gr.setOptions(classOptions);
		}
		}
		( tk:TOKENS_SPEC {gr.setTokenSection(tk.getText());} )?
		( memberA:ACTION {gr.setMemberAction(memberA.getText());} )?
		( rule[gr] )+
	;

optionSpec[Grammar gr] returns [IndexedVector options]
{
	options = new IndexedVector();
}
	:	OPTIONS_START
			(	op:ID rhs:ASSIGN_RHS
				{
				Option newOp = new Option(op.getText(),rhs.getText(),gr);
				options.appendElement(newOp.getName(),newOp);
				if ( gr!=null && op.getText().equals("importVocab") ) {
					gr.specifiedVocabulary = true;
					gr.importVocab = rhs.getText();
				}
				else if ( gr!=null && op.getText().equals("exportVocab") ) {
					// don't want ';' included in outputVocab.
					// This is heinously inconsistent!  Ugh.
					gr.exportVocab = rhs.getText().substring(0,rhs.getText().length()-1);
					gr.exportVocab = gr.exportVocab.trim();
				}
				}
			)*
//		{gr.fixupVocabOptionsForInheritance();}
		RCURLY
	;

rule[Grammar gr]
{
	IndexedVector o = null;	// options for rule
	String vis = null;
	boolean bang=false;
	String eg=null, thr="";
}
	:	(	"protected"	{vis="protected";}
		|	"private"	{vis="private";}
		|	"public"	{vis="public";}
		)?
		r:ID
		( BANG {bang=true;} )?
		( arg:ARG_ACTION )?
		( "returns" ret:ARG_ACTION )?
		( thr=throwsSpec )?
		( o = optionSpec[null] )?
		( init:ACTION )?
		blk:RULE_BLOCK
		eg=exceptionGroup
		{
		String rtext = blk.getText()+eg;
		Rule ppr = new Rule(r.getText(),rtext,o,gr);
		ppr.setThrowsSpec(thr);
		if ( arg!=null ) {
			ppr.setArgs(arg.getText());
		}
		if ( ret!=null ) {
			ppr.setReturnValue(ret.getText());
		}
		if ( init!=null ) {
			ppr.setInitAction(init.getText());
		}
		if ( bang ) {
			ppr.setBang();
		}
		ppr.setVisibility(vis);
		if ( gr!=null ) {
			gr.addRule(ppr);
		}
		}
	;

throwsSpec returns [String t]
{t="throws ";}
	:	"throws" a:ID {t+=a.getText();}
		( COMMA b:ID {t+=","+b.getText();} )*
	;

exceptionGroup returns [String g]
{String e=null; g="";}
	:	( e=exceptionSpec {g += e;} )*
	;

exceptionSpec returns [String es]
{ String h=null;
  es = System.getProperty("line.separator")+"exception ";
}
	:	"exception"
		( aa:ARG_ACTION {es += aa.getText();} )?
		( h=exceptionHandler {es += h;} )*
	;

exceptionHandler returns [String h]
{h=null;}
	:	"catch" a1:ARG_ACTION a2:ACTION
		{h = System.getProperty("line.separator")+
			 "catch "+a1.getText()+" "+a2.getText();}
	;

class PreprocessorLexer extends Lexer;
options {
	k=2;
	charVocabulary = '\3'..'\377';	// common ASCII
	interactive=true;
}

RULE_BLOCK
    :   ':' (options {greedy=true;}:WS!)?
		ALT (options {greedy=true;}:WS!)?
		( '|' (options {greedy=true;}:WS!)? ALT (options {greedy=true;}:WS!)? )* ';'
    ;

SUBRULE_BLOCK
	:	'(' (options {greedy=true;}:WS)? ALT
		(	options {greedy=true;}
		:	(WS)? '|' (options {greedy=true;}:WS)? ALT
		)*
		(WS)?
		')'
		(	options {greedy=true;}
		:	'*'
		|	'+'
		|	'?'
		|	"=>"
		)?
	;

protected
ALT	:	(options {greedy=true;} : ELEMENT)*
	;

protected
ELEMENT
	:	COMMENT
	|	ACTION
	|	STRING_LITERAL
	|	CHAR_LITERAL
	|	SUBRULE_BLOCK
	|	NEWLINE
	|	~('\n' | '\r' | '(' | ')' | '/' | '{' | '"' | '\'' | ';')
	;

BANG:	'!'
	;

SEMI:	';'
	;

COMMA:	','
	;

RCURLY
	:	'}'
	;

LPAREN : '(' ;

RPAREN : ')' ;

/** This rule picks off keywords in the lexer that need to be
 *  handled specially.  For example, "header" is the start
 *  of the header action (used to distinguish between options
 *  block and an action).  We do not want "header" to go back
 *  to the parser as a simple keyword...it must pick off
 *  the action afterwards.
 */
ID_OR_KEYWORD
	:	id:ID	{$setType(id.getType());}
		(	{id.getText().equals("header")}? (options {greedy=true;}:WS)?
			(STRING_LITERAL)? (WS|COMMENT)* ACTION
			{$setType(HEADER_ACTION);}
		|	{id.getText().equals("tokens")}? (WS|COMMENT)* CURLY_BLOCK_SCARF
			{$setType(TOKENS_SPEC);}
		|	{id.getText().equals("options")}? (WS|COMMENT)* '{'
			{$setType(OPTIONS_START);}
		)?
	;


protected
CURLY_BLOCK_SCARF
	:	'{'
		(	options {greedy=false;}
		:	NEWLINE
		|	STRING_LITERAL
		|	CHAR_LITERAL
		|	COMMENT
		|	.
		)*
		'}'
	;

protected
ID
options {
	testLiterals=true;
}
	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;

ASSIGN_RHS
	:	'='!
		(	options {greedy=false;}
		:	STRING_LITERAL
		|	CHAR_LITERAL
		|	NEWLINE
		|	.
		)*
		';'
	;

WS	:	(	options {greedy=true;}
		: 	' '
		|	'\t'
		|	NEWLINE
		)+
		{$setType(Token.SKIP);}
	;

protected
NEWLINE
	:	(	options {
				generateAmbigWarnings=false;
			}
		:	'\r' '\n'	{newline();}
		|	'\r'		{newline();}
		|	'\n'		{newline();}
		)
	;

COMMENT
	:	( SL_COMMENT | ML_COMMENT )
		{$setType(Token.SKIP);}
	;

protected
SL_COMMENT
	:	"//" (options {greedy=false;}:.)* NEWLINE
	;

protected
ML_COMMENT :
	"/*"
	(	options {greedy=false;}
	:	NEWLINE
	|	.
	)*
	"*/"
	;

CHAR_LITERAL
	:	'\'' (ESC|~'\'') '\''
	;

STRING_LITERAL
	:	'"' (ESC|~'"')* '"'
	;

protected
ESC	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'w'
		|	'a'
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
		|	'u' XDIGIT XDIGIT XDIGIT XDIGIT
		)
	;

protected
DIGIT
	:	'0'..'9'
	;

protected
XDIGIT
	:	'0' .. '9'
	|	'a' .. 'f'
	|	'A' .. 'F'
	;

ARG_ACTION
	:	'['
		(
			options {
				greedy=false;
			}
		:	ARG_ACTION
		|	NEWLINE
		|	CHAR_LITERAL
		|	STRING_LITERAL
		|	.
		)*
		']'
	;

ACTION
	:	'{'
		(
			options {
				greedy=false;
			}
		:	NEWLINE
		|	ACTION
		|	CHAR_LITERAL
		|	COMMENT
		|	STRING_LITERAL
		|	.
		)*
		'}'
   ;
