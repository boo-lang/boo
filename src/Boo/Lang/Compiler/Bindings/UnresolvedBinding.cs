namespace Boo.Lang.Compiler.Bindings
{
	public class UnresolvedBinding : NullBinding
	{
		IBinding _resolved;
		
		public UnresolvedBinding()
		{
		}
		
		public override string Name
		{
			get
			{
				return "Unresolved";
			}
		}
		
		public override BindingType BindingType
		{
			get
			{
				return BindingType.Unresolved;
			}
		}
		
		public override bool IsSubclassOf(ITypeBinding other)
		{
			return true;
		}
		
		public IBinding Resolved
		{
			get
			{
				return _resolved;
			}
			
			set
			{
				_resolved = value;
			}
		}
	}
}
