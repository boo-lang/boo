import System
import System.IO
import System.Runtime.Serialization.Formatters.Binary

def serialize(obj):
	stream = MemoryStream()
	BinaryFormatter().Serialize(stream, obj)
	return stream.ToArray()
	
def deserialize(buffer as (byte)):
	return BinaryFormatter().Deserialize(MemoryStream(buffer))

array = (1, 2, 3)
array = deserialize(serialize(array))
print(join(array))
