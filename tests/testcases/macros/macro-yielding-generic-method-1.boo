"""
System.Int32
System.String
"""
macro printTypeDef:
	yield [|
		def printType[of T]():
			print T
	|]
	
printTypeDef
printType[of int]()
printType[of string]()
