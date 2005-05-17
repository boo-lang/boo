// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header "html_l.__main__"
{   
    L = Lexer()

    token = L.nextToken()
    while not token.isEOF():
        print token
        token = L.nextToken()
}

options {
    language=Python;
}

class html_l extends Lexer;
options {	
	k = 4;
	exportVocab=HTML;
	charVocabulary = '\3'..'\377';
	caseSensitive=false;
	filter=UNDEFINED_TOKEN;
}


/*	STRUCTURAL tags
*/

DOCTYPE 
	: "<!doctype" WS "html" WS "public" (WS)? STRING (WS)? (STRING (WS)?)? '>'
	;

OHTML
 	: 	"<html>"
	; 

CHTML
	: 	"</html>"
	;

OHEAD
	: 	"<head>"
	;

CHEAD
	: 	"</head>"
	;

OBODY
	:	"<body" (WS (ATTR )*)? '>' 
	;

CBODY
	:	"</body>"
	;


/*	HEAD ELEMENTS
*/

OTITLE
	: "<title>"
	;

CTITLE
	: "</title>"
	;


OSCRIPT
	: 	"<script>" 
	;

CSCRIPT
	:	"</script>"
	;

ISINDEX
 	: 	"<isindex" WS ATTR '>'
	;

META
	: 	"<meta" WS (ATTR)+ '>'
	;

LINK
	:	"<link" WS (ATTR)+ '>'	
	;


/* headings */

OH1	:	"<h1" (WS ATTR)? '>' 
	;

CH1	:	"</h1>" 
	;

OH2	:	"<h2" (WS ATTR)?'>' 
	;

CH2	:	"</h2>" 
	;

OH3	:	"<h3" (WS ATTR)? '>' 
	;

CH3	:	"</h3>" 
	;

OH4	:	"<h4" (WS ATTR)? '>' 
	;

CH4	:	"</h4>" 
	;

OH5	:	"<h5" (WS ATTR)? '>' 
	;

CH5	:	"</h5>" 
	;

OH6	:	"<h6" (WS ATTR)? '>' 
	;

CH6	:	"</h6>" 
	;

OADDRESS
	:	"<address>" 
	;

CADDRESS
	:	"</address>"
	;

OPARA
	:	"<p" (WS ATTR)? '>' 
	;

CPARA
	: 	"</p>"		//it's optional
	;

		/*UNORDERED LIST*/
OULIST
	:	"<ul" (WS ATTR)? '>' 
	;

CULIST
	:	"</ul>"
	;

		/*ORDERED LIST*/
OOLIST
	:	"<ol" (WS ATTR)? '>'
	;

COLIST
	:	"</ol>"
	;

		/*LIST ITEM*/

OLITEM
	:	"<li" (WS ATTR)? '>'
	;

CLITEM
	:	"</li>"
	;

		/*DEFINITION LIST*/ 

ODLIST 
	:	"<dl" (WS ATTR)? '>' 
	;

CDLIST
	:	"</dl>"
	;

ODTERM
	: 	"<dt>"
	;

CDTERM
	: 	"</dt>"
	;

ODDEF
	: 	"<dd>"
	;

CDDEF
	: 	"</dd>"
	;

ODIR:	"<dir>"
	;

CDIR_OR_CDIV
	:	"</di"
		(	'r' {$setType(CDIR);}
		|	'v' {$setType(CDIV);}
		)
		'>'
	;

ODIV:	"<div" (WS ATTR)? '>'
	;

OMENU
	:	"<menu>"
	;

CMENU
	:	"</menu>"
	;

OPRE:	("<pre>" | "<xmp>") ('\n')? 
	;

CPRE:	 "</pre>" | "</xmp>" 
	;

OCENTER
	:	"<center>"
	;

CCENTER
	:	"</center>"
	;

OBQUOTE
	:	"<blockquote>"
	;

CBQUOTE
	:	"</blockquote>"
	;

//this is block element and thus can't be nested inside of
//other block elements, ex: paragraphs.
//Netscape appears to generate bad HTML vis-a-vis the standard.

HR	:	"<hr" (WS (ATTR)*)? '>'
	;


OTABLE	
	:	"<table" (WS (ATTR)*)? '>'
	;

CTABLE
	: 	"</table>"
	;

OCAP:	"<caption" (WS (ATTR)*)? '>'
	;

CCAP:	"</caption>"
	;

O_TR
	:	"<tr" (WS (ATTR)*)? '>'
	;

C_TR:	"</tr>"
	;

