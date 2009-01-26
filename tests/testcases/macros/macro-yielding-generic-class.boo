"""
GenericType[of System.Int32]
GenericType[of System.String]
"""
macro genericTypeDef:
	yield [|
		class GenericType[of T]:
			override def ToString():
				return "GenericType[of ${T}]"
	|]
	
genericTypeDef
print GenericType[of int]()
print GenericType[of string]()
