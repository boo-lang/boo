import System

version = Project.Properties["distro-version"]

trunk = getTrunkUri() 
tag = Uri(trunk, "tags/${version}")

print(svn("cp ${trunk} ${tag} -m '${version} release'"))



