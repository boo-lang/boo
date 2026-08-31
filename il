#!/bin/sh
# Compiles Boo source and prints the IL that came out of it.
# Set BOO_CONFIGURATION=Release to use that build of booc.
#
#   $ ./il examples/misc/now.boo
#   $ ./il -p:boo somefile.boo
#
# Arguments are passed through to booc, which is why the output path is not
# yours to choose. Disassembly is ilspycmd, pinned in .config/dotnet-tools.json;
# the old monodis and ildasm halves of this script needed Mono and the Windows
# SDK respectively.

set -e

config=${BOO_CONFIGURATION:-Debug}
root=$(cd "$(dirname "$0")" && pwd)
booc="$root/src/booc/bin/$config/net10.0/booc"

if [ $# -eq 0 ]; then
	echo "usage: il [booc options] <source.boo>" >&2
	exit 2
fi

if [ ! -x "$booc" ]; then
	echo "il: booc is not built. Run: dotnet build Boo.slnx" >&2
	exit 1
fi

work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT

"$booc" -target:library -out:"$work/il.dll" "$@" >&2

cd "$root"
if ! dotnet tool restore >/dev/null 2>&1; then
	echo "il: could not restore ilspycmd from .config/dotnet-tools.json" >&2
	exit 1
fi
dotnet ilspycmd -il "$work/il.dll"
