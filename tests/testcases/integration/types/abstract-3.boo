import BooCompiler.Tests

class Concrete(AbstractClass):

	[property(Token)] _token
	
	override def ToString():
		return "${_token}"
	
c = Concrete(Token: "Hello!")
assert "Hello!" == c.Token
