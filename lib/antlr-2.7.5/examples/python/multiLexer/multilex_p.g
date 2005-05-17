// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class multilex_p extends Parser;
options {
	importVocab=Java;
}

input
	:	( (javadoc)? INT ID SEMI )+
	;

javadoc
	:	JAVADOC_OPEN
		{
            import javadoc_p
            jdocparser = javadoc_p.Parser(self.getInputState())
            jdocparser.content();
		}
		JAVADOC_CLOSE
	;
