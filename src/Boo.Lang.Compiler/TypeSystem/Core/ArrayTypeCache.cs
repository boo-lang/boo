using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Core
{
	public class ArrayTypeCache
	{
		private MemoizedFunction<int, IArrayType> _arrayTypes;

		public ArrayTypeCache(IType elementType)
		{
			_arrayTypes = new MemoizedFunction<int, IArrayType>(newRank => new ArrayType(elementType, newRank));
		}

		public IArrayType MakeArrayType(int rank)
		{
			return _arrayTypes.Invoke(rank);
		}
	}
}