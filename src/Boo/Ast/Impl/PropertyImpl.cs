using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class PropertyImpl : TypeMember
	{
		protected Method _getter;
		protected Method _setter;
		protected TypeReference _type;
		
		protected PropertyImpl()
		{
 		}
		
		protected PropertyImpl(Method getter, Method setter, TypeReference type)
		{
 			Getter = getter;
			Setter = setter;
			Type = type;
		}
		
		protected PropertyImpl(antlr.Token token, Method getter, Method setter, TypeReference type) : base(token)
		{
 			Getter = getter;
			Setter = setter;
			Type = type;
		}
		
		internal PropertyImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal PropertyImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public Method Getter
		{
			get
			{
				return _getter;
			}
			
			set
			{
				_getter = value;
				if (null != _getter)
				{
					_getter.InitializeParent(this);
				}
			}
		}
		public Method Setter
		{
			get
			{
				return _setter;
			}
			
			set
			{
				_setter = value;
				if (null != _setter)
				{
					_setter.InitializeParent(this);
				}
			}
		}
		public TypeReference Type
		{
			get
			{
				return _type;
			}
			
			set
			{
				_type = value;
				if (null != _type)
				{
					_type.InitializeParent(this);
				}
			}
		}
	}
}
