namespace Boo.Lang.Interpreter.Help

import System.Reflection
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.PatternMatching

def DescribeEntity(entity as IEntity):
	match entity:
		case method = ExternalMethod(MethodInfo: MethodInfo()):
			return DescribeMethod(method.MethodInfo)
		case field = ExternalField():
			return DescribeField(field.FieldInfo)
		case property = ExternalProperty():
			return DescribeProperty(property.PropertyInfo)
		case e = ExternalEvent():
			return DescribeEvent(e.EventInfo)
		otherwise:
			entity.ToString()
		
def DescribeEvent(e as EventInfo):
	return "${DescribeModifiers(e)}event ${e.Name} as ${e.EventHandlerType}"
		
def DescribeProperty(p as PropertyInfo):
	modifiers = DescribeModifiers(p)
	params = DescribePropertyParameters(p.GetIndexParameters())
	return "${modifiers}${p.Name}${params} as ${GetBooTypeName(p.PropertyType)}"
		
def DescribeField(f as FieldInfo):
	return "${DescribeModifiers(f)}${f.Name} as ${GetBooTypeName(f.FieldType)}"
		
def DescribeMethod(m as MethodInfo):
	returnType = GetBooTypeName(m.ReturnType)
	modifiers = DescribeModifiers(m)
	return "${modifiers}def ${m.Name}(${DescribeParameters(m.GetParameters())}) as ${returnType}"
		
def DescribeModifiers(f as FieldInfo):
	return "static " if f.IsStatic
	return ""
		
def DescribeModifiers(m as MethodBase):
	return "static " if m.IsStatic
	return ""
	
def DescribeModifiers(e as EventInfo):
	return DescribeModifiers(e.GetAddMethod(true) or e.GetRemoveMethod(true))
	
def DescribeModifiers(p as PropertyInfo):
	accessor = p.GetGetMethod(true) or p.GetSetMethod(true)
	return DescribeModifiers(accessor)
		
def DescribePropertyParameters(parameters as (ParameterInfo)):
	return "" if 0 == len(parameters)
	return "(${DescribeParameters(parameters)})"
		
def DescribeParameters(parameters as (ParameterInfo)):
	return join(DescribeParameter(p) for p in parameters, ", ")
	
def DescribeParameter(p as ParameterInfo):
	return "${p.Name} as ${GetBooTypeName(p.ParameterType)}"
	
def GetBooTypeName(type as System.Type) as string:
	return "(${GetBooTypeName(type.GetElementType())})" if type.IsArray
	return "object" if object is type
	return "string" if string is type
	return "void" if void is type
	return "bool" if bool is type		
	return "byte" if byte is type
	return "char" if char is type
	return "sbyte" if sbyte is type
	return "short" if short is type
	return "ushort" if ushort is type
	return "int" if int is type
	return "uint" if uint is type
	return "long" if long is type
	return "ulong" if ulong is type
	return "single" if single is type
	return "double" if double is type
	return "date" if date is type
	return "timespan" if timespan is type
	return "regex" if regex is type
	return type.FullName