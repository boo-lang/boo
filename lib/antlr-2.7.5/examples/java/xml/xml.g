/*
Rudimentary lexer grammar for a non-validating XML parser.
Lexer is not intended to be used by parser, but is standalone.
Use something like

            while ( lexer.nextToken().getType() != Token.EOF_TYPE );

to iterate through tokens.

Replace print statements (only there to make something visible) with your 
own code and have fun.

Limitations:
- internal DTD is parsed but not processed
- only supported encoding is iso-8859-1 aka extended ASCII aka ISO-latin-1
- special entity references (like &amp; &lt;) do not get resolved (to '&', '<')
- uses SAX attribute implementation (could easily be dropped)
  [TJP: commented out so it compiles w/o SAX.]
- probably many more 

The good thing about some of these limitations is, that the parsed XML
can be written *literally* unmodified.

Author: Olli Z. (oliver@zeigermann.de)

Initial date: 07.02.1999 (02/07/99)
Complete revision: 16.01.2003 (01/16/03)

Developed and testes with ANTLR 2.7.2
*/
header {
    // import org.xml.sax.helpers.*;
}

class XMLLexer extends Lexer;
options {
    // needed to tell "<!DOCTYPE..."
    // from "<?..." and "<tag..." and "</tag...>" and "<![CDATA...>"
    // also on exit branch "]]>", "-->"
	k=3;
	charVocabulary = '\3'..'\377'; // extended ASCII (3-255 in octal notation)
	caseSensitive=true;
}

DOCTYPE!
    :
        "<!DOCTYPE" WS rootElementName:NAME 
        { System.out.println("ROOTELEMENT: "+rootElementName.getText()); }   
        WS
        ( 
            ( "SYSTEM" WS sys1:STRING
                { System.out.println("SYSTEM: "+sys1.getText()); }   
                
            | "PUBLIC" WS pub:STRING WS sys2:STRING
                { System.out.println("PUBLIC: "+pub.getText()); }   
                { System.out.println("SYSTEM: "+sys2.getText()); }   
            )
            ( WS )?
        )?
        ( dtd:INTERNAL_DTD ( WS )? 
            { System.out.println("DTD: "+dtd.getText()); }   

        )?
		'>'
	;

protected INTERNAL_DTD
    :
        '['!
        // reports warning, but is absolutely ok (checked generated code)
        // besides this warning was not generated with k=1 which is 
        // enough for this rule...
        ( options {greedy=false;} : NL
        | STRING // handle string specially to avoid to mistake ']' in string for end dtd
        | .
        )*
        ']'!
    ;

PI! 
    :
        // { AttributesImpl attributes = new AttributesImpl(); }
        "<?" 
        target:NAME
        ( WS )?
		( ATTR /*[attributes]*/ ( WS )? )*
        {
            if (target.getText().equalsIgnoreCase("xml")) {
                // this is the xml declaration, handle it
                System.out.println("XMLDECL: "+target.getText());
            } else {
                System.out.println("PI: "+target.getText());
            }
        }
		"?>"
	;

//////////////////

COMMENT!
	:	"<!--" c:COMMENT_DATA "-->"
        { System.out.println("COMMENT: "+c.getText()); }
	;

protected COMMENT_DATA
    : 
        ( options {greedy=false;} : NL
        | .
        )*
    ;

//////////////////

ENDTAG! :
        "</" g:NAME ( WS )? '>'
        { System.out.println("ENDTAG: "+g.getText()); }
	;

//////////////////

STARTTAG! : 
        // XXX should org.xml.sax.AttributesImpl be replaced by something else?
        // { AttributesImpl attributes = new AttributesImpl(); }
        '<' 
        g:NAME
        ( WS )?
		( ATTR /*[attributes]*/ ( WS )? )*
		( "/>"
            { System.out.println("EMTYTAG: "+g.getText()); }
		| '>'
            { System.out.println("STARTTAG: "+g.getText()); }
		)
	;

PCDATA!	: 
        p:PCDATA_DATA
        { System.out.println("PCDATA: "+p.getText()); }
	;

protected PCDATA_DATA
	: 
        ( options {greedy=true;} : NL
        | ~( '<' | '\n' | '\r' )
        )+
    ;

CDATABLOCK!
	: "<![CDATA[" p:CDATA_DATA "]]>"
        { System.out.println("CDATABLOCK: "+p.getText()); }
	;

protected CDATA_DATA
    : 
        ( options {greedy=false;} : NL
        | .
        )*
    ;

protected ATTR // [AttributesImpl attributes]
	:	name:NAME ( WS )? '=' ( WS )? value:STRING_NO_QUOTE
        /*
		{ attributes.addAttribute("", "", name.getText(), "CDATA", 
                value.getText()); 
        }
        */
        { System.out.println("ATTRIBUTE: "+name.getText()+"="+value.getText()); }
	;

protected STRING_NO_QUOTE
	:	'"'! (~'"')* '"'!
	|	'\''! (~'\'')* '\''!
	;

protected STRING
	:	'"' (~'"')* '"'
	|	'\'' (~'\'')* '\''
	;

protected NAME
	:	( LETTER | '_' | ':') ( options {greedy=true;} : NAMECHAR )*
	;

protected NAMECHAR
	: LETTER | DIGIT | '.' | '-' | '_' | ':'
	;

protected DIGIT
	:	'0'..'9'
	;

protected LETTER
	: 'a'..'z' 
	| 'A'..'Z'
	;

protected WS
	:	(	options {
                greedy = true;
			}
		:	' '
		|	ESC
		)+
	;

protected ESC
	: ( '\t'
	 	|	NL
		)
	;

// taken from html.g
// Alexander Hinds & Terence Parr
// from antlr 2.5.0: example/html 
//
// '\r' '\n' can be matched in one alternative or by matching
// '\r' in one iteration and '\n' in another.  I am trying to
// handle any flavor of newline that comes in, but the language
// that allows both "\r\n" and "\r" and "\n" to all be valid
// newline is ambiguous.  Consequently, the resulting grammar
// must be ambiguous.  I'm shutting this warning off.
protected NL
    : (	options {
	generateAmbigWarnings=false;
	greedy = true;
    }
		: '\n'
		|	"\r\n"
		|	'\r'
		)
		{ newline(); }
	;
