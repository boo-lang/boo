/* ANTLR Translator Generator
 * Project led by Terence Parr at http://www.jGuru.com
 * Software rights: http://www.antlr.org/license.html
 *
 * $Id: //depot/code/org.antlr/release/antlr-2.7.5/antlr/tokdef.g#1 $
 */

header { package antlr; }

/** Simple lexer/parser for reading token definition files
  in support of the import/export vocab option for grammars.
 */
class ANTLRTokdefParser extends Parser;
options {
	k=3;
	interactive=true;
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

file [ImportVocabTokenManager tm] :
	name:ID
	(line[tm])*;

line [ImportVocabTokenManager tm]
{ Token t=null; Token s=null; }
	:	(	s1:STRING {s = s1;}
		|	lab:ID {t = lab;} ASSIGN s2:STRING {s = s2;}
		|	id:ID {t=id;} LPAREN para:STRING RPAREN
		|	id2:ID {t=id2;}
		)
		ASSIGN
		i:INT
		{
		Integer value = Integer.valueOf(i.getText());
		// if literal found, define as a string literal
		if ( s!=null ) {
			tm.define(s.getText(), value.intValue());
			// if label, then label the string and map label to token symbol also
			if ( t!=null ) {
				StringLiteralSymbol sl =
					(StringLiteralSymbol) tm.getTokenSymbol(s.getText());
				sl.setLabel(t.getText());
				tm.mapToTokenSymbol(t.getText(), sl);
			}
		}
		// define token (not a literal)
		else if ( t!=null ) {
			tm.define(t.getText(), value.intValue());
			if ( para!=null ) {
				TokenSymbol ts = tm.getTokenSymbol(t.getText());
				ts.setParaphrase(
					para.getText()
				);
			}
		}
		}
	;

class ANTLRTokdefLexer extends Lexer;
options {
	k=2;
	testLiterals=false;
	interactive=true;
	charVocabulary='\003'..'\377';
}

WS	:	(	' '
		|	'\t'
		|	'\r' ('\n')?	{newline();}
		|	'\n'		{newline();}
		)
		{ _ttype = Token.SKIP; }
	;

SL_COMMENT :
	"//"
	(~('\n'|'\r'))* ('\n'|'\r'('\n')?)
	{ _ttype = Token.SKIP; newline(); }
	;

ML_COMMENT :
   "/*"
   (
			'\n' { newline(); }
		|	'*' ~'/'
		|	~'*'
	)*
	"*/"
	{ _ttype = Token.SKIP; }
	;

LPAREN : '(' ;
RPAREN : ')' ;

ASSIGN : '=' ;

STRING
	:	'"' (ESC|~'"')* '"'
	;

protected
ESC	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('0'..'3') ( DIGIT (DIGIT)? )?
		|	('4'..'7') (DIGIT)?
		|	'u' XDIGIT XDIGIT XDIGIT XDIGIT
		)
	;

protected
DIGIT
	:	'0'..'9'
	;

protected
XDIGIT :
		'0' .. '9'
	|	'a' .. 'f'
	|	'A' .. 'F'
	;

ID :
	('a'..'z'|'A'..'Z')
	('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;

INT : (DIGIT)+
	;
