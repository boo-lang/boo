#!/bin/sh
# Regenerates src/Boo.Lang.Parser/Generated from BooLexer.g4 and BooParser.g4.
#
# The generated sources are committed so that building Boo needs neither Java
# nor a network fetch. Only editing a grammar needs this script, and only this
# script needs Java: ANTLR 4's code generator is a Java program and has no .NET
# equivalent. The jar ships inside a NuGet package, which is fetched here if it
# is not already in the local cache.
#
#   $ scripts/regenerate-parser.sh
#
# Commit whatever it changes under Generated/ along with the grammar.

set -e

VERSION=4.13.1
PACKAGE=antlr4codegenerator.tool
PACKAGE_VERSION=2.3.0

root=$(cd "$(dirname "$0")/.." && pwd)
parser="$root/src/Boo.Lang.Parser"

if ! command -v java >/dev/null 2>&1; then
	echo "regenerate-parser: java is needed to run the ANTLR tool, and is not on PATH." >&2
	echo "                   Nothing else in this repository needs it." >&2
	exit 1
fi

jar=$(find "$HOME/.nuget/packages" "$HOME/.cache/NuGetPackages" -name "antlr-$VERSION-complete.jar" 2>/dev/null | head -1)

if [ -z "$jar" ]; then
	echo "regenerate-parser: fetching the ANTLR $VERSION tool"
	work=$(mktemp -d)
	trap 'rm -rf "$work"' EXIT
	curl -fsSL -o "$work/tool.nupkg" \
		"https://api.nuget.org/v3-flatcontainer/$PACKAGE/$PACKAGE_VERSION/$PACKAGE.$PACKAGE_VERSION.nupkg"
	unzip -q -o "$work/tool.nupkg" -d "$work"
	jar=$(find "$work" -name "antlr-$VERSION-complete.jar" | head -1)
fi

if [ -z "$jar" ]; then
	echo "regenerate-parser: could not find antlr-$VERSION-complete.jar" >&2
	exit 1
fi

echo "regenerate-parser: using $jar"
cd "$parser"
java -jar "$jar" \
	-Dlanguage=CSharp \
	-package Boo.Lang.ParserA4 \
	-visitor \
	-no-listener \
	-o Generated \
	BooLexer.g4 BooParser.g4

# The tool also writes .interp and .tokens beside the sources; they describe the
# grammar it just read and are not compiled.
rm -f Generated/*.interp Generated/*.tokens

echo "regenerate-parser: wrote"
ls -1 Generated/*.cs | sed 's|^|                   |'
