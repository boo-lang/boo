#!/bin/sh
# Regenerates the AST classes from ast.model.boo.
#
# The generated sources are committed, so only editing ast.model.boo or one of
# the templates under scripts/Templates needs this script. It writes
# src/Boo.Lang.Compiler/Ast/Impl and the *.Generated.cs files beside it.
#
#   $ scripts/regenerate-ast.sh
#
# Commit whatever it changes along with the model.
#
# astgen.boo is itself a Boo program, so this needs booi, which is built here
# if it is missing. That makes the generator depend on a working compiler: if
# a change to the AST breaks the build, build an older commit first.

set -e

root=$(cd "$(dirname "$0")/.." && pwd)
configuration=${CONFIGURATION:-Debug}
booi="$root/src/booi/bin/$configuration/net10.0/booi"

if [ ! -x "$booi" ]; then
	echo "regenerate-ast: building booi"
	dotnet build "$root/Boo.slnx" --configuration "$configuration" --verbosity quiet --nologo
fi

if [ ! -x "$booi" ]; then
	echo "regenerate-ast: $booi was not built" >&2
	exit 1
fi

# astgen.boo resolves ast.model.boo, scripts/Templates and its output paths
# relative to the working directory.
cd "$root"
"$booi" scripts/astgen.boo
