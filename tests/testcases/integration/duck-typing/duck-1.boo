import System

class LooksLikeADuck:
	def Quack():
		return "quack!"

struct QuacksLikeADuck:
	dummy as object # keep the verifier happy
	def Quack():
		return "quack!"
		
class NotExactlyADuck:
	def Bark():
		return "au!"

def quack(obj as duck):
	assert "quack!" == obj.Quack()	
	
quack(LooksLikeADuck())
quack(QuacksLikeADuck())

try:
	quack(NotExactlyADuck())
	raise "Expected MissingMethodException!"
except x as MissingMethodException:
	pass
	

