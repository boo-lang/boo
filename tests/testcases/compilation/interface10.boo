"""
True
False
"""
interface IFlag:
	Value as bool:
		get
		set
		
class Flag(IFlag):
	[property(Value)]
	_value as bool
	
def printFlag(flag as IFlag):
	print(flag.Value)
	
f = Flag(Value: true)
printFlag(f)
f.Value = false
printFlag(f)
