using System
using Boo.IO

_, fname = Environment.GetCommandLineArgs()
for index, line in enumerate(TextFile(fname)):
	print("${index}: ${line}")
