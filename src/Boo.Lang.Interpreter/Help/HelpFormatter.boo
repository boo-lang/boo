namespace Boo.Lang.Interpreter.Help

import System
import System.Reflection

class HelpFormatter:
	
	indent as string
	
	def constructor(indent as string):
		self.indent = indent
		
	def GenerateFormattedLinesFor(type as Type):
		if type.IsInterface:
			typeDef = "interface"
			baseTypes = array(GetBooTypeName(t) for t in type.GetInterfaces())
		else:
			typeDef = "class"
			baseTypes = (GetBooTypeName(type.BaseType),) + array(GetBooTypeName(t) for t in type.GetInterfaces())
			
		yield "${typeDef} ${type.Name}(${join(baseTypes, ', ')}):"
		yield ""
		
		for ctor in type.GetConstructors():
			yield "${indent}def constructor(${DescribeParameters(ctor.GetParameters())})"
			yield ""
			
		sortByName = def (lhs as MemberInfo, rhs as MemberInfo):
			return lhs.Name.CompareTo(rhs.Name)
			
		for f as FieldInfo in List(type.GetFields()).Sort(sortByName):
			yield "${indent}public ${DescribeField(f)}"
			yield ""
			
		for p as PropertyInfo in List(type.GetProperties()).Sort(sortByName):
			yield "${indent}${DescribeProperty(p)}:"
			yield "${indent}${indent}get" if p.GetGetMethod() is not null
			yield "${indent}${indent}set" if p.GetSetMethod() is not null
			yield ""
		
		for m as MethodInfo in List(type.GetMethods()).Sort(sortByName):
			continue if m.IsSpecialName
			yield "${indent}${DescribeMethod(m)}"
			yield ""
			
		for e as EventInfo in List(type.GetEvents()).Sort(sortByName):
			yield "${indent}${DescribeEvent(e)}"
			yield ""