namespace Boo.Ast.Compilation.Binding
{
	public class TypeReferenceBinding : ITypedBinding, INamespace
	{
		ITypeBinding _type;
		
		public TypeReferenceBinding(ITypeBinding type)
		{
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _type.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.TypeReference;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _type;
			}
		}
		
		public IBinding Resolve(string name)
		{
			return  _type.Resolve(name);
		}
		
		public override string ToString()
		{
			return _type.ToString();
		}
	}
}
