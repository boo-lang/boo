header {
    /* common code to all generated files */
    def println(*args):
        if args:
            import sys
            enc = sys.getdefaultencoding()
            for arg in args[0:-1]:
                print arg.encode(enc,"replace"),
            print args[-1].encode(enc,"replace")
}
header "__main__" {
    // the main header
    pass
}
header "ident_l.__main__" {
    import sys,codecs

    def warn(msg):
        print >>sys.stderr,"warning:",msg
        sys.stderr.flush()

    def error(msg):
        print >>sys.stderr,"error:",msg
        sys.stderr.flush()

    
    try:
        sys.stdin = codecs.lookup("Shift-JIS")[-2](sys.stdin)
    except:
        warn("Japanese codecs required - please install.")
        sys.exit(0)
    L = Lexer()
    for token in L: 
        // I'm being conservative here ..
        print token.__str__().encode("ascii","replace")
}

header "__init__" {
    // init - for all classes
}

header "ident_p.__init__" {
    // init - for ident_l
}

header "ident_l.__init__" {
    // init - for ident_p
}

options {
    language=Python;
}

/*
** Unicode example
** written by Matthew Ford (c)2000 Forward Computing and Control Pty. Ltd.
** email matthew.ford@forward.com.au
**
** The UnicodeLexer is the interesting part
*/



class ident_p extends Parser;

options {
	buildAST = false;	// skip the tree building
	defaultErrorHandler = false;     // Don't generate parser error handlers
}


program
	: (statement)* // perhaps none
	   EOF 
;

protected
statement
  {exprToken=None}
	: lhs:IDENT ASSIGNS rhs:IDENT SEMI!
        { println(" Found statement:   ",lhs.getText(),":=",rhs.getText() ); }
	| tt:TOTAL_TIME SEMI!
        { println(" Found TOTAL_TIME statement: ",tt.getText()); }
	| SEMI! {println(" Found empty statement"); }
	;



class ident_l extends Lexer;

options {
	charVocabulary = '\u0000'..'\uFFFE';  // allow all possiable unicodes except -1 == EOF
	testLiterals = false;  // in general do not test literals 
	caseSensitiveLiterals=false;
	caseSensitive=false;  
	defaultErrorHandler = false;   // pass error back to parser
  k = 2; // two character lookahead for // versus /*	
}

tokens {
  TOTAL_TIME = "\u5408\u8A08\u6642\u9593"; // total_time
}


// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
// NOTE: any char > \u0080 can start an Ident
// may need to restrict this more in some cases
// \uFFFF is EOF so do not include it here, stop at \uFFFE
IDENT
	options {testLiterals=true;
	    paraphrase = "an identifier";}
	:	('a'..'z'|'_'|'$'|'\u0080'..'\uFFFE') ('a'..'z'|'_'|'0'..'9'|'$'|'\u0080'..'\uFFFE')*
	;


ASSIGNS options {paraphrase = ":=";}
	: ":="
	;
	
SEMI options {paraphrase = ";";}
	: ';';
	

// white space is skipped by the parser  
WS	:	(	' '			
		|	'\t'
		|	'\r'('\n')?	 {self.newline();}
		|	'\n' {self.newline();}		
		)
		{$setType(Token.SKIP);}		// way to set token type
	;
