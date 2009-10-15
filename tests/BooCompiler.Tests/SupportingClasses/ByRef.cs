namespace BooCompiler.Tests.SupportingClasses
{
	public class ByRef
	{
		public static void SetValue(int value, ref int output)
		{
			output = value;
		}
		
		public static void SetRef(object value, ref object output)
		{
			output = value;
		}
		
		public static void ReturnValue(int value, out int output)
		{
			output = value;
		}
		
		public static void ReturnRef(object value, out object output)
		{
			output = value;
		}
	}
}