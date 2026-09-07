# boo-ls

A language server for the [Boo programming language](https://github.com/boo-dotnet/boo),
spoken over stdio.

## Install

```
dotnet tool install --global boo-ls
```

## Use

An editor starts it; you rarely run it by hand.

```
boo-ls --stdio
```

`--version` says which build answered, and `--help` lists the options.

## What it does

Diagnostics, document symbols, semantic tokens, completion, signature help,
hover, go to definition, find references, rename, and highlighting every use of
the name under the cursor.

Going to the definition of a type that only exists in a compiled assembly opens
decompiled source. That is C# by default; set `boo.decompiler.language` to `boo`
in a client that offers it to read it as Boo instead.

A document is analysed against the project it belongs to, so the types it takes
from a reference or from a file beside it resolve. A file in no project is
compiled with the files next to it.
