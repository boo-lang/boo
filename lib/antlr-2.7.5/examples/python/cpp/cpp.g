/*
 * A C PreProcessor
 * 
 * Handles #define/#undef, #ifdef/#elsif/#else/#endif, and #include using only
 * an ANTLR lexer (actually a stack of them).  This could be easily integrated
 * with an existing lexer to do preprocessing and tokenizing all at once.
 * 
 * Author: Eric Mahurin - eric_mahurin at yahoo dot com
 * License: just give me credit
 * 
 * BUG: missing some of the simpler directives
 * BUG: doesn't follow the cpp spec perfectly - haven't made any effort at
 * this not well tested
 * 
 * Be aware that this is my first real attempt at both ANTLR and Java, so
 * things may not be done the best way.  I welcome suggestions and fixes.
 *
 * 041124 - cpp.g translated and adapted as Python example by MK.
 */

header {
import sys
import StringIO
}

header "__main__" {

import traceback

class cpp:

    def __init__(self, *args):
        try:
            // will need a stack of lexers for #include and macro calls
            self.mainLexer = Lexer(sys.stdin)
            Lexer.selector.select(self.mainLexer)
            for token in Lexer.selector:
                sys.stdout.write(token.getText())
        except Exception, e:
            sys.stderr.write("exception: " + str(e) + '\n')
            traceback.print_exc()

Lexer.selector = antlr.TokenStreamSelector()

cpp(sys.argv[1:])

}

options {
    language="Python";
}

class cppLexer extends Lexer;

options {
    testLiterals = false;
    k = 4;
}

tokens {
    ENDIF ;
}

{
    selector = antlr.TokenStreamSelector()     // must be assigned externally

    ifState = 1         // -1: no-else false, 0: false, 1: true
    ifStates = []       // holds nested if conditions
    defines = {}        // holds the defines
    defineArgs = {}     // holds the args for a macro call

    def uponEOF(self):
        if Lexer.selector.getCurrentStream() != Lexer:
            try:
                Lexer.selector.pop() // return to old lexer/stream
                Lexer.selector.retry()
            //except antlr.TokenStreamRetryException, tsre:
            //    raise tsre
            except IndexError:
                // return a real EOF if nothing in stack
                pass

}

DIRECTIVE {
    args = []
    condition = True
} : '#'
    ( "include" (WS)? includeFile:STRING
      {
        if Lexer.ifState == 1:
            name = includeFile.getText()
            name = name[1:-1]
            try:
                sublexer = Lexer(file(name))
        // want defines to be persistent
                sublexer.defines = Lexer.defines
                sublexer.setFilename(name)
                Lexer.selector.push(sublexer)
                Lexer.selector.retry()
            except IOError, e:
                sys.stderr.write("cannot find file " + name + '\n')
      }
    | "define" WS defineMacro:RAW_IDENTIFIER
      {
          // first element will hold the macro text
      }
      (
        ( '(' // get arguments if you find them (no spaces before left paren)
          (WS)? defineArg0:RAW_IDENTIFIER (WS)?
          { args += defineArg0.getText() }
          ( COMMA (WS)? defineArg1:RAW_IDENTIFIER (WS)?
            { args += defineArg1.getText() } )*
          ')'
        | ' '|'\t'|'\f'
        )
        ( options { greedy=true; } : ' '|'\t'|'\f' )*
        // store the text verbatim - tokenize when called
        defineText:MACRO_TEXT { args[0] = defineText.getText() }
      )? ('\n'|"\r\n"|'\r') { $newline }
      {
          if Lexer.ifState == 1:
              Lexer.defines[defineMacro.getText()] = args
              $skip
      }
    | "undef" WS undefMacro:RAW_IDENTIFIER
      {
          if Lexer.ifState == 1:
              del Lexer.defines[undefMacro.getText()]
              $skip
      }
    | ( "ifdef" | "ifndef" { condition=False } )
      WS ifMacro:RAW_IDENTIFIER
      {
        Lexer.ifStates.append(ifState)
        if Lexer.ifState == 1:
            if Lexer.defines.has_key(ifMacro.getText()) == condition:
                Lexer.ifState = 1
            else:
                Lexer.ifState = 0
        else:
            Lexer.ifState = -1
        if Lexer.ifState == 1:
            $skip
        else:
            // gobble up tokens until ENDIF (could be caused by else)
            while True:
                try:
                    if Lexer.selector.nextToken().getType() == ENDIF:
                        break
                except antlr.TokenStreamRetryException, r:
                    // just continue if someone tried retry
                    pass
            // retry in case we switched lexers
            Lexer.selector.retry()
      }
    | ( "else" // treat like elsif (true)
      | "elsif" WS elsifMacro:RAW_IDENTIFIER
        {
            condition = Lexer.defines.has_key(elsifMacro.getText())
        }
      )
      {
        if Lexer.ifState == 1:
            // previous if/elsif was taken - discard rest
            Lexer.ifState = -1;
            while True:
                try:
                    if Lexer.selector.nextToken().getType() == ENDIF:
                        break
                except antlr.TokenStreamRetryException, r:
                    // just continue if someone tried retry
                    pass
            // retry in case we switched lexers
            Lexer.selector.retry()
        elif Lexer.ifState == 0 and condition:
            // "elsif" (true) or "else"
            $setType(ENDIF)
            Lexer.ifState = 1
      }
    | "endif"
      {
        if Lexer.ifState == 1:
            condition = True
        else:
            condition = False
        try:
            // return to previous if state
            del Lexer.ifStates[-1]
            if condition:
                $skip
            else:
                // tell if/else/elsif to stop discarding tokens
                $setType(ENDIF)
        except IndexError, e:
            // endif with no if
            pass
      }
    );

