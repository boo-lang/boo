// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class javadoc_p extends Parser;
options {
    importVocab=JavaDoc;
}

content
        :       (       p:PARAM // includes ID as part of PARAM
                        {print "found: ",p.getText()}
                |       e:EXCEPTION
                        {print "found: ",e.getText()}
                )*
        ;
