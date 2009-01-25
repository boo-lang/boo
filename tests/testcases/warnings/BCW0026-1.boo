"""
BCW0026-1.boo(9,9): BCW0026: WARNING: Likely typo in member name 'Constructor'. Did you mean 'constructor'?
BCW0026-1.boo(11,9): BCW0026: WARNING: Likely typo in member name 'contructor'. Did you mean 'constructor'?
BCW0026-1.boo(15,9): BCW0026: WARNING: Likely typo in member name 'destructr'. Did you mean 'destructor'?
BCW0026-1.boo(18,16): BCW0026: WARNING: Likely typo in member name 'op_addition'. Did you mean 'op_Addition'?
BCW0026-1.boo(21,16): BCW0026: WARNING: Likely typo in member name 'op_Inplicit'. Did you mean 'op_Implicit'?
"""
class StickyKeyboard:
	def Constructor(s as string):
		pass
	def contructor():
		pass
	def conntrucctor(): #distance>1, skip
		pass
	def destructr():
		pass

	static def op_addition(x as StickyKeyboard, y as StickyKeyboard) as int:
		return 0

	static def op_Inplicit(s as StickyKeyboard) as bool:
		return false

	def constructor():
		pass

	def destructor():
		pass

	static def op_Implicit(s as StickyKeyboard) as bool:
		return true

