"""
def foo():
	return _foo
"""
field = [|
	private _foo as string
|]
code = [|
	def foo():
		return $field
|]
print code.ToCodeString()


