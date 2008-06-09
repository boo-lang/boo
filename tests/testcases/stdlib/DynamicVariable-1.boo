"""
True
False
True
False
True
"""
variable = DynamicVariable[of bool]()
variable.With(true):
	print variable.Value
	variable.With(false):
		print variable.Value
		variable.With(true):
			print variable.Value
		print variable.Value
	print variable.Value
