import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Concrete(AbstractClass):

	[property(Token)] _token = null
	
	override def ToString():
		return "${_token}"
	
c = Concrete(Token: "Hello!")
assert "Hello!" == c.Token
