_, fname = System.Environment.GetCommandLineArgs()
for line in Boo.IO.TextFile(fname):
	print("print('${line}')")
