import System // Environment
import System.IO // Directory
import Boo.IO // TextFile

_, glob, expression = Environment.GetCommandLineArgs()
print("searching for ${expression} in ${glob}...")
