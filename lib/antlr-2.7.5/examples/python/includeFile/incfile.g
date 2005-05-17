// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class incfile_p extends Parser;
{
    def traceOut(self,rname):
        print "exit ",rname,"; LT(1)=",self.LT(1)

    def traceIn(self,rname):
        print "enter ",rname,"; LT(1)=",self.LT(1)
}

startRule
    :   ( decl )+
    ;

decl:   INT a:ID {print("decl "+a.getText());}
        ( COMMA b:ID {print("decl "+b.getText());} )*
        SEMI
    ;

{
    pass
}

class incfile_l extends Lexer;
options {
    charVocabulary = '\3'..'\377';
    k=2;
}

tokens {
    INT="int";
}

{
    def uponEOF(self):
        import incfile
        if incfile.getselector().getCurrentStream() != incfile.getlexer():
            incfile.getselector().pop()
            incfile.getselector().retry()
        else:
            print("Hit EOF of main file")
}

SEMI:   ';'
    ;

COMMA
    :   ','
    ;

ID
    :   ('a'..'z')+
    ;

INCLUDE
    :   "#include" (WS)? f:STRING
        {
        name = f.getText();
        input= None
        try:
            fi = open(name,"rb")
        except IOError:
            import sys
            print >>sys.stderr,"cannot find file ",name
        else:
            import incfile
            assert fi
            sublexer = Lexer(fi);
            sublexer.setFilename(name);
            incfile.getparser().setFilename(name);
            incfile.getselector().push(sublexer);
            incfile.getselector().retry()
        }
    ;

STRING
    :   '"'! ( ~'"' )* '"'!
    ;

WS  :   (   ' '
        |   '\t'
        |   '\f'
            // handle newlines
        |   (   options {generateAmbigWarnings=false;}
            :   "\r\n"  // Evil DOS
            |   '\r'    // Macintosh
            |   '\n'    // Unix (the right way)
            )
            { self.newline(); }
        )+
        { $setType(Token.SKIP); }
    ;
