The boo programming language (c) 2009- Rodrigo Barreto de Oliveira (rbo@acm.org)

This distribution contains contributions of Harald Meyer auf'm Hofe (harald_meyer@users.sourceforge.net)

Building
========

Just type:
	
	nant
	
to build the project.

	nant test
	
will also run all the unit tests.

mono users might want to do this instead:

	nant compile-tests && nunit-console tests/build/*Tests.dll	

to rebuild everything from scratch:

	nant rebuild
	
the rebuild target will also cause the ast classes and parser
to be regenerated (needs a java vm).

How to Start
============

For a brief description of the project and its goals
take a look at `docs/BooManifesto.sxw`.

`src/` contains all the source code for the runtime and
compiler components.

`tests/` contains all the unit tests.

`testcases/integration` is a good source of information
on the language features.

`lib/` contains project dependencies such as antlr.

`bin/` contains the latest version that passed all the tests
and could be successfully used to rebuild the system.

Running and compiling code
==========================

To execute a boo script run:

	booi <script> [args]
	
For instance:

	booi examples/hw.boo	
	
You can also have booi to read from stdin by typing:

	booi -
	
You can generate .net assemblies by using `booc` (either
the `booc.exe` utility or the `booc nant` task):

	booc -output:build/hello.exe examples/hw.boo	
	
If you want to simply see the transformations applied to
your code by the compiler use the boo pipeline, run:

	booc -p:boo examples/replace.boo	
	
More Information
================

http://boo.codehaus.org/
http://boo.codehaus.org/Mailing+Lists

Contributors
============

See: https://github.com/hmah/boo/graphs/contributors

Differnces to the original distribution
=======================================

- Fix of issue BOO-1078 Cannot use enumerations as attribute parameters.
- booish: Display one suggestion per line. Allow scrolling.
