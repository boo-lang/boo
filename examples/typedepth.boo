import System
import System.Collections

def depth(t as Type) as int:
	if t.IsInterface:
		return GetInterfaceDepth(t)
	else:
		return GetClassDepth(t)

def GetInterfaceDepth(t as Type) as int:
	interfaces = t.GetInterfaces()
	if len(interfaces):
		return 1+cast(int, [GetInterfaceDepth(i) for i in interfaces].Sort()[0])
	return 1
	
def GetClassDepth(t as Type) as int:
	if t is object:
		return 0
	return 1 + GetClassDepth(t.BaseType)
	
def test(t as Type):
	print("${t}: ${depth(t)}")

print("="*40)
test(List)
test(Hash)
test(IEnumerable)
test(ICollection)
test(int)
