import System

trunk = getTrunkUri() 
stable = Uri(trunk, "branches/stable")

print(svn("rm ${stable} -m 'stable branch update'"))
print(svn("cp ${trunk} ${stable} -m 'stable branch update'"))



