import System
import System.Xml.Serialization from System.Xml
import System.IO

class GameSession:

	public Name as string
	
	def constructor():
		pass
		
	def constructor(name as string):
		Name = name
	
class GameLogData:
	
	_sessions = []
	
	Sessions as (GameSession):
		get:
			return _sessions.ToArray(GameSession)
		set:
			_sessions.Clear()
			_sessions.Extend(value)
			
	def Add([required] session as GameSession):
		_sessions.Add(session)
	
data = GameLogData()
data.Add(GameSession("Foo"))
data.Add(GameSession("Bar"))

serializer = XmlSerializer(GameLogData)
serializer.Serialize(Console.Out, data)
