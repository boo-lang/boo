namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public class EntityFormatter
	{
		public virtual string FormatType(IType type)
		{
			var callableType = type as ICallableType;
			if (callableType != null && callableType.IsAnonymous)
				return callableType.GetSignature().ToString();
			return type.FullName;
		}
	}
}