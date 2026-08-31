#!/bin/sh
# Boo bisecting tool.
#
# Usage:
#   $ git bisect start BAD_REV GOOD_REV   (eg. HEAD v0.9.7)
#
#   [to test against one source file]
#   $ export TESTCASE=testcase.boo        (eg. tests/testcases/regression/BOO-1008-1.boo)
#   [to test against one fixture]
#   $ export TESTFIXTURE=fixture          (eg. BooCompiler.Semantics)
#
#   with neither set, the whole suite runs
#
#   $ git bisect run extras/bisect.sh
#
# Exit 125 tells git the revision cannot be judged, which is what a commit
# that does not build deserves.

if [ -n "$TESTCASE" ] && [ -n "$TESTFIXTURE" ]; then
	echo "!!! Both TESTCASE and TESTFIXTURE are set. Please make your mind!"
	exit 255
fi

root=$(cd "$(dirname "$0")/.." && pwd)
cd "$root" || exit 125

if ! dotnet build Boo.slnx --configuration Release --nologo --verbosity quiet; then
	echo "!!! SKIP (cannot build)"
	exit 125
fi

if [ -n "$TESTCASE" ]; then
	# shellcheck disable=SC2086
	BOO_CONFIGURATION=Release ./booc $BOOC_OPTIONS "$TESTCASE"
	result=$?
elif [ -n "$TESTFIXTURE" ]; then
	dotnet test Boo.slnx --configuration Release --no-build --filter "FullyQualifiedName~$TESTFIXTURE"
	result=$?
else
	dotnet test Boo.slnx --configuration Release --no-build
	result=$?
fi

if [ "$result" != "0" ]; then
	echo "!!! BAD"
	exit 1
fi
echo "!!! good"
exit 0
