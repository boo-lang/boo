import NUnit.Framework from nunit.framework
import System.IO
import System.Runtime.Serialization.Formatters.Binary

def save(o):
	stream=MemoryStream()
	BinaryFormatter().Serialize(stream, o)
	return stream.GetBuffer()
	
def load(data as (byte)):
	return BinaryFormatter().Deserialize(MemoryStream(data))

def make_counter(value as int):
	return <return ++value>
	
c1 = make_counter(10)
c2 = make_counter(20)

Assert.AreEqual(11, c1())
Assert.AreEqual(21, c2())

saved = save(c1)
Assert.AreEqual(12, c1())

# restore
c1 = load(saved)
Assert.AreEqual(12, c1())
