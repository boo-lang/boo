using Boo.IO
using System.IO
		
def InsertLicense(fname as string, license as string):
	print(fname)
	contents = TextFile.ReadFile(fname)
	f = StreamWriter(fname, false, System.Text.Encoding.UTF8)
	f.WriteLine(license)
	f.WriteLine()
	f.Write(contents)
	f.Close()

def ScanDirectory(name as string, license as string):
	for fname in Directory.GetFiles(name, "*.cs"):
		f = TextFile(fname)
		firstLine = f.ReadLine()
		f.Close()
		InsertLicense(fname, license) if firstLine != "#region license"
		
	for dir in Directory.GetDirectories(name):
		ScanDirectory(dir, license)

license = TextFile.ReadFile("license.txt")
ScanDirectory("src", license)
