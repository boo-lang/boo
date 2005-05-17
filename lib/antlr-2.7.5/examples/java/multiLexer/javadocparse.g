class DemoJavaDocParser extends Parser;
options {
	importVocab=JavaDoc;
}

content
	:	(	p:PARAM	// includes ID as part of PARAM
			{System.out.println("found: "+p.getText());}
		|	e:EXCEPTION
			{System.out.println("found: "+e.getText());}
		)*
	;
