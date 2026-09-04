# Examples

Build Boo first, from the root of a clone:

```
dotnet build Boo.slnx
```

The `booi`, `booc` and `il` scripts at the root run what that produced.

## Running a script

Most examples are single files that run as they are:

```
./booi examples/misc/now.boo
```

## Macro examples

These come in two files: the macro itself, and a file using it. Compile the
macro to a library, then compile the usage against it. Seeing the expansion is
the point, so run the second step through the `boo` pipeline, which prints the
transformed source instead of emitting an assembly:

```
cd examples/macros/With
../../../booc -target:library -out:Macros.dll WithMacro.boo
../../../booc -p:boo -r:Macros.dll MacroUsage.boo
```

A macro that uses `assert` in its own body needs the assembly `assert` lives
in, which `booc` does not pick up on its own here:

```
../../../booc -target:library -out:Macros.dll \
	-r:../../../src/Boo.Lang.Useful/bin/Debug/net10.0/Boo.Lang.Extensions.dll \
	WithMacro.boo
```

## Compiler pipeline examples

Those under `pipeline/` follow the same shape: build the pipeline step as a
library, then invoke `booc -p:` with it.

## Inspecting the output

`il` compiles a file and prints the IL it produced:

```
./il examples/misc/now.boo
```

## When an example does not build

Not every example here has been checked against a current build. Run it
through `booi` and read the error; a missing type usually means the file needs
a `-r:` that the commands above do not pass.
