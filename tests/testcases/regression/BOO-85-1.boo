import System.IO
import NUnit.Framework

def PathJoin(files):
	buffer = System.Text.StringBuilder()
	for fname in files:
		buffer.Append(fname)
		buffer.Append(Path.PathSeparator)
	return buffer.ToString()[:-1]

files = "foo.bar", "spam.eggs"
separator = string((Path.PathSeparator,))
Assert.AreEqual(PathJoin(files), join(files, separator))
