using Boo.IO
using System.IO
		
def InsertLicense(fname as string, license as string):
	print(fname)
	contents = TextFile.ReadFile(fname)
	f = StreamWriter(fname)
	f.WriteLine(license)
	f.Write(contents)
	f.Close()

def ScanDirectory(name as string, license as string):
	for fname in Directory.GetFiles(name, "*.cs"):
		f = TextFile(fname)
		firstLine = f.ReadLine()
		f.Close()
		InsertLicense(fname, license) if firstLine !~ "#license"
		
	for dir in Directory.GetDirectories(name):
		ScanDirectory(dir, license)

license = TextFile.ReadFile("license.txt")
ScanDirectory(".", license)
