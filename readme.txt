The boo programming language
(c) 2004 Rodrigo Barreto de Oliveira
rbo@acm.org

Building
========

Just type:
	
	nant
	
to build the project.

	nant tests
	
will also run all the unit tests.

	nant rebuild tests
	
will also cause the ast classes and parser
to be regenerated (needs a java vm).

How to Start
============

For a brief description of the project and its goals
take a look at docs/BooManifesto.sxw.

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

Running and compiling code
==========================

To execute a boo script run:

	booi <script> [args]
	
For instance:

	booi examples/hw.boo	
	
You can also have booi to read from stdin by typing:

	booi -
	
You can generate .net assemblies by using booc (either
the booc.exe utility or the booc nant task):

	booc -output:build/hello.exe examples/hw.boo
	
You can enable duck typing by specifying the appropriate
pipeline with -p option:

	booc -p:quack examples/ie.boo
	
If you want to simply see the transformations applied to
your code by the compiler use the boo pipeline, run:

	booc -p:boo examples/replace.boo	
	

	

