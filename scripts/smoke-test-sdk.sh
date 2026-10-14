#!/bin/sh
# Tests a packed Boo.Sdk by using it the way a user's project would.
#
#   $ scripts/smoke-test-sdk.sh <directory holding Boo.Sdk.<version>.nupkg> <version>
#
# The package goes into a feed of its own, with a NuGet cache of its own, so
# nothing installed on the machine can stand in for it. Two projects are built
# from it, a library and an app that references the library, and the app is run.

set -e

if [ $# -ne 2 ]; then
	echo "usage: smoke-test-sdk.sh <package-directory> <version>" >&2
	exit 1
fi

version=$2
package="$1/Boo.Sdk.$version.nupkg"
if [ ! -f "$package" ]; then
	echo "smoke-test-sdk: $package was not found. Is the version in the package the one released?" >&2
	exit 1
fi

work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT

# pwd -W is the Windows spelling of a path under Git Bash, which NuGet needs;
# elsewhere there is only one spelling.
spell() { (cd "$1" && { pwd -W 2>/dev/null || pwd; }); }

mkdir "$work/feed" "$work/lib" "$work/app"
cp "$package" "$work/feed/"
feed=$(spell "$work/feed")

cat > "$work/nuget.config" <<EOF
<configuration>
  <packageSources>
    <clear />
    <add key="package" value="$feed" />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

cat > "$work/lib/lib.booproj" <<EOF
<Project Sdk="Boo.Sdk/$version">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
EOF

cat > "$work/lib/Greeter.boo" <<'EOF'
namespace Greetings

class Greeter:
	def Greet(name as string) as string:
		return "Hello, $name!"
EOF

cat > "$work/app/app.booproj" <<EOF
<Project Sdk="Boo.Sdk/$version">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../lib/lib.booproj" />
  </ItemGroup>
</Project>
EOF

cat > "$work/app/main.boo" <<'EOF'
import Greetings

print Greeter().Greet("world")
EOF

# An empty cache of its own, so the SDK is restored from the package under test.
NUGET_PACKAGES=$(spell "$work")/packages
export NUGET_PACKAGES
export DOTNET_NOLOGO=1 DOTNET_CLI_TELEMETRY_OPTOUT=1

cd "$work/app"

for configuration in Debug Release; do
	dotnet build --configuration "$configuration" --nologo --verbosity quiet

	# Each program is run on its own line and its output compared on the next:
	# inside [ "$(...)" ] the program's exit status is thrown away.
	output=$(dotnet "bin/$configuration/net10.0/app.dll")
	[ "$output" = "Hello, world!" ]
done

# A second build of what has not changed must leave booc alone.
dotnet build --configuration Release --nologo --verbosity detailed > "$work/rebuild.log"
if grep -q 'booc.dll" -noconfig' "$work/rebuild.log"; then
	echo "smoke-test-sdk: an unchanged project was compiled again." >&2
	exit 1
fi

# The library has to pack, which is where missing symbol files show up.
cd "$work/lib"
dotnet pack --configuration Release --nologo --verbosity quiet --output "$work/packed"
ls "$work"/packed/*.nupkg > /dev/null
