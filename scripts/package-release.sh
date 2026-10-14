#!/bin/sh
# Builds a self contained archive of booc, booi and booish for one runtime.
#
#   $ scripts/package-release.sh linux-x64 artifacts
#
# Writes boo-<version>-<rid>.tar.gz and boo-<version>-<rid>.zip into the output
# directory, with the same contents: a tarball for unix, and a zip for Windows,
# where File Explorer opens it natively. The archive is flat: the three programs and the runtime they carry
# sit at its root, so unpacking it into a directory and putting that directory
# on the PATH is the whole install.
#
# Publishing follows packaging/deb/build.sh, which explains why each tool is
# published on its own and merged afterwards.

set -e

if [ $# -ne 2 ]; then
	echo "usage: package-release.sh <rid> <output-dir>" >&2
	exit 1
fi

rid=$1
root=$(cd "$(dirname "$0")/.." && pwd)
mkdir -p "$2"
out=$(cd "$2" && pwd)
version=$(tr -d '[:space:]' < "$root/version.txt")
name=boo-$version-$rid

stage=$(mktemp -d)
trap 'rm -rf "$stage"' EXIT

for tool in booc booi booish; do
	case $tool in
		booc) project=src/booc/booc.csproj ;;
		*) project=src/$tool/$tool.booproj ;;
	esac
	echo "publishing $tool for $rid"
	dotnet publish "$root/$project" \
		--configuration Release \
		--runtime "$rid" \
		--self-contained \
		--output "$stage/$tool" \
		--verbosity quiet --nologo
done

# booi and booish each publish a copy of booc, built as a tool and so not self
# contained. Keep each tool's own files only.
for tool in booc booi booish; do
	for other in booc booi booish; do
		[ "$tool" = "$other" ] && continue
		rm -f "$stage/$tool/$other" "$stage/$tool/$other".*
	done
done

# The runtime files are identical across the three, so this leaves one copy.
mkdir "$stage/boo"
for tool in booc booi booish; do
	cp -a "$stage/$tool/." "$stage/boo/"
done

for tool in booc booi booish; do
	if ! grep -q includedFrameworks "$stage/boo/$tool.runtimeconfig.json"; then
		echo "package-release: $tool is not self contained" >&2
		exit 1
	fi
done

find "$stage/boo" -name '*.pdb' -delete

archive=$out/$name.tar.gz
# --force-local keeps GNU tar on Windows from reading the drive letter as a host.
case $rid in
	win-*) tar --force-local -czf "$archive" -C "$stage/boo" . ;;
	*) tar -czf "$archive" -C "$stage/boo" . ;;
esac

# Python's zipfile is the one zip writer every runner has. It is given each
# file's mode, so the apphosts stay executable for anyone unpacking it on unix.
# Probed by running it: on Windows, python3 can be a Store shim that only
# prints an install prompt.
python=
for candidate in python3 python; do
	if "$candidate" -c 'import zipfile' >/dev/null 2>&1; then
		python=$candidate
		break
	fi
done
if [ -z "$python" ]; then
	echo "package-release: python is needed to write the zip." >&2
	exit 1
fi
zip=$out/$name.zip
rm -f "$zip"
"$python" - "$stage/boo" "$zip" <<'PY'
import os, sys, zipfile

source, target = sys.argv[1], sys.argv[2]
with zipfile.ZipFile(target, "w", zipfile.ZIP_DEFLATED) as z:
    for folder, dirs, files in os.walk(source):
        dirs.sort()
        for name in sorted(files):
            path = os.path.join(folder, name)
            info = zipfile.ZipInfo.from_file(path, os.path.relpath(path, source))
            info.compress_type = zipfile.ZIP_DEFLATED
            with open(path, "rb") as f:
                z.writestr(info, f.read())
PY

echo "package-release: wrote $archive"
echo "package-release: wrote $zip"
