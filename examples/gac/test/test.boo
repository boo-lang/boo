import System
import GacLibrary from GacLibrary

class LocalType:
	pass
	
def test(typeName as string):
	yac = GacType()
	report(typeName, yac.Load(typeName))
	
def report(typeName, type):
	print "${typeName}: ${type}"
	
test("GacLibrary.GacType")
test("GacLibrary.GacType, GacLibrary")
test("LocalType")
test("LocalType, test")
test("PrivateType")
test("PrivateType, PrivateLibrary")

report("LocalType", Type.GetType("LocalType"))
report("LocalType, test", Type.GetType("LocalType, test"))
