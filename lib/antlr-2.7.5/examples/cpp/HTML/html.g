header {
#include <iostream>
}

/*	
	Based on the HTML 3.2 spec. by the W3 (http://www.w3.org)
	Alexander Hinds & Terence Parr
	Magelang Institute, Ltd.
	Send comments to:  parrt@jguru.com

	v1.1	Terence Parr (updated to 2.6.0)

	Fixed CCYTE->CCITE
	Fixed def of COMMENT_DATA so it scarfs stuff correctly.
	Also, fixed refs to (PCDATA)? -> (PCDATA)* because a comment
		between PCDATA returns 2 PCDATA--ya need the loop not optional.

	v1.0	Terence John Parr (version 2.5.0 of ANTLR required)

	Fixed how whitespace as handled, removing some ambiguities; some
	because of ANTLR lexical filtering in 2.5.0.

	Changed (PCDATA)* loops to (PCDATA)? general since PCDATA matches
	everything between valid tags (how could there be more than one
	between tags?)

	Made the DOCTYPE optional.

	Reduced lookahead from k=5 to k=1 on the parser and number
	of parser ambiguities to 2.  Reduced lexer lookahead from 6
	to 4; had to left factor a bunch of stuff.

	List items couldn't contain nested lists...fixed it.

	Fixed def of WORD so it can't be an INT.  Removed '-' from WORD.

	Fixed HEXNUM so it will allow letters A..F.

	KNOWN ISSUES:

	1.  Does not handle "staggered" tags, eg: <p> <i> <p> <i>

	2.  Adhere's somewhat strictly to the html spec, so many pages
	won't parse without errors.

	3.  Doesn't convert &(a signifier) to it's proper single char 
	representation

	4.  Checks only the syntax of element attributes, not the semantics,
	e.g. won't very that a base element's attribute is actually
	called "href" 

	5.  Tags split across lines, for example, <A (NEWLINE) some text >
	won't be properly recognized.  TJP: I think I fixed this.

	7.  Lines not counted properly due to the def'n of PCDATA - see the
	alternate def'n for a possible fix.  TJP: I think I fixed this.

*/

options {
	language="Cpp";
}

class HTMLParser extends Parser;
options {
	exportVocab=HTML;
	k = 1;
}


document
	: 	(PCDATA)? (DOCTYPE (PCDATA)?)?
		(OHTML (PCDATA)?)?
		(head)?
		(body)?
		(CHTML (PCDATA)?)?
	;

head: 	(OHEAD (PCDATA)?)?
		head_element
		(PCDATA | head_element)* 
		(CHEAD (PCDATA)?)? 
	;	

head_element
	:	title	//bug need at least a title, rest optional
	|	script
	|	style
	|	ISINDEX
	|	BASE
	|	META
	|	LINK
	;

title
	:	OTITLE (PCDATA)? CTITLE
	;

script
	:	OSCRIPT (~CSCRIPT)+ CSCRIPT
	;

style
	:	OSTYLE (~CSTYLE)+ CSTYLE
	;

body: 	( OBODY (PCDATA)* )? 
		body_content_no_PCDATA
		( body_content )+ 
		( CBODY (PCDATA)* )? 
	;	

body_content_no_PCDATA
	:	body_tag | text_tag
	;

body_tag
	: 	heading | block | ADDRESS
	;

body_content
	: 	body_tag | text
	;


/*revised*/
heading
	:	h1 | h2 | h3 | h4 | h5 | h6
	;

block
	:	paragraph | list | preformatted | div |
		center | blockquote | HR | table
	;	//bug - ?FORM v %form, ISINDEX here too?

font:	teletype | italic | bold | underline | strike | 
		big | small | subscript | superscript
	;

phrase
	:	emphasize | strong | definition | code | sample_output|
		keyboard_text | variable | citation
	;
	
special
	:	anchor | IMG | applet | font_dfn | BFONT |
		map | BR 
	;

text_tag
	:	font | phrase | special | form
	;

text:	PCDATA | text_tag
	;

/*end*/


/*BLOCK ELEMENTS*/

h1	:	OH1 (block | text)* CH1
	;
h2	:	OH2 (block | text)* CH2
	;
h3	:	OH3 (block | text)* CH3
	;
h4	:	OH4 (block | text)* CH4
	;
h5	:	OH5 (block | text)* CH5
	;
h6	:	OH6 (block | text)* CH6
	;

address
	:	OADDRESS (PCDATA)* CADDRESS
	;

//NOTE:  according to the standard, paragraphs can't contain block elements
//like HR.  Netscape may insert these elements into paragraphs.
//We adhere strictly here.

