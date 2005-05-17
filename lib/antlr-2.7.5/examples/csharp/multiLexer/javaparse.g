options
{
	language = "CSharp";
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
			DemoJavaDocParser jdocparser = new DemoJavaDocParser(getInputState());
			jdocparser.content();
		}
		JAVADOC_CLOSE
	;
