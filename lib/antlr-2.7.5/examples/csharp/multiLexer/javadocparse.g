options
{
	language = "CSharp";
}

class DemoJavaDocParser extends Parser;
options {
	importVocab=JavaDoc;
}

content
	:	(	p:PARAM	// includes ID as part of PARAM
			{Console.Out.WriteLine("found: "+p.getText());}
		|	e:EXCEPTION
			{Console.Out.WriteLine("found: "+e.getText());}
		)*
	;
