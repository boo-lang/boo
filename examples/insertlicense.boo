import Boo.IO
import System.IO
		
def InsertLicense(fname as string, license as string):
	print(fname)
	contents = TextFile.ReadFile(fname)
	using writer = StreamWriter(fname, false, System.Text.Encoding.UTF8):
		writer.WriteLine(license)
		writer.WriteLine()
		writer.Write(contents)

def ScanDirectory(name as string, license as string):
	for fname in Directory.GetFiles(name, "*.cs"):		
		InsertLicense(fname, license) if GetFirstLine(fname) != "#region license"
		
	for dir in Directory.GetDirectories(name):
		ScanDirectory(dir, license)
		
def GetFirstLine(fname as string):
	using f = TextFile(fname):
		return f.ReadLine()

license = TextFile.ReadFile("license.txt")
ScanDirectory("src", license)
