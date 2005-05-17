// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header {
package antlr.actions.python;
}

{
import java.io.StringReader;
import antlr.collections.impl.Vector;
import antlr.*;
}

class CodeLexer extends Lexer;
options {
	k=2;
	charVocabulary='\3'..'\377';
	testLiterals=false;
	interactive=true;
}

{
	protected int lineOffset = 0;
	private Tool antlrTool;	// The ANTLR tool

 	public CodeLexer ( 
        String s,
        String fname,
        int line,
        Tool tool
    )
    {
		this(new StringReader(s));
        setLine(line);
        setFilename(fname);
        this.antlrTool = tool;
	}

	public void setLineOffset(int lineOffset) {
		setLine(lineOffset);
	}

	public void reportError(RecognitionException e)
	{
		antlrTool.error(
            "Syntax error in action: "+e,
            getFilename(),getLine(),getColumn());
	}

	public void reportError(String s)
	{
		antlrTool.error(s,getFilename(),getLine(),getColumn());
	}

	public void reportWarning(String s)
	{
		if ( getFilename()==null ) {
			antlrTool.warning(s);
		}
		else {
			antlrTool.warning(s,getFilename(),getLine(), getColumn());
		}
	}
}

// rules are protected because we don't care about nextToken().

public
ACTION
	:	( STUFF )*
	;

// stuff in between #(...) and #id items
protected
STUFF
	:	COMMENT
	|	"\r\n" 		{ newline(); }
	|	'\r' 		{ newline(); }
	|	'\n'		{ newline(); }
	|	'/'	~('/'|'*')	// non-comment start '/'
	|	~('/'|'\n'|'\r')
	;

protected
COMMENT
	:	SL_COMMENT
	|	ML_COMMENT
	;

protected
SL_COMMENT
	:   "//"! {
            /* rewrite comment symbol */
            $append("#");
        } 

        (
            options {greedy=false;}:.
        )*

        ('\n'|"\r\n"|'\r')
		{
            newline();
        }
	;

protected
IGNWS
    : 
        (   ' '
        |   '\t'
        )*
    ;

protected
ML_COMMENT 
{
    int offset = 0;
}
    :
        "/*"!
        {
            /* rewrite comment symbol */
            $append("#");
        }
        
        (	
            options {greedy=false;}
        :	'\r' '\n' IGNWS!	{
                newline();
                $append("# ");
            }
        |	'\r' IGNWS! {
                newline();
                $append("# ");
            }
        |	'\n' IGNWS! {
                newline();
                $append("# ");
            }
        |	.
        )*
        {
            /* force a newline */
            $append("\n");
        }
        "*/"!
	;
