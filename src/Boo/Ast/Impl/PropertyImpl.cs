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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Property;
			}
		}
		public Method Getter
		{
			get
			{
				return _getter;
			}
			
			set
			{
				
				if (_getter != value)
				{
					_getter = value;
					if (null != _getter)
					{
						_getter.InitializeParent(this);
					}
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
				
				if (_setter != value)
				{
					_setter = value;
					if (null != _setter)
					{
						_setter.InitializeParent(this);
					}
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
				
				if (_type != value)
				{
					_type = value;
					if (null != _type)
					{
						_type.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Property thisNode = (Property)this;
			Property resultingTypedNode = thisNode;
			transformer.OnProperty(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
