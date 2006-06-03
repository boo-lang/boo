"""
Range[A3:B4]
luae => 42
"""
class Sheet:
	Range[expression]:
		get:
			print "Range[${expression}]"
			return RangeImpl()
			
	class RangeImpl:
		Value[expression]:
			set:
				print expression, "=>", value				

s = Sheet()				
s.Range['A3:B4'].Value["luae"] = 42
