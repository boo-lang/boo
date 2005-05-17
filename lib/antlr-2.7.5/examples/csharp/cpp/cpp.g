//
// A C PreProcessor
//
// Handles #define/#undef, #ifdef/#elsif/#else/#endif, and #include using only
// an ANTLR lexer (actually a stack of them).  This could be easily integrated
// with an existing lexer to do preprocessing and tokenizing all at once.
//
// Author: Eric Mahurin - eric_mahurin at yahoo dot com
// License: just give me credit
//
// BUG: missing some of the simpler directives
// BUG: doesn't follow the cpp spec perfectly - haven't made any effort at this
// not well tested
//
// Be aware that this is my first real attempt at both ANTLR and Java, so
// things may not be done the best way.  I welcome suggestions and fixes.
//

header {
	using System.Collections;
	using System.IO;
	using CommonAST					= antlr.CommonAST;
	using AST						= antlr.collections.AST;
	using TokenStreamSelector		= antlr.TokenStreamSelector;
	using TokenStreamRetryException = antlr.TokenStreamRetryException;
}

options {
    language="CSharp";
}

{
	class cpp : cppLexerTokenTypes {
	    public static TokenStreamSelector selector = new TokenStreamSelector();
	    public static void Main(string[] args) {
	        try {
	            // will need a stack of lexers for #include and macro calls
	            cppLexer mainLexer = new cppLexer(new CharBuffer(Console.In));
	            cppLexer.selector = selector;
	            selector.select(mainLexer);
	            for (;;) {
	                IToken t = selector.nextToken();
	                if (t.Type == Token.EOF_TYPE) 
	                {
	                	break;
	                }
	                Console.Out.Write(t.getText());
	            }
	        } catch(Exception e) {
	            Console.Error.WriteLine("exception: " + e);
	        }
	    }
	}
}

class cppLexer extends Lexer;

options {
    testLiterals 	= false;
    k 				= 4;
}

tokens {
    ENDIF ;
}

{
    public static TokenStreamSelector selector; 				// must be assigned externally
    protected static int ifState 			= 1; 				// -1: no-else false, 0: false, 1: true
    protected static IList ifStates 		= new ArrayList(); 	// holds nested if conditions
    protected static IDictionary defines 	= new Hashtable(); 	// holds the defines
    protected IDictionary defineArgs 		= new Hashtable(); 	// holds the args for a macro call
    
    public override void uponEOF() {
        try {
            selector.pop(); // return to old lexer/stream
            selector.retry();
        } catch (/*NoSuchElement*/Exception /*e*/) {
            // return a real EOF if nothing in stack
        }
    }
}

DIRECTIVE {
    IList args 		= new ArrayList();
    bool  condition = true;
} : '#'
    ( "include" (WS)? includeFile:STRING { if (ifState==1) {
        // found this in examples/java/includeFile
        string name = includeFile.getText();
        name = name.Substring(1, name.Length-2);
        try {
            cppLexer sublexer = new cppLexer(new StreamReader(name));
            cppLexer.defines = defines; // want defines to be persistent
            sublexer.setFilename(name);
            selector.push(sublexer);
            selector.retry();
        } catch (/*FileNotFound*/IOException /*fnf*/) {
            Console.Error.WriteLine("cannot find file "+name);
        }
    }}
    | "define" WS defineMacro:RAW_IDENTIFIER
    {
        args.Add(""); // first element will hold the macro text
    }
        (
            ( '(' // get arguments if you find them (no spaces before left paren)
                (WS)? defineArg0:RAW_IDENTIFIER (WS)? {args.Add(defineArg0.getText());}
                ( COMMA (WS)? defineArg1:RAW_IDENTIFIER (WS)? {args.Add(defineArg1.getText());} )*
              ')'
            | ' '|'\t'|'\f'
            )
            ( options{greedy=true;}: ' '|'\t'|'\f' )*
            // store the text verbatim - tokenize when called
            defineText:MACRO_TEXT { args[0] = defineText.getText(); }
        )? '\n' { newline(); }
    { if (ifState==1) {
        defines[defineMacro.getText()] = args;
        $setType(Token.SKIP);
    }}
    | "undef" WS undefMacro:RAW_IDENTIFIER { if (ifState==1) {
        defines.Remove(undefMacro.getText());
        $setType(Token.SKIP);
    }}
    | ("ifdef"|"ifndef"{condition=false;})
        WS ifMacro:RAW_IDENTIFIER
    {
        ifStates.Add(ifState);
        if (ifState==1) {
            condition = (defines.Contains(ifMacro.getText())==condition);
            ifState = condition?1:0;
        } else {
            ifState = -1;
        }
        if (ifState==1) {
            $setType(Token.SKIP);
        } else {
            // gobble up tokens until ENDIF (could be caused by else)
            for (;;) {
                try {
                    if (selector.nextToken().Type==ENDIF) break;
                } catch (TokenStreamRetryException /*r*/) {
                    // just continue if someone tried retry
                }
            }
            // retry in case we switched lexers
            selector.retry();
        }
    }
    |
        ( "else" // treat like elsif (true)
        | "elsif" WS elsifMacro:RAW_IDENTIFIER {
            condition=defines.Contains(elsifMacro.getText());
        }
        )
    {
        if (ifState==1) {
            // previous if/elsif was taken - discard rest
            ifState = -1;
            for (;;) {
                try {
                    if (selector.nextToken().Type==ENDIF) break;
                } catch (TokenStreamRetryException /*r*/) {
                    // just continue if someone tried retry
                }
            }
            // retry in case we switched lexers
            selector.retry();
        } else if (ifState==0 && condition) {
            // "elsif" (true) or "else"
            $setType(ENDIF);
            ifState = 1;
        }
    }
    | "endif" {
        condition = (ifState==1);
        try {
            // return to previous if state
            ifState = (int) ifStates[ifStates.Count - 1];
            ifStates.RemoveAt(ifStates.Count - 1);
            if (condition) {
                $setType(Token.SKIP);
            } else {
                // tell if/else/elsif to stop discarding tokens
                $setType(ENDIF);
            }
        } catch (ArgumentOutOfRangeException /*e*/) {
            // endif with no if
        }
    }
    );

IDENTIFIER options {testLiterals=true;} {
    IList define = new ArrayList();
    IList args = new ArrayList();
} :
    identifier:RAW_IDENTIFIER
    {
        // see if this is a macro argument
        define = (IList)defineArgs[identifier.getText()];
        if (_begin==0 && define==null) {
            // see if this is a macro call
            define = (IList)defines[identifier.getText()];
        }
    }
    ( { (define!=null) && (define.Count > 1) }? (WS|COMMENT)?
        // take in arguments if macro call requires them
        '('
        callArg0:EXPR {args.Add(callArg0.getText());}
        ( COMMA callArg1:EXPR {args.Add(callArg1.getText());} )*
        { args.Count==define.Count-1 }? // better have right amount
        ')'
    | { !((define!=null) && (define.Count>1)) }?
    )
{ if (define!=null) {
    string defineText = (string)define[0];
    if (_begin!=0) {
        // just substitute text if called from EXPR - no token created
        $setText(defineText);
    } else {
        // create a new lexer to handle the macro text
        cppLexer sublexer = new cppLexer(new StringReader(defineText));
        for (int i=0;i<args.Count;++i) {
            // treat macro arguments similar to local defines
            IList arg = new ArrayList();
            arg.Add((string)args[i]);
            sublexer.defineArgs[define[1+i]] = arg;
        }
        selector.push(sublexer);
        // retry in new lexer
        selector.retry();
    }
}};

STRING
    : '"' ( '\\' . | ~('\\'|'"') )* '"' // double quoted string
    | '\'' ( '\\' . | ~('\\'|'\'') )* '\'' // single quoted string
    ;

protected MACRO_TEXT :
    ( '\\'! NL {newline();} // escaped newline
    | ~('\n'|'\r')
    )*;

protected
NL :
    ( '\r'
    | '\n'
    | '\r' '\n'
    );

WS :
    ( ' '
    | '\t'
    | '\f'
    | NL {newline();}
    ) { /*$setType(Token.SKIP);*/ };

COMMENT :
    ( "//" (~('\n'|'\r'))* NL {newline();} // single line comment
    | "/*" ( options{greedy=false;} : NL {newline();} | ~('\n'|'\r') )* "*/" // multi-line comment
    ) { /*$setType(Token.SKIP);*/ };

protected RAW_IDENTIFIER : ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')* ;

NUMBER : ('0'..'9') ('0'..'9'|'a'..'z'|'A'..'Z'|'_')* ; // allow ahpha suffixes on numbers (i.e. L:long)

// group symbols into categories to parse EXPR
LEFT  : '(' | '[' | '{' ;
RIGHT : ')' | ']' | '}' ;
COMMA : ',' ;
OPERATOR : '!' | '#' | '$' | '%' | '&' | '*' | '+' | '-' | '.' | '/' | ':' | ';' | '<' | '=' | '>' | '?' | '@' | '\\' | '^' | '`' | '|' | '~' ;

protected EXPR // allow just about anything without being ambiguous
    : (WS)? (NUMBER|IDENTIFIER)?
        (
            ( LEFT EXPR ( COMMA EXPR )* RIGHT
            | STRING
            | OPERATOR // quotes, COMMA, LEFT, and RIGHT not in here
            )
            EXPR
        )?
    ;

