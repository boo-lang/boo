#!/bin/sh
#Boo Bisecting Tool
#
#Usage:
#  $ git bisect start BAD_REV GOOD_REV   (eg. HEAD v0.8.1)
#
#  [to test against one testcase/source]
#  $ export TESTCASE=testcase.boo        (eg. tests/testcases/regression/BOO-1008-1.boo)
#  [to test against one testfixture]
#  $ export TESTFIXTURE=fixture			 (eg. BooCompiler.Semantics [no .Tests.dll])
#
#  with no TESTCASE/TESTFIXTURE the whole testsuite is run
#
#  $ git bisect run extras/bisect.sh
#
#Enjoy!
#

if [ ! -z "$TESTCASE" ] && [ ! -z "$TESTFIXTURE" ]; then
	echo "!!! Both TESTCASE and TESTFIXTURE environment variable are set. Please make your mind!"
	exit 255
fi


#compile
nant
if [ "$?" -ne "0" ]; then
	echo "!!! SKIP (cannot build)"
	exit 125
fi

#test
if [ ! -z "$TESTCASE" ]; then
	build/booc.exe $BOOC_OPTIONS $TESTCASE
	BOOC_EXITCODE="$?"
fi
if [ ! -z "$TESTFIXTURE" ]; then
	nant -D:fixture="$TESTFIXTURE" test
	BOOC_EXITCODE="$?"
fi
if [ -z "$BOOC_EXITCODE" ]; then
	nant test
	BOOC_EXITCODE="$?"
fi

#return result to git
if [ "$BOOC_EXITCODE" != "0" ]; then
	echo "!!! BAD"
	exit 1
fi
echo "!!! good"
exit 0

