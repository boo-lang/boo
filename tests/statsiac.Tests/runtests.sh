#!/bin/bash
current_dir=`dirname $0`
export MONO_PATH=$current_dir/../../../build:/usr/local/lib/nunit
$current_dir/../../statsiac -debug+ -lib:$current_dir/../../build,/usr/local/lib/nunit -r:Statsia.dll -t:library $current_dir/BroadcastingTests.boo -o:$current_dir/build/statsiac.Tests.dll
if [ $? -ne 0 ]; then
    echo "(statsiac) COMPILE ERROR: Could not compile the tests."
    exit 1
fi
mono --debug /usr/local/lib/nunit/nunit-console.exe build/statsiac.Tests.dll
rm TestResult.xml