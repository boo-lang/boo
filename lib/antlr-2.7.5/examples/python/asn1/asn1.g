// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header {

    // import language specific stuff
    // need to import my local module defining super classes etc.
    import asn1
}
options {
	language="Python";
}

class asn1_l extends Lexer("asn1.CharScanner");

options {
    k = 3;
    charVocabulary = '\3'..'\377';
    caseSensitive=true;
    testLiterals = true;
    codeGenMakeSwitchThreshold = 2;
    codeGenBitsetTestThreshold = 2;
    importVocab=ASN1;
}

tokens {
    DOTDOT;
    ELLIPSIS;
}

ASSIGN_OP           :   "::="   ;
BAR                 :   '|'     ;
COLON               :   ':'     ;
COMMA               :   ','     ;
DOT                 :   '.'     ;
DOTDOT              :   ".."    ;
ELLIPSIS            :   "..."   ;
EXCLAMATION         :   '!'     ;
INTERSECTION        :   '^'     ;
LESS                :   '<'     ;
L_BRACE             :   '{'     ;
L_BRACKET           :   '['     ;
LL_BRACKET          :  { self.state_with_syntax==False }?  "[["    ;
L_PAREN             :   '('     ;
MINUS               :   '-'     ;
PLUS                :   '+'     ;
R_BRACE             :   '}'     ;
R_BRACKET           :   ']'     ;
RR_BRACKET          :  { self.state_with_syntax==False }?  "]]"    ;
R_PAREN             :   ')'     ;
SEMI                :   ';'     ;
AT                  :   '@'     ;


/* These are whitespace (without newline) characters according to X.680:2002 */
protected
WSchr
    : '\t'   // horizontal tab (HT)  '\t' 0x09  9
    | ' '   // space          (SP)  ' '  0x20 32
    ;    

/* Same as WSign - just ignore consumed character */
protected
WSign  
    : WSchr { $setText("") }
    ;

/* the end of line */
protected
EOLchr
    :   ( 
            options {
                generateAmbigWarnings = false;
            }
        : '\r''\n'
        | '\r'    
        | '\n'
//        | '\v'   // vertical   tab (VT)       0x0b 11
//        | '\f'   // form feed      (FF)  '\f' 0x0c 12
        )   
        {
            $newline
        }
    ;

/* like EOL but we ignore the consumed symbol */
protected
EOLign
    : EOLchr { 
            $setText("")  
        }
    ;

/* like EOL but we normalize consumed symbol */
protected
EOLnrm
    : EOLchr { $setText("\n")  }
    ;

/* upper (ASCII) case characters */
protected
UPCHR 
    : 'A' .. 'Z' 
    ;

/* lower (ASCII) case characters */
protected
LOCHR 
    : 'a' .. 'z' 
    ;

/* what's a (arabic) digit */
protected
DIGIT 
    : '0' .. '9' 
    ;

/* whats a (roman) letter - yes, the name sucks a bit */
protected
CHR 
    : UPCHR | LOCHR 
    ;

/* what's allowed in an identifier */
protected
IDCHR 
    : CHR | '-' | DIGIT 
    ;


/* a binary digit */
protected
BINCHR      
    :   ('0'|'1') 
    ;

/* a hex digit */
protected
HEXCHR      
    :   ('0'..'9')
    |   ('A'..'F')
    |   ('a'..'f')
    ;

/* a binary string */
protected
BINSTR  
    : "'" (BINCHR|WSign|EOLign)+ "'B" ;

/* a hex string */
protected
HEXSTR  
    : "'" (HEXCHR|WSign|EOLign)+ "'H"  ;

/* escape character in character strings */
protected
CHResc
    : '"' '"' { $setText("\"") }
    ;


/* define which input symbols we can skip (so called whitespace) */
WS          
    : ( WSchr | EOLchr )+   { $skip }
    ;


/* A number is a sequence of digits - note that deliberatly we allow
** here for tokens like '001' etc. 
*/
TOKEN_NUMBER  
    : (DIGIT)+ 
    ;


