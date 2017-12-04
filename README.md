[![Build Status](https://travis-ci.org/boo-lang/boo.svg?branch=master)](https://travis-ci.org/boo-lang/boo)

The Boo Programming Language (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)

Prerequisites
=============

## Windows

- .NET 4.5
- [Visual C++ Build Tools*](http://landinghub.visualstudio.com/visual-cpp-build-tools)

\* Boo is built with NAnt, which must be built from sources, which requires NMake, which comes with the Visual C++ Build Tools.

## Mac/Linux

- Mono 4.2.x (4.2.4 is the latest and recommended)
- Bash

Build Tools
==============

You can install compatible versions of the required tools into the ```build-tools``` directory, where the build scripts will execute them from, by running the bootstrap script.

## Windows
The bootstrap script is a PowerShell script; however, it must be run from a x86 Native Tools Command Prompt:
```
# FROM A x86 NATIVE TOOLS COMMAND PROMPT
powershell .\build-tools\bootstrap
```

## Mac/Linux

```
./build-tools/bootstrap
```

### Mac

Building Boo requires Mono 4.2.x, which is not likely to be your "Current" version of Mono. To avoid having to switch your current version every time you want to work on Boo, you can specify the version to use when you run the bootstrap script. The build scripts will then use that version of Mono, regardless of your current version.

```
./build-tools/bootstrap [<mono version>]
```

Building
========

To build the repository, run the ```nant``` script:

```PowerShell
# Windows (PowerShell)
.\nant [<target>]
```

```sh
# Mac/Linux
./nant [<target>]
```

With no target specified, this will build the repository (code and tests) incrementally. To clean and build the repository from scratch, run the "rebuild" target. This will also cause the ast classes and parser
to be regenerated (needs a java vm)

To run the unit tests that have already been built with ```nant```, run the ```nunit``` script:

```PowerShell
# Windows (PowerShell)
.\nunit
```

```sh
# Mac/Linux
./nunit
```

To build and test the entire repository, the same way the CI build does, run the ```ci``` script:

```PowerShell
# Windows (PowerShell)
.\ci
```

```sh
# Mac/Linux
./ci
```

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