paragraph
	:	OPARA
		(
			/*	Rule body_content may also be just plain text because HTML is
				so loose.  When body puts body_content in a loop, ANTLR
				doesn't know whether you want it to match all the text as part
				of this paragraph (in the case where the </p> is missing) or
				if the body rule should scarf it.  This is analogous to the
				dangling-else clause.  I shut off the warning.
			*/
			options {
				generateAmbigWarnings=false;
			}
		:	text
		)*
		(CPARA)?	
	;

list:	unordered_list
	|	ordered_list
	|	def_list
	;

unordered_list
	:	OULIST (PCDATA)* (list_item)+ CULIST
	;

ordered_list
	:	OOLIST (PCDATA)* (list_item)+ COLIST
	;

def_list
	:	ODLIST (PCDATA)* (def_list_item)+ CDLIST 
	;

list_item
	:	OLITEM ( text | list )+ (CLITEM (PCDATA)*)?
	;
	
def_list_item
	:	dt | dd
	;

dt	:	ODTERM (text)+ CDTERM (PCDATA)*
	;

dd	:	ODDEF (text | block)+ CDTERM (PCDATA)*
	;

dir	:	ODIR (list_item)+ CDIR
	;

menu:	OMENU (list_item)+ CMENU
	;

preformatted
	:	OPRE (text)+ CPRE
	;

div	:	ODIV (body_content)* CDIV		//semi-revised
	;

center
	:	OCENTER (body_content)* CCENTER //semi-revised
	;

blockquote
	:	OBQUOTE (body_content)* CBQUOTE
	;

form:	OFORM (form_field | body_content)* CFORM
	;

table
	:	OTABLE (caption)? (PCDATA)* (tr)+ CTABLE
	;

caption
	:	OCAP (text)* CCAP	
	;

tr	:	O_TR (PCDATA)* (th_or_td)* (C_TR (PCDATA)*)? 
	;

th_or_td
	:	O_TH_OR_TD (body_content)* (C_TH_OR_TD (PCDATA)*)?
	;

/*TEXT ELEMENTS*/

/*font style*/

teletype
	:	OTTYPE ( text )+ CTTYPE
	;

italic
	:	OITALIC ( text )+ CITALIC
	;

bold:	OBOLD ( text )+ CBOLD
	;

underline
	:	OUNDER ( text )+ CUNDER
	;

strike
	:	OSTRIKE ( text )+ CSTRIKE
	;

big	:	OBIG ( text )+ CBIG
	;

small
	:	OSMALL ( text )+ CSMALL
	;

subscript
	:	OSUB ( text )+ CSUB
	;

superscript
	:	OSUP ( text )+ CSUP
	;

	/*phrase elements*/

emphasize
	:	OEM ( text )+ CEM
	;

strong
	:	OSTRONG ( text )+ CSTRONG
	;

definition
	:	ODFN ( text )+ CDFN
	;

code
	:	OCODE ( text )+ CCODE
	;

sample_output
	:	OSAMP ( text )+ CSAMP
	;

keyboard_text
	:	OKBD ( text )+ CKBD
	;

variable
	:	OVAR ( text )+ CVAR
	;

citation
	:	OCITE ( text )+ CCITE
	;

/*	form fields (combined with body_content elsewhere so no PCDATA on end) */
form_field
	:	INPUT | select | textarea
	;

select
	:	OSELECT (PCDATA)* (select_option)+ CSELECT
	;

select_option
	:	SELOPT (PCDATA)*
	;

textarea
	:	OTAREA (PCDATA)* CTAREA
	;

/*	special text level elements*/
anchor
	:	OANCHOR (text)* CANCHOR
	;

applet
	:	OAPPLET (APARAM)? (PCDATA)* CAPPLET
	;

//not w3-no blocks allowed; www.microsoft.com uses
font_dfn
	:	OFONT (text)* CFONT	
	;

map	:	OMAP (AREA)+ CMAP
	;

class HTMLLexer extends Lexer;
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
options {
ignore=WS_;
}
	: "<!doctype" "html" "public" STRING '>'
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
	:	"<body" (WS_ (ATTR )*)? '>' 
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
 	: 	"<isindex" WS_ ATTR '>'
	;

META
	: 	"<meta" WS_ (ATTR)+ '>'
	;

LINK
	:	"<link" WS_ (ATTR)+ '>'	
	;


/* headings */

OH1	:	"<h1" (WS_ ATTR)? '>' 
	;

CH1	:	"</h1>" 
	;

OH2	:	"<h2" (WS_ ATTR)?'>' 
	;

CH2	:	"</h2>" 
	;

OH3	:	"<h3" (WS_ ATTR)? '>' 
	;

CH3	:	"</h3>" 
	;

OH4	:	"<h4" (WS_ ATTR)? '>' 
	;

CH4	:	"</h4>" 
	;

OH5	:	"<h5" (WS_ ATTR)? '>' 
	;

