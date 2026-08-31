[![CI](https://github.com/boo-lang/boo/actions/workflows/ci.yml/badge.svg)](https://github.com/boo-lang/boo/actions/workflows/ci.yml)

The Boo Programming Language (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)

Building with the .NET SDK
==========================

Requires the .NET 10 SDK.

```
dotnet build Boo.slnx
dotnet test Boo.slnx
```

The C# core builds first and produces `booc`, which then compiles the Boo
libraries. The `booc`, `booi` and `booish` scripts run what it produced, with
`.cmd` versions of each for Windows; set `BOO_CONFIGURATION=Release` for a
release build.

Regenerating generated sources
==============================

Both generators write files that are committed, so this is only needed after
editing their inputs.

```
scripts/regenerate-ast.sh        # after ast.model.boo or scripts/Templates
scripts/regenerate-parser.sh     # after BooLexer.g4 or BooParser.g4, needs java
```

Testing against musl
====================

CI covers Ubuntu, macOS and Windows, all glibc. To check Alpine as well:

```
scripts/test-alpine.sh
```

It runs the suite in a dotnet 10 Alpine container through podman.

Building a Debian package
=========================

```
scripts/build-deb.sh
```

Writes a self contained .deb to `artifacts/`, built in a container through
podman. It carries the .NET runtime, so it depends only on the C library and
ICU, and is built for whichever architecture podman is running.

How to Start
============

For a brief description of the project and its goals
take a look at `docs/BooManifesto.sxw`.

`extras/boox` contains a sweet little tool you can use
to get yourself acquainted with the language.

`src/` contains all the source code for the runtime and
compiler components.

`tests/` contains all the unit tests.

`tests/testcases/integration` is a good source of information
on the language features.

Running and compiling code
==========================

To execute a boo script run:

	booi <script> [args]
	
For instance:

	booi examples/misc/now.boo	
	
You can also have booi to read from stdin by typing:

	booi -
	
You can generate .net assemblies by using `booc`:

	booc -output:hello.exe examples/misc/now.boo	
	
If you want to simply see the transformations applied to
your code by the compiler use the boo pipeline, run:

	booc -p:boo examples/misc/replace.boo	

To see the IL a source file compiles to, run:

	./il examples/misc/now.boo

On Windows use `il.cmd`. Disassembly uses ilspycmd, restored from
`.config/dotnet-tools.json` on first run.
	
More Information
================

Boo development Google group:
https://groups.google.com/forum/#!forum/boolang

Boo community Discord:
[https://discord.gg/J4Guxadwma](https://discord.gg/J4Guxadwma)

Contributors
============

See: https://github.com/boo-lang/boo/graphs/contributors


