using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class AttributeImpl : Node, INodeWithArguments
	{
		protected string _name;
		protected ExpressionCollection _arguments;
		protected ExpressionPairCollection _namedArguments;
		
		protected AttributeImpl()
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		protected AttributeImpl(string name)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 			Name = name;
		}
		
		protected AttributeImpl(antlr.Token token, string name) : base(token)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 			Name = name;
		}
		
		internal AttributeImpl(antlr.Token token) : base(token)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		internal AttributeImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Attribute;
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
		public ExpressionCollection Arguments
		{
			get
			{
				return _arguments;
			}
			
			set
			{
				_arguments = value;
				if (null != _arguments)
				{
					_arguments.InitializeParent(this);
				}
			}
		}
		public ExpressionPairCollection NamedArguments
		{
			get
			{
				return _namedArguments;
			}
			
			set
			{
				_namedArguments = value;
				if (null != _namedArguments)
				{
					_namedArguments.InitializeParent(this);
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Attribute resultingTypedNode;
			transformer.OnAttribute((Attribute)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
