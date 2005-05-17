options {
	language="Cpp";
}

{
#include "DemoJavaDocParser.hpp"
}

class DemoJavaParser extends Parser;
options {
	importVocab=Java;
}

input
	:	( (javadoc)? INT ID SEMI )+
	;

javadoc
	:	JAVADOC_OPEN
		{
		DemoJavaDocParser jdocparser(getInputState());
		jdocparser.content();
		}
		JAVADOC_CLOSE
	;

