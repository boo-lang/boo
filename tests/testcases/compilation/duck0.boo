import System
import NUnit.Framework

class QuacksLikeADuck:
	def Quack():
		return "quack!"
		
class ActsLikeADuck:
	def Quack():
		return "quack!"
		
class NotExactlyADuck:
	def Bark():
		return "au!"

def quack(obj as duck):
	Assert.AreEqual("quack!", obj.Quack())	
	
quack(QuacksLikeADuck())
quack(ActsLikeADuck())

try:
	quack(NotExactlyADuck())
	Assert.Fail("Expected MissingMethodException!")
except x as MissingMethodException:
	pass
	