/* what's an idenifier */
ID  
{ lowchrseen=False}
    : ("BIT" WS "STRING") => "BIT" WS "STRING"        { 
            $setType(TOKEN_BIT_STRING)   
        }
    | ("OCTET" WS "STRING") => "OCTET" WS "STRING"    { 
            $setType(TOKEN_OCTET_STRING) 
        }
    | ("OBJECT" WS "IDENTIFIER") => "OBJECT" WS "IDENTIFIER" {
            $setType(TOKEN_OBJECT_IDENTIFIER)
        }
    | ("ENCODED" WS "BY") => "ENCODED" WS "BY" {
            $setType(TOKEN_ENCODED_BY)
        }
    | ("CONSTRAINED" WS "BY") => "CONSTRAINED" WS "BY" {
            $setType(TOKEN_CONSTRAINED_BY)
        }
    | ("DEFINED" WS "BY") => "DEFINED" WS "BY" {
            $setType(TOKEN_DEFINED_BY)
        }
    | UPCHR ( LOCHR{lowchrseen=True}|UPCHR|DIGIT|'-')* { 
            $setType(TOKEN_Word)
            if lowchrseen: pass
            else: $setType(TOKEN_WORD) 
        }
    |  LOCHR ( IDCHR )* { 
            $setType(TOKEN_word)
        }
    ;   

/* what's a field */
FIELD  
{ lowchrseen=False }
    :  '&' UPCHR ( LOCHR{lowchrseen=True}|UPCHR|DIGIT|'-')* { 
            $setType(TOKEN_Field)
            if lowchrseen: 
              pass
            else: 
              $setType(TOKEN_FIELD)
        }
    |  '&' LOCHR ( IDCHR )* { $setType(TOKEN_field) }
    ;   



/* an octet string is either a bit string or a hex string */
OCTSTR
    : (BINSTR)=>BINSTR { $setType(TOKEN_BSTRING)  }
    | HEXSTR           { $setType(TOKEN_HSTRING)  }
    ;


/* A character string: this rule is not 1oo% correct as it will not
** ignore ws before  and  after eol. This needs  best to be handled
** via a language specific function. Note  that  rule  EOLnrm  will 
** replace any eol character by \n to simplify text processing.
** Contrary, ws is not normalized as ws can't be ignored in general.
*/
TOKEN_CSTRING 
    :   '"' (CHResc | EOLnrm | ~('"'|'\r'|'\n'))* '"' {
            s = self.chr_ws_erase($getText,"\n","\t ")
            $setText(s)
        }
    ;



/* ASN.1 has kind of tricky comment rule: A comment starts with "--"
** and ends either with a "--" or with a eol character. Nesting of
** comments is therefore not possible, ie.
**  -- not visible -- visible -- not visible
** The real ugly thing about this is that you can't just uncomment
** a line (regardless of it's content) by prefixing the liene with
** "--". For example assume you have this line:
**  one INTEGER ::= 1  -- sample integer
** Then have this:
**  -- one INTEGER ::= 1  -- sample integer
** This will hide ASN.1 and just makes the comment visible!
*/

COMMENT
    :
        "--" 
        (
            ~('-'|'\n'|'\r') | {self.LA(2) != '-'}? '-'
        )* 
        {
            if self.LA(1) == '-': self.match("--");
            $skip
        }
    ;

ALTCOMMENT
    :   { altcomment == true }? 
        ( ALTCOMMENT1 
        | ALTCOMMENT2 
        | ALTCOMMENT3
        )
        {
            $skip
        }
    ;

/* Due to problematic ASN.1  commentaries we have an alternative - 
** "//" starts a comment that eat's up everything till end of line
** (as in C++ and Java).
*/

protected
ALTCOMMENT1
    : 
        { altcomment == true }? "//" (~('\n'|'\r'))*
        {
            pass
        }
    ;

/* We also also for typical C comments albeit not nested ones */
protected
ALTCOMMENT2
    : "/*" 
        (
            options { 
                greedy=false; 
            } 
        : '\r' ( options { warnWhenFollowAmbig=false; } : '\n')? { $newline }
        | '\n' { $newline }
        | .    
        )* 
        "*/" 
        { 
            pass
        }
    ;

/* And as homage to the master of style, Niklaus Wirth, we also also
** comments ala PASCAL */
protected
ALTCOMMENT3
    : "{*" 
        (
            options { 
                greedy=false; 
            } 
        : '\r' ( options { warnWhenFollowAmbig=false; } : '\n')? { $nl }
        | '\n' { $nl; }
        | .
        )* 
        "*}" 
        {
            pass
        }
    ;

