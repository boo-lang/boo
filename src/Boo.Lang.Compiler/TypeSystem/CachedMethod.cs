namespace Boo.Lang.Compiler.TypeSystem
{
	class CachedMethod
	{
		public readonly IMethod Value;

		public CachedMethod(IMethod value)
		{
			Value = value;
		}
	}
}