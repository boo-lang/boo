booi
====

Executes boo scripts by automatically compiling their source code.

Usage
-----

::

    Usage: booi [options] <script|-> [-- [script options]]
    Options:
         -cache[+-]           Generate compilation cache files (.booc) (default: -)
         -debug[+-]           Generate debugging information (default: +)
     -d  -define:symbol       Defines a symbols with optional values (=val)
         -ducky[+-]           Turns on duck typing by default
     -h  -help[+-]            Display this help and exit
     -l  -lib:directory       Adds a directory to the list of assembly search paths
     -o  -output:output       Save assembly in the given file name (with dependencies)
     -p  -packages:directory  Adds a packages directory for assemblies to load
     -r  -reference:assembly  References assembly
         -runner:executable   Runs an executable file passing the generated assembly
         -strict[+-]          Turns on strict mode
     -v  -verbose[+-]         Generate verbose information (default: -)
         -version[+-]         Display program version
     -w  -warnings[+-]        Report warnings (default: -)
         -wsa[+-]             Enables white-space-agnostic build

If you need to run code which is separated in multiple files you can provide all of them as 
arguments followed by ``--`` to indicate that no more ``booi`` options are expected (note: 
shell expansions like ``*.boo`` will also need this technique)::

    booi foo.boo bar.boo baz*.boo --
    booi *.boo -- param_for_the_script

Alternatively a directory can be given as argument, all .boo source files contained in that
directory (no recursion applied) will be part of the compilation.

If you prefer to obtain the script from *stdin*, you can either skip any arguments or use 
 ``-`` to indicate it should consume script code from *stdin*::

    echo "print BooVersion" | booi
    echo "print BooVersion" | booi foo.boo - --


Dependencies
------------

Any directory given in a ``-packages`` option is scanned for assemblies (*.dll) loading them 
automatically before any code is executed. The scanning algorithm supports the common convention 
of having assembly files in a directory but also understands the structure of NuGet local 
repositories. This allows you to manage dependencies with the NuGet command line program without 
needing to setup a project file.

When the compiler finds a namespace not currently loaded it will look in the following places for an assembly file matching the namespace.

  - Check ``-lib`` directories looking for assemblies
  - Check the GAC
  - Check ``-lib`` directories looking for paths with .boo source files


Automatic compilation
---------------------

When ``booi`` does not find a matching assembly for a namespace it will try to locate a directory 
structure which maps to the needed namespace. The algorithm is better explained with an example,
for the namespace *Foo.Bar.Baz* it will look for the following directories:

  - Foo.Bar.Baz
  - Foo.Bar/Baz
  - Foo/Bar/Baz

Once a matching directory is found a new compiler is created to compile all the boo source files in 
the directory without recursing into subdirectories. The compilation inherits the same settings given
to ``booi``, like assembly references, lib paths, defines ...

Any compiler errors produced by automatically compiling a *namespace* directory are reported to the 
user, indicating the offending location, so in essence it works just like if those source files where
actually given as arguments from the start.


Compilation caches
~~~~~~~~~~~~~~~~~~

In order to improve the performance of automatically compiling source code, ``booi`` will generate
(unless disabled with ``-cache-``) files with the extension ``.booc`` next to the automatically 
compiled directories.

These cache files can be safely deleted and should not be included in source control systems, just
apply a rule to ignore them completely.

They are simple text files, allowing to be easily inspected for troubleshooting, indicating
compiled source files, dependencies and the actual assembly. The compiler will load these
files if present and use them if they are up to date. Basically any modification to the source
files or assembly dependency will render the cache invalid and a new compilation will take place.


Runners
-------

Most of the .Net tooling expects to work with assemblies, to play nice with them the option 
``-runner`` can be used to indicate a command to be run after the script has been successfully
compiled instead of executing the actual script. 
Options and arguments can be given to the runner program using the same mechanism as if they
were for the script. If no arguments are explicitly given the full path to the assembly file
is provided as only argument. The following expansion patterns are available:

  - ``%`` : Full path to the assembly file
  - ``%assembly%`` : Full path to the assembly file
  - ``%filename%`` : Assembly's file name (without path)
  - ``%pathname%`` : Path to the assembly's directory
  - ``%solution%`` : Path to the MonoDevelop solution file

Checkout the following example of running NUnit tests. It will automatically compile all the
source files for tests, optionally compiling your project code if needed, and finally will
run the tests using NUnit's console runner::

    booi -l=src -runner=nunit-console tests/*Test.boo -- -exclude=ignore %


Debugging
---------

When you need to debug your script you can indicate ``booi`` to generate an assembly and
copy all the loaded dependencies next to it with the ``-output`` option. This allows to use
.Net/Mono debuggers attaching to process for example.

A very simple solution file, compatible with MonoDevelop (aka Xamarin Studio), will be also 
generated including references to the main script and any sources automatically compiled. Opening 
this solution with MonoDevelop will allow for setting breakpoints and run it under its debugger, 
the solution file however does not allow to rebuild from sources, so if you make changes you 
have to run again the ``booi`` command.


Tips
----

Configure an alias for ``booi`` in your shell to apply common conventions. For example::

    alias booi="/path/to/booi -p:packages -p:lib -l:src"

When a single boo source file is given as argument a special define ``MAIN`` is setup in 
the compiler. It allows to have several entry points in a project, specially useful to 
run tests from the same source file containing them::

    ifdef MAIN:
        # Place your entry point code here 

Debugging with MonoDevelop is possible even if it doesn't have the Boo language binding. 
Source code will not be highlighted but the debugger will allow to set breakpoints, stepping
the execution, inspecting locals and setting watches. Here is an example of how to run it
in OSX::

    booi -runner:/Application/Xamarin\ Studio.app test.boo -- %solution%