O_TH_OR_TD
	:	("<th" | "<td") (WS (ATTR)*)? '>'
	;

C_TH_OR_TD
	:	"</th>" | "</td>"
	;

/*	PCDATA-LEVEL ELEMENTS
*/

/*		font style elemens*/
	
OTTYPE
	:	"<tt>"
	;

CTTYPE
	:	"</tt>"
	;

OITALIC
	:	"<i>"
	;

CITALIC
	:	"</i>"
	;

OBOLD
 	:	"<b>" 
	;

CBOLD
	:	"</b>" 
	;

OUNDER
	:	"<u>"
	;

CUNDER
	:	"</u>" 
	;

/* Left-factor <strike> and <strong> to reduce lookahead */
OSTRIKE_OR_OSTRONG
	:	"<str"
		(	"ike" {$setType(OSTRIKE);}
		|	"ong" {$setType(OSTRONG);}
		)
		'>'
	;

CST_LEFT_FACTORED
	:	"</st"
		(	"rike" {$setType(CSTRIKE);}
		|	"rong" {$setType(CSTRONG);}
		|	"yle"  {$setType(CSTYLE);}
		)
		'>'
	;

OSTYLE
 	: 	"<style>" 
	;

OBIG:	"<big>"
	;

CBIG:	"</big>"
	;

OSMALL
	:	"<small>"
	;

CSMALL
	:	"</small>"
	;

OSUB:	"<sub>"
	;

OSUP:	"<sup>"
	;

CSUB_OR_CSUP
	:	"</su"
		(	'b' {$setType(CSUB);}
		|	'p' {$setType(CSUP);}
		)
		'>'
	;

/*		phrase elements*/
OEM	:	"<em>"
	;

CEM	:	"</em>"
	;

ODFN:	"<dfn>"
	;

CDFN:	"</dfn>"
	;

OCODE
 	:	"<code>" 
	;

CCODE
	:	"</code>"
	;

OSAMP
	:	"<samp>"
	;

CSAMP
	:	"</samp>"
	;

OKBD:	"<kbd>"
	;

CKBD:	"</kbd>"
	;

OVAR:	"<var>"
	;

CVAR:	"</var>"
	;

OCITE
	:	"<cite>"
	;

CCITE
	:	"</cite>"
	;

/* form fields*/
INPUT	
	:	"<input" (WS (ATTR)*)? '>'
	;

OSELECT
	:	"<select" (WS (ATTR)*)? '>'
	;

CSELECT
	:	"</select>"
	;

OTAREA
	:	"<textarea" (WS (ATTR)*)? '>'
	;

CTAREA
	:	"</textarea>"
	;

SELOPT	
	:	"<option" (WS (ATTR)*)? '>' 
	;

/* special text level elements*/

OANCHOR
	:	"<a" WS (ATTR)+ '>'
	;

CANCHOR
	:	"</a>"
	;	

IMG	:	"<img" WS (ATTR)+ '>'
	;


OAPPLET
	:	"<applet" WS (ATTR)+ '>'
	;

CAPPLET
	:	"</applet>"
	;

APARM
	:	"<param" WS (ATTR)+ '>'
	;	

OFORM
	:	"<form" WS (ATTR)+ '>'
	;

OFONT	
	:	"<font" WS (ATTR)+ '>'
	;

CFORM_OR_CFONT
	:	"</fo"
		(	"rm" {$setType(CFORM);}
		|	"nt" {$setType(CFONT);}
		)
		'>'
	;

/*
CFORM
	:	"</form>"
	;	

CFONT
	:	"</font>"
	;
*/

BFONT_OR_BASE
	:	"<base"
		(	"font" WS ATTR {$setType(BFONT);}
		|	WS ATTR        {$setType(BASE);}
		)
		'>'
	;

/*
BFONT	
	:	"<basefont" WS ATTR '>'
	;

BASE: 	"<base" WS ATTR '>'
	;
*/

BR
	:	"<br" (WS ATTR)? '>'
	;

OMAP
	:	"<map" WS ATTR '>'
	; 

CMAP:	"</map>"
	;

AREA:	"<area" WS (ATTR)+ '>'
	;

/*MISC STUFF*/

PCDATA
	:	(
			/* See comment in WS.  Language for combining any flavor
			 * newline is ambiguous.  Shutting off the warning.
			 */
			options {
				generateAmbigWarnings=false;
			}
		:	'\r' '\n'		{$newline;}
		|	'\r'			{$newline;}
		|	'\n'			{$newline;}
		|	~('<'|'\n'|'\r'|'"'|'>')
		)+ 
	;

