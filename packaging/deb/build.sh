#!/bin/sh
# Builds the boo .deb. Runs inside the image in packaging/deb/Dockerfile;
# scripts/build-deb.sh is what starts that.

set -e

version=$(tr -d '[:space:]' < /src/version.txt)
arch=$(dpkg --print-architecture)
case "$arch" in
	amd64) rid=linux-x64 ;;
	arm64) rid=linux-arm64 ;;
	*) echo "build.sh: no RID mapped for $arch" >&2; exit 1 ;;
esac

root=/tmp/deb
stage=/tmp/stage
rm -rf "$root" "$stage"
mkdir -p "$root/DEBIAN" "$root/usr/lib/boo" "$root/usr/bin"

# Published one at a time and merged after, not straight into a shared
# directory. booi and booish build booc as a tool, and that build is not self
# contained; publishing them over the top of it replaces booc's runtimeconfig
# with one that asks for a shared runtime.
for tool in booc booi booish; do
	case $tool in
		booc) project=src/booc/booc.csproj ;;
		*) project=src/$tool/$tool.booproj ;;
	esac
	echo "publishing $tool"
	dotnet publish "/src/$project" \
		--configuration Release \
		--runtime "$rid" \
		--self-contained \
		--output "$stage/$tool" \
		--verbosity quiet --nologo
done

# booi and booish each publish a copy of booc, built as a tool and so not self
# contained. Drop every tool's copies of the other two, leaving each to be
# supplied only by its own publish.
for tool in booc booi booish; do
	for other in booc booi booish; do
		[ "$tool" = "$other" ] && continue
		rm -f "$stage/$tool/$other" "$stage/$tool/$other".*
	done
done

# The runtime files are identical across the three, so this leaves one copy.
for tool in booc booi booish; do
	cp -a "$stage/$tool/." "$root/usr/lib/boo/"
done

# Each app must carry its own runtime rather than ask for one.
for tool in booc booi booish; do
	config="$root/usr/lib/boo/$tool.runtimeconfig.json"
	if ! grep -q includedFrameworks "$config"; then
		echo "build.sh: $tool is not self contained" >&2
		exit 1
	fi
done

for tool in booc booi booish; do
	ln -sf /usr/lib/boo/$tool "$root/usr/bin/$tool"
done

find "$root/usr/lib/boo" -name '*.pdb' -delete

installed=$(du -ks "$root/usr" | cut -f1)
sed -e "s/@VERSION@/$version/" -e "s/@ARCH@/$arch/" -e "s/@SIZE@/$installed/" \
	/src/packaging/deb/control > "$root/DEBIAN/control"

dpkg-deb --build --root-owner-group "$root" "/out/boo_${version}_${arch}.deb"
dpkg-deb --info "/out/boo_${version}_${arch}.deb" | sed -n '2,8p'
