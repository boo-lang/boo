using System // Environment
using System.IO // Directory
using Boo.IO // TextFile

_, glob, expression = Environment.GetCommandLineArgs()
for index, arg in enumerate(Environment.GetCommandLineArgs()):
	print("${index}: ${arg}")
