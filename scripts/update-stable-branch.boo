import System

trunk = Uri(/URL:\s(.+)/.Match(shell("svn", "info")).Groups[1].Value)
stable = Uri(trunk, "branches/stable")

print(shell("svn", "cp ${trunk} ${stable} -m 'stable branch update'"))



