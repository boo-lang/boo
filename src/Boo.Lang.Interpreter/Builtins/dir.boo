namespace Boo.Lang.Interpreter.Builtins

def dir([required] obj):
	type = (obj as System.Type) or obj.GetType()
	return array(
			member for member in type.GetMembers()
			unless (method=(member as System.Reflection.MethodInfo)) is not null
			and method.IsSpecialName)