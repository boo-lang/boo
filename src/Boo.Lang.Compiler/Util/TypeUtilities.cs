namespace Boo.Lang.Compiler.Util
{
	using System;
	
	public class TypeUtilities
	{
		public static string GetFullName(Type type)
		{
			if (type.IsByRef) return "ref " + GetFullName(type.GetElementType());
			if (type.DeclaringType != null) return GetFullName(type.DeclaringType) + "." + TypeName(type);		
			
			// HACK: Some constructed generic types report a FullName of null
			if (type.FullName == null) 
			{
				string[] argumentNames = Array.ConvertAll<Type, string>(
					type.GetGenericArguments(),
					delegate(Type t) { return GetFullName(t); });
				
				return string.Format(
					"{0}[{1}]",
					GetFullName(type.GetGenericTypeDefinition()),
					string.Join(", ", argumentNames));
				
			}
			return type.FullName;
		}

	    public static string TypeName(Type type)
		{
			if (!type.IsGenericTypeDefinition) return type.Name;
			string name = type.Name;
			int index = name.LastIndexOf('`');
			if (index < 0) return name;
			return name.Substring(0, index);
		}
	}
}