IDENTIFIER options { testLiterals=true; } {
    define = []
    args = []
} :
    identifier:RAW_IDENTIFIER
    {
        // see if this is a macro argument
        define = Lexer.defineArgs.has_key(identifier.getText())
        if define:
            define = Lexer.defineArgs[identifier.getText()]
        elif _begin == 0 and not define:
            // see if this is a macro call
            define = Lexer.defines.has_key(identifier.getText())
            if define:
                define = Lexer.defines[identifier.getText()]
    }
    ( { define and len(define) }? ( WS | COMMENT )?
        // take in arguments if macro call requires them
        '('
        callArg0:EXPR { args += callArg0.getText() }
        ( COMMA callArg1:EXPR { args += callArg1.getText() } )*
        { len(args) == len(define)-1 }? // better have right amount
        ')'
    | { not (define and len(define)) }?
    )
    {
      if define:
          defineText = define[0]
          if _begin:
              // just substitute text if called from EXPR - no token created
              $setText(defineText)
          else:
              // create a new lexer to handle the macro text
              sublexer = Lexer(StringIO.StringIO(defineText))
              for i in range(len(args)):
                  // treat macro arguments similar to local defines
                  arg = []
                  arg.append(args[i])
                  sublexer.defineArgs[define[1+i]] = arg
              Lexer.selector.push(sublexer)
              // retry in new lexer
              Lexer.selector.retry()
    };

STRING
    : '"' ( '\\' . | ~('\\'|'"') )* '"' // double quoted string
    | '\'' ( '\\' . | ~('\\'|'\'') )* '\'' // single quoted string
    ;

protected
MACRO_TEXT :
    ( '\\'! NL { $newline } // escaped newline
    | ~('\n'|'\r')
    )*
    ;

protected
NL
options {
    generateAmbigWarnings=false; // single '\r' is ambig with '\r' '\n'
}
	: '\r'
	| '\n'
	| '\r' '\n'
    ;

WS :
    ( ' '
    | '\t'
    | '\f'
    | NL { $newline }
    ) { /* $skip */ }
    ;

COMMENT :
    ( "//" (~('\n'|'\r'))* NL { $newline } // single line comment
    | "/*" ( options{greedy=false;} : NL { $newline } | ~('\n'|'\r') )* "*/"
      // multi-line comment
    ) { /* $skip */ }
    ;

protected
RAW_IDENTIFIER :
    ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
    ;

NUMBER :
    ('0'..'9') ('0'..'9'|'a'..'z'|'A'..'Z'|'_')*
    // allow alpha suffixes on numbers (i.e. L:long)
    ;

// group symbols into categories to parse EXPR
LEFT  : '(' | '[' | '{' ;
RIGHT : ')' | ']' | '}' ;
COMMA : ',' ;
OPERATOR : '!' | '#' | '$' | '%' | '&' | '*' | '+' | '-' | '.' | '/' | ':' | ';' | '<' | '=' | '>' | '?' | '@' | '\\' | '^' | '`' | '|' | '~' ;

protected
EXPR // allow just about anything without being ambiguous
    : (WS)? (NUMBER|IDENTIFIER)?
      (
        ( LEFT EXPR ( COMMA EXPR )* RIGHT
        | STRING
        | OPERATOR // quotes, COMMA, LEFT, and RIGHT not in here
        )
        EXPR
      )?
    ;
