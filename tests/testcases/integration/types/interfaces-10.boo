"""
System.String
get_Name
null
System.String
null
set_Name
System.String
get_Name
set_Name
"""
import System.Reflection

interface IFoo:
	Name as string:
		get
		
interface IBar:
	Name as string:
		set


interface IBaz:
	Name as string:
		get
		set
		
def dump(property as PropertyInfo):
	print(property.PropertyType)
	
	getter = property.GetGetMethod()
	if getter is null:
		print("null")
	else:
		print(getter.Name)
	setter = property.GetSetMethod()
	if setter is null:
		print("null")
	else:
		print(setter.Name)

dump(typeof(IFoo).GetProperties()[0])
dump(typeof(IBar).GetProperties()[0])
dump(typeof(IBaz).GetProperties()[0])
