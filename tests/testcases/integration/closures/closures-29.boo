class Console:

	public WriteLine as callable(object) as object
	
	def constructor():
		WriteLine = { value | return value }
	
	
c = Console()
assert 34 == c.WriteLine(34)
assert "42" == c.WriteLine("42")
