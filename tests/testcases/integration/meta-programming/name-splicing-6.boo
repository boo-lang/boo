"""
def foo():
	return self.bar
"""
fieldName = "bar"
code = [|
	def foo():
		return self.$fieldName
|]
print code.ToCodeString()


