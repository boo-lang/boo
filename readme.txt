The boo programming language
(c) 2004 Rodrigo Barreto de Oliveira (rbo@acm.org)

Just type:
	
	nant
	
to build the project.

	nant tests
	
will also run all the unit tests.

	nant rebuild
	
will also cause the ast classes and parser
to be regenerated (needs a java vm).

For a brief description of the project and its goals
take a look at docs/introduction.sxw.

extras/boox contains a sweet little tool you can use
to get yourself acquainted with the language.

src/ contains all the source code for the runtime and
compiler components.

tests/ contains all the unit tests
	testcases/compilation is a good source of information
	on the language features.

lib/ contains project dependencies such as antlr.

bin/ contains the latest version that passed all the tests
and could be successfully used to rebuild the system. 
