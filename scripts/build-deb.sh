#!/bin/sh
# Builds a .deb of booc, booi and booish into artifacts/.
#
#   $ scripts/build-deb.sh
#
# The package is self contained: it carries the .NET runtime, so it depends on
# nothing but the C library and ICU. That also makes it architecture specific,
# and the architecture is whichever one podman is running.
#
# Needs podman with a running machine. On macOS that means `podman machine
# start` first.

set -e

root=$(cd "$(dirname "$0")/.." && pwd)
image=boo-deb
out="$root/artifacts"

if ! command -v podman >/dev/null 2>&1; then
	echo "build-deb: podman is not on PATH." >&2
	exit 1
fi

if ! podman info >/dev/null 2>&1; then
	echo "build-deb: podman is installed but not running." >&2
	echo "           On macOS, start it with: podman machine start" >&2
	exit 1
fi

mkdir -p "$out"

echo "build-deb: building $image"
podman build --tag "$image" --file "$root/packaging/deb/Dockerfile" "$root"

echo "build-deb: packaging"
podman run --rm --volume "$out:/out" "$image"

echo "build-deb: wrote"
ls -1 "$out"/*.deb | sed 's|^|           |'
