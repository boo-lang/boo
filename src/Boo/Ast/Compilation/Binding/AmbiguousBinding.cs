using System;

namespace Boo.Ast.Compilation.Binding
{
	public class AmbiguousBinding : IBinding
	{
		IBinding[] _bindings;
		
		public AmbiguousBinding(IBinding[] bindings)
		{
			if (null == bindings)
			{
				throw new ArgumentNullException("bindings");
			}
			if (0 == bindings.Length)
			{
				throw new ArgumentException("bindings");
			}
			_bindings = bindings;
		}
		
		public string Name
		{
			get
			{
				return _bindings[0].Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Ambiguous;
			}
		}
		
		public IBinding[] Bindings
		{
			get
			{
				return _bindings;
			}
		}
		
		public override string ToString()
		{
			return "";
		}
	}
}