// multiple-line comments
protected
COMMENT_DATA
	:	(	/*	'\r' '\n' can be matched in one alternative or by matching
				'\r' in one iteration and '\n' in another.  I am trying to
				handle any flavor of newline that comes in, but the language
				that allows both "\r\n" and "\r" and "\n" to all be valid
				newline is ambiguous.  Consequently, the resulting grammar
				must be ambiguous.  I'm shutting this warning off.
			 */
			options {
				generateAmbigWarnings=false;
			}
		:
			{ not (self.LA(2)=='-' and self.LA(3)=='>')}? '-' // allow '-' if not "-->"
		|	'\r' '\n'		{$newline;}
		|	'\r'			{$newline;}
		|	'\n'			{$newline;}
		|	~('-'|'\n'|'\r')
		)*
	;


COMMENT
	:	"<!--" c:COMMENT_DATA "-->" (WS)?
		{ $setType(SKIP); }
	;

/*
	PROTECTED LEXER RULES
*/

protected
WS	:	(
			/*	'\r' '\n' can be matched in one alternative or by matching
				'\r' in one iteration and '\n' in another.  I am trying to
				handle any flavor of newline that comes in, but the language
				that allows both "\r\n" and "\r" and "\n" to all be valid
				newline is ambiguous.  Consequently, the resulting grammar
				must be ambiguous.  I'm shutting this warning off.
			 */
			options {
				generateAmbigWarnings=false;
			}
		:	' '
		|	'\t'
		|	'\n'	{ $newline; }
		|	"\r\n"	{ $newline; }
		|	'\r'	{ $newline; }
		)+
	;

protected
ATTR
	:       WORD (WS)? ('=' (WS)? (WORD ('%')? | ('-')? INT | STRING | HEXNUM) (WS)?)? 
	;

//don't need uppercase for case-insen.
//the '.' is for words like "image.gif"
protected
WORD:	(	LCLETTER
		|	'.'
		)

		(
			/*	In reality, a WORD must be followed by whitespace, '=', or
				what can follow an ATTR such as '>'.  In writing this grammar,
				however, we just list all the possibilities as optional
				elements.  This is loose, allowing the case where nothing is
				matched after a WORD and then the (ATTR)* loop means the
				grammar would allow "widthheight" as WORD WORD or WORD, hence,
				an ambiguity.  Naturally, ANTLR will consume the input as soon
				as possible, combing "widthheight" into one WORD.

				I am shutting off the ambiguity here because ANTLR does the
				right thing.  The exit path is ambiguous with ever
				alternative.  The only solution would be to write an unnatural
				grammar (lots of extra productions) that laid out the
				possibilities explicitly, preventing the bogus WORD followed
				immediately by WORD without whitespace etc...
			 */
			options {
				generateAmbigWarnings=false;
			}
		:	LCLETTER
		|	DIGIT
		|	'.'
		)+
	;

protected
STRING
	:	'"' (~'"')* '"'
	|	'\'' (~'\'')* '\''
	;

protected
WSCHARS
	:	' ' | '\t' | '\n' | '\r'
	;

protected 
SPECIAL
	:	'<' | '~'
	;
	
protected
HEXNUM
	:	'#' HEXINT
	;

protected
INT	:	(DIGIT)+
	;

protected
HEXINT
	:	(
			/*	Technically, HEXINT cannot be followed by a..f, but due to our
				loose grammar, the whitespace that normally would follow this
				rule is optional.  ANTLR reports that #4FACE could parse as
				HEXINT "#4" followed by WORD "FACE", which is clearly bogus.
				ANTLR does the right thing by consuming a much input as
				possible here.  I shut the warning off.
			 */
			 options {
				generateAmbigWarnings=false;
			}
		:	HEXDIGIT
		)+
	;

protected
DIGIT
	:	'0'..'9'
	;

protected
HEXDIGIT
	:	'0'..'9'
	|	'a'..'f'
	;

protected
LCLETTER
	:	'a'..'z'
	;	

protected
UNDEFINED_TOKEN
	:	'<' (~'>')* '>'
		(
			(	/* the usual newline hassle: \r\n can be matched in alt 1
				 * or by matching alt 2 followed by alt 3 in another iteration.
				 */
				 options {
					generateAmbigWarnings=false;
				}
			:	"\r\n" | '\r' | '\n'
			)
			{ $newline;}
		)*
		{ print "invalid tag: "+ $getText;}
	|	( "\r\n" | '\r' | '\n' ) {$newline;}
	|	.
	;
