options {
	language="Cpp";
}

{
#include <iostream>
}

class DemoJavaDocParser extends Parser;
options {
	importVocab=JavaDoc;
}

content
	:	(	p:PARAM	// includes ID as part of PARAM
			{std::cout << "found: " << p->getText() << std::endl;}
		|	e:EXCEPTION
			{std::cout << "found: " << e->getText() << std::endl;}
		)*
	;
