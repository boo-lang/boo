import System.IO
import System.Runtime.Serialization.Formatters.Binary

class Person:
	_name as string
	
	def constructor(name):
		_name = name
		
	override def ToString():
		return _name

def serialize(fname, obj):
	using stream=File.OpenWrite(fname):
		BinaryFormatter().Serialize(stream, obj)

serialize("\\temp\\p.dat", Person("Homer Simpson"))
