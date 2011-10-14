

class Factory:

	_prefix = null
	
	def constructor(prefix):
		_prefix = prefix
		
	def Create(suffix):
		return { item | return "${_prefix}${item}${suffix}" }
		
c1 = Factory("<").Create(">")
c2 = Factory("{{").Create("}}")

assert "<Yellow!>" == c1("Yellow!")
assert "{{Zeng!}}" == c2("Zeng!")
