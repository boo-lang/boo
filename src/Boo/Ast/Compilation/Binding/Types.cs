using System;

namespace Boo.Ast.Compilation.Binding
{
	public class Types
	{
		public static readonly Type RuntimeServices = typeof(Boo.Lang.RuntimeServices);
		
		public static readonly Type List = typeof(Boo.Lang.List);
		
		public static readonly Type IEnumerable = typeof(System.Collections.IEnumerable);
		
		public static readonly Type IEnumerator = typeof(System.Collections.IEnumerator);
		
		public static readonly Type Object = typeof(object);
		
		public static readonly Type ObjectArray = Type.GetType("System.Object[]");
		
		public static readonly Type Void = typeof(void);
		
		public static readonly Type String = typeof(string);
		
		public static readonly Type Int = typeof(int);
		
		public static readonly Type Single = typeof(float);
		
		public static readonly Type Date = typeof(System.DateTime);
		
		public static readonly Type Bool = typeof(bool);
		
		public static readonly Type IntPtr = typeof(IntPtr);

	}
}
