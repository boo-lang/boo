namespace GacLibrary

import System
import System.Reflection

class GacType:
	def Load(typeName as string):
		type = Type.GetType(typeName)
		return if type is null
		print "found '${type.Name}' from assembly '${type.Assembly.FullName}'"
		return type
		
[assembly: AssemblyKeyFile("../GacLibrary/GacLibrary.snk")]
