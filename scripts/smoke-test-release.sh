#!/bin/sh
# Tests an unpacked release: that its zip matches it, that its tools run, and
# that what its compiler writes actually runs.
#
#   $ scripts/smoke-test-release.sh <install-dir> <release.zip>
#
# <install-dir> is the unpacked release, and is expected to be on the PATH.

set -e

if [ $# -ne 2 ]; then
	echo "usage: smoke-test-release.sh <install-dir> <release.zip>" >&2
	exit 1
fi

install=$1
zip=$2

# pwd -W is the Windows spelling of a path under Git Bash, which a native
# Python needs; elsewhere there is only one spelling.
here=$(cd "$(dirname "$0")" && { pwd -W 2>/dev/null || pwd; })

# Probed by running it: on Windows, python3 can be a Store shim that only
# prints an install prompt.
python=
for candidate in python3 python; do
	if "$candidate" -c 'pass' >/dev/null 2>&1; then
		python=$candidate
		break
	fi
done
if [ -z "$python" ]; then
	echo "smoke-test-release: python is needed to check the zip." >&2
	exit 1
fi

"$python" "$here/check-release-zip.py" "$zip" "$install"

work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT
cd "$work"

printf 'print "hello from boo"\n' > hello.boo

output=$(booi hello.boo)
[ "$output" = "hello from boo" ]

# What booc writes is an assembly, not a program: running it takes a
# runtimeconfig to say which runtime to use, and Boo.Lang beside it.
booc -out:hello.exe hello.boo
printf '%s\n' '{"runtimeOptions":{"tfm":"net10.0","framework":{"name":"Microsoft.NETCore.App","version":"10.0.0"}}}' > hello.runtimeconfig.json
cp "$install/Boo.Lang.dll" .
output=$(dotnet hello.exe)
[ "$output" = "hello from boo" ]
