#!/bin/sh
# Runs the test suite on Alpine, against musl rather than glibc.
#
# CI covers Ubuntu, macOS and Windows, all of them glibc, so nothing there
# would catch a musl-only problem. This is the gap filler, and it is meant to
# be run by hand rather than on every push.
#
#   $ scripts/test-alpine.sh                 # the whole suite
#   $ scripts/test-alpine.sh --filter Lexer  # passed through to dotnet test
#
# Needs podman with a running machine. On macOS that means `podman machine
# start` first, which boots a Linux VM.

set -e

root=$(cd "$(dirname "$0")/.." && pwd)
image=boo-alpine

if ! command -v podman >/dev/null 2>&1; then
	echo "test-alpine: podman is not on PATH." >&2
	exit 1
fi

if ! podman info >/dev/null 2>&1; then
	echo "test-alpine: podman is installed but not running." >&2
	echo "             On macOS, start it with: podman machine start" >&2
	exit 1
fi

echo "test-alpine: building $image"
podman build --tag "$image" --file "$root/Dockerfile" "$root"

echo "test-alpine: running the suite"
if [ $# -eq 0 ]; then
	podman run --rm "$image"
else
	podman run --rm "$image" dotnet test Boo.slnx --configuration Release "$@"
fi
