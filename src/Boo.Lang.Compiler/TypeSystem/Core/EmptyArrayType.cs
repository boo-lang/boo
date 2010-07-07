namespace Boo.Lang.Compiler.TypeSystem.Core
{
	public class EmptyArrayType : ArrayType
	{
		public static readonly IArrayType Default = new EmptyArrayType();

		EmptyArrayType() : base(Null.Default, 1)
		{
		}

		protected override bool IsSubclassOfGenericEnumerable(IType other)
		{
			return true;
		}
	}
}
