using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class MethodImpl : TypeMember
	{
		protected ParameterDeclarationCollection _parameters;
		protected TypeReference _returnType;
		protected AttributeCollection _returnTypeAttributes;
		protected Block _body;
		protected LocalCollection _locals;
		
		protected MethodImpl()
		{
			_parameters = new ParameterDeclarationCollection(this);
			_returnTypeAttributes = new AttributeCollection(this);
			Body = new Block();
 		}
		
		protected MethodImpl(TypeReference returnType)
		{
			_parameters = new ParameterDeclarationCollection(this);
			_returnTypeAttributes = new AttributeCollection(this);
			Body = new Block();
 			ReturnType = returnType;
		}
		
		protected MethodImpl(antlr.Token token, TypeReference returnType) : base(token)
		{
			_parameters = new ParameterDeclarationCollection(this);
			_returnTypeAttributes = new AttributeCollection(this);
			Body = new Block();
 			ReturnType = returnType;
		}
		
		internal MethodImpl(antlr.Token token) : base(token)
		{
			_parameters = new ParameterDeclarationCollection(this);
			_returnTypeAttributes = new AttributeCollection(this);
			Body = new Block();
 		}
		
		internal MethodImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_parameters = new ParameterDeclarationCollection(this);
			_returnTypeAttributes = new AttributeCollection(this);
			Body = new Block();
 		}
		public ParameterDeclarationCollection Parameters
		{
			get
			{
				return _parameters;
			}
			
			set
			{
				_parameters = value;
				if (null != _parameters)
				{
					_parameters.InitializeParent(this);
				}
			}
		}
		public TypeReference ReturnType
		{
			get
			{
				return _returnType;
			}
			
			set
			{
				_returnType = value;
				if (null != _returnType)
				{
					_returnType.InitializeParent(this);
				}
			}
		}
		public AttributeCollection ReturnTypeAttributes
		{
			get
			{
				return _returnTypeAttributes;
			}
			
			set
			{
				_returnTypeAttributes = value;
				if (null != _returnTypeAttributes)
				{
					_returnTypeAttributes.InitializeParent(this);
				}
			}
		}
		public Block Body
		{
			get
			{
				return _body;
			}
			
			set
			{
				_body = value;
				if (null != _body)
				{
					_body.InitializeParent(this);
				}
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public LocalCollection Locals
		{
			get
			{
				if (null == _locals)
				{
					_locals = new LocalCollection(this);
				}
				return _locals;
			}
		}
	}
}
