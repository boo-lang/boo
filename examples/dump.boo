"""
Lists the string representation of every object serialized
to a file.
"""
import System.Console
import System.IO
import System.Runtime.Serialization.Formatters.Binary
	
_, fname = Environment.GetCommandLineArgs()

formatter = BinaryFormatter()
stream = File.OpenRead(fname)
WriteLine(formatter.Deserialize(stream)) while stream.Position < stream.Size
