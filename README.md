The boo programming language (c) 2009- Rodrigo Barreto de Oliveira (rbo@acm.org)

Distribution of binaries available at
http://www.sieda.com/de-wAssets/en/docs/products/Boo-distrib/.

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
	
Additionally, boo also comes with an interactive shell that
provides you with the opportunity to use the interpreter
interactively.

	booish
	
After starting the shell you may type in command "h" to
display some hopefully helpful information on the
particular abilities of this shell. The startup environment
of the shell may be configured editing file booish.rsp that
also resides in the bin/ folder (must be side-by-side to
the program).

You can generate .net assemblies by using `booc` (either
the `booc.exe` utility or the `booc nant` task):

	booc -output:build/hello.exe examples/hw.boo	
	
If you want to simply see the transformations applied to
your code by the compiler use the boo pipeline, run:

	booc -p:boo examples/replace.boo	
	
Folder bin/ also contains support modules for the NANT and
MSBUILD build systems. You will have to define environment
variable "BooBinPath" in such a way that

	$(BooBinPath)\Boo.Microsoft.Build.targets
	
points at the definitions of the MSBuild targets referring to
Boo.


More Information
================

http://boo-lang.org/
https://groups.google.com/forum/#!forum/boolang

Contributors
============

See: https://github.com/hmah/boo/graphs/contributors

Differences to the original distribution
=======================================


- Fix of issue BOO-1078 "Cannot use enumerations as attribute parameters."
- Version information on assemblies is now pasted into the native resources. Thus, Windows Explorer can read and display it.
- booish: describe() now looks for XML documentation of referenced assemblies.
- booish: Autocompletion offers suggestions in a scrollable list with one offer per row (better overview).
- booish: Solved some problems with managing input into the shell. Added mode to allow users to paste content into the shell.
- booish: booish.rsp now offers additional options to configure the startup behaviour of the shell.
- booish: Added means to add commands to the shell. Added booish.mod.os to demonstrate this. This module provides commands like "cd" and "dir".
- boo: range(-1) now is an empty enumeration (instead of raising an exception).
- boo: Operator "isa" now also works with structs. This is important to avoid exceptions on casts.
- boo: Constructors and destructors now do not require a "def" (but you still can write this). Keyword "def" is redundant here (not very wrist friendly).
- boo: Macro "property" now also works for static classes.
- boo: Added macro "getproperty" that keeps the setter private.
- boo: array(int, null) now returns null (without an exception). This spares some if-then when using array for casting.
- boo: Resolved issues #57 and #58.

If you want to paste preformatted lines of code into the console, you will have to turn
off autoindention using the shell command "indent".

	>>> indent
	Auto indention has been turned off. User [SHIFT][RETURN] to leave the editor and execute the command.
	-->

The prompt will show you whether autoindention will be done or not.

Please note, that complex input operations on the System.Console are error prone. There
are certainly several things to do here that probably will never be done. You might
experience problems when you enter lines of codes that are longer that span over more
than two lines of the console's line buffer.
