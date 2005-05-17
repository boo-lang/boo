// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header "Lexer.__main__" {
// main - create and run lexer from stdin
if __name__ == "__main__":
    import sys
    import antlr
    import columns_l
    
    // create lexer - shall read from stdin
    L = columns_l.Lexer()
    
    try:
        token = L.nextToken()
        while not token.isEOF():
            print token
            token = L.nextToken()
    
    except antlr.TokenStreamException, e:
        print "error: exception caught while lexing:", e
    
    // end of main
}

options {
    language=Python;
}
// not working: Lexer not seen at this place ..
// {
//    if __name__ == "__main__":
//      import columns_l
//      Lexer.main()

// }
class columns_l extends Lexer;

{
    done = False;

    def uponEOF(self):
        done=True

    def tab(self):
       t = 4;
       c = self.getColumn();
       nc = (((c-1)/t)+1)*t+1;
       self.setColumn( nc )

    def main():
       lexer = columns_l.Lexer()
       while not Lexer.done:
           t = lexer.nextToken();
           print "Token: ",t

    main = staticmethod(main)
}

INT : ('0'..'9')+ ;

ID : ('a'..'z')+ ;

WS : (' '|'\t'|'\n'{ self.newline();})+ {$setType(SKIP)}
   ;
