using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ParameterDeclarationImpl : Node, INodeWithAttributes
	{
		protected string _name;
		protected TypeReference _type;
		protected AttributeCollection _attributes;
		
		protected ParameterDeclarationImpl()
		{
			_attributes = new AttributeCollection(this);
 		}
		
		protected ParameterDeclarationImpl(string name, TypeReference type)
		{
			_attributes = new AttributeCollection(this);
 			Name = name;
			Type = type;
		}
		
		protected ParameterDeclarationImpl(antlr.Token token, string name, TypeReference type) : base(token)
		{
			_attributes = new AttributeCollection(this);
 			Name = name;
			Type = type;
		}
		
		internal ParameterDeclarationImpl(antlr.Token token) : base(token)
		{
			_attributes = new AttributeCollection(this);
 		}
		
		internal ParameterDeclarationImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_attributes = new AttributeCollection(this);
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
		
		public AttributeCollection Attributes
		{
			get
			{
				return _attributes;
			}
			
			set
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
