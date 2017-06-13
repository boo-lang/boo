[![Build Status](https://travis-ci.org/boo-lang/boo.png?branch=master)](https://travis-ci.org/boo-lang/boo)

The Boo Programming Language (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)

Prerequisites
=============

## Mono

If you are building on Mono, you need to have a compatible version from the 4.2.x line. The most recent compatible version is [4.2.4](https://download.mono-project.com/archive/4.2.4/).

## Build Tools

### Windows/Linux
You'll need to install compatible versions of NAnt and NUnit. See [build-tools/versions](build-tools/versions) for known-good versions. _NAnt must be cloned and built from sources_; the last official release (0.92) does not work.

### Mac
Just type:

```
./build-tools/bootstrap [<mono version>]
```

This will install compatible versions of NAnt and NUnit into the ```build-tools``` directory.

Building
========

### Build Tools

Just type:
	
	# Installed on system
	nant

	# Installed via bootstrap
	./nant
	
to build the project.

	nant test
	
will also run all the unit tests.

to rebuild everything from scratch:

	nant rebuild
	
the rebuild target will also cause the ast classes and parser
to be regenerated (needs a java vm).

How to Start
============

For a brief description of the project and its goals
take a look at `docs/BooManifesto.sxw`.

`extras/boox` contains a sweet little tool you can use
to get yourself acquainted with the language.

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

http://boo-lang.org/
https://groups.google.com/forum/#!forum/boolang

Contributors
============

See: https://github.com/boo-lang/boo/graphs/contributors