CH5	:	"</h5>" 
	;

OH6	:	"<h6" (WS_ ATTR)? '>' 
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
	:	"<p" (WS_ ATTR)? '>' 
	;

CPARA
	: 	"</p>"		//it's optional
	;

		/*UNORDERED LIST*/
OULIST
	:	"<ul" (WS_ ATTR)? '>' 
	;

CULIST
	:	"</ul>"
	;

		/*ORDERED LIST*/
OOLIST
	:	"<ol" (WS_ ATTR)? '>'
	;

COLIST
	:	"</ol>"
	;

		/*LIST ITEM*/

OLITEM
	:	"<li" (WS_ ATTR)? '>'
	;

CLITEM
	:	"</li>"
	;

		/*DEFINITION LIST*/ 

ODLIST 
	:	"<dl" (WS_ ATTR)? '>' 
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

ODIV:	"<div" (WS_ ATTR)? '>'
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

HR	:	"<hr" (WS_ (ATTR)*)? '>'
	;


OTABLE	
	:	"<table" (WS_ (ATTR)*)? '>'
	;

CTABLE
	: 	"</table>"
	;

OCAP:	"<caption" (WS_ (ATTR)*)? '>'
	;

CCAP:	"</caption>"
	;

O_TR
	:	"<tr" (WS_ (ATTR)*)? '>'
	;

C_TR:	"</tr>"
	;

O_TH_OR_TD
	:	("<th" | "<td") (WS_ (ATTR)*)? '>'
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

/** Left-factor <strike> and <strong> to reduce lookahead */
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
	:	"<input" (WS_ (ATTR)*)? '>'
	;

OSELECT
	:	"<select" (WS_ (ATTR)*)? '>'
	;

CSELECT
	:	"</select>"
	;

OTAREA
	:	"<textarea" (WS_ (ATTR)*)? '>'
	;

CTAREA
	:	"</textarea>"
	;

SELOPT	
	:	"<option" (WS_ (ATTR)*)? '>' 
	;

/* special text level elements*/

OANCHOR
	:	"<a" WS_ (ATTR)+ '>'
	;

CANCHOR
	:	"</a>"
	;	

IMG	:	"<img" WS_ (ATTR)+ '>'
	;


OAPPLET
	:	"<applet" WS_ (ATTR)+ '>'
	;

CAPPLET
	:	"</applet>"
	;

APARM
	:	"<param" WS_ (ATTR)+'>'
	;	

OFORM
	:	"<form" WS_ (ATTR)+ '>'
	;

OFONT	
	:	"<font" WS_ (ATTR)+ '>'
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
		(	"font" WS_ ATTR {$setType(BFONT);}
		|	WS_ ATTR        {$setType(BASE);}
		)
		'>'
	;

/*
BFONT	
	:	"<basefont" WS_ ATTR '>'
	;

BASE: 	"<base" WS_ ATTR '>'
	;
*/

BR
	:	"<br" (WS_ ATTR)? '>'
	;

OMAP
	:	"<map" WS_ ATTR '>'
	; 

CMAP:	"</map>"
	;

AREA:	"<area" WS_ (ATTR)+ '>'
	;

/*MISC STUFF*/

PCDATA
	:	(
			/* See comment in WS_.  Language for combining any flavor
			 * newline is ambiguous.  Shutting off the warning.
			 */
			options {
				generateAmbigWarnings=false;
			}
		:	'\r' '\n'		{newline();}
		|	'\r'			{newline();}
		|	'\n'			{newline();}
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
			{LA(2)!='-' && LA(3)!='>'}? '-' // allow '-' if not "-->"
		|	'\r' '\n'		{newline();}
		|	'\r'			{newline();}
		|	'\n'			{newline();}
		|	~('-'|'\n'|'\r')
		)*
	;


COMMENT
	:	"<!--" COMMENT_DATA "-->"	{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
	;

/*
	PROTECTED LEXER RULES
*/

protected
WS_	:	(
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
		|	'\n'	{ newline(); }
		|	"\r\n"	{ newline(); }
		|	'\r'	{ newline(); }
		)+
	;

protected
ATTR
options {
ignore=WS_;
}
	:	WORD ('=' (WORD ('%')? | ('-')? INT | STRING | HEXNUM))?
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
			{ newline();}
		)*
		{std::cerr << "invalid tag: " << $getText << std::endl;}
	|	( "\r\n" | '\r' | '\n' ) {newline();}
	|	.
	;

/*
	:	('<'  { std::cerr << "Warning: non-standard tag <" << LA(1); } )
		(~'>' { std::cerr << LA(1); } )* 
		('>'  { std::cerr << " skipped." << std::endl; } ) 
		{ _ttype = ANTLR_USE_NAMESPACE(antlr::)Token::SKIP; }
	;
*/
