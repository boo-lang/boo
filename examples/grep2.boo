import System // Environment
import System.IO // Directory
import Boo.IO // TextFile

_, glob, expression = Environment.GetCommandLineArgs()
for index, arg in enumerate(Environment.GetCommandLineArgs()):
	print("${index}: ${arg}")
