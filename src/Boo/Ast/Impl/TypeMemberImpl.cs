using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class TypeMemberImpl : Node, INodeWithAttributes
	{
		protected TypeMemberModifiers _modifiers;
		protected string _name;
		protected AttributeCollection _attributes;
		
		protected TypeMemberImpl()
		{
			_attributes = new AttributeCollection(this);
 		}
		
		protected TypeMemberImpl(TypeMemberModifiers modifiers, string name)
		{
			_attributes = new AttributeCollection(this);
 			Modifiers = modifiers;
			Name = name;
		}
		
		protected TypeMemberImpl(antlr.Token token, TypeMemberModifiers modifiers, string name) : base(token)
		{
			_attributes = new AttributeCollection(this);
 			Modifiers = modifiers;
			Name = name;
		}
		
		internal TypeMemberImpl(antlr.Token token) : base(token)
		{
			_attributes = new AttributeCollection(this);
 		}
		
		internal TypeMemberImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_attributes = new AttributeCollection(this);
 		}
		
		public TypeMemberModifiers Modifiers
		{
			get
			{
				return _modifiers;
			}
			
			set
			{
				
				if (_modifiers != value)
				{
					_modifiers = value;
				}
			}
		}
		public string Name
		{
			get
			{
				return _name;
			}
			
			set
			{
				
				_name = value;
			}
		}
		public AttributeCollection Attributes
		{
			get
			{
				return _attributes;
			}
			
			set
			{
				
				if (_attributes != value)
				{
					_attributes = value;
					if (null != _attributes)
					{
						_attributes.InitializeParent(this);
					}
				}
			}
		}
	}
}
