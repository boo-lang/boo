using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class LocalImpl : Node
	{
		protected string _name;
		
		protected LocalImpl()
		{
 		}
		
		protected LocalImpl(string name)
		{
 			Name = name;
		}
		
		protected LocalImpl(antlr.Token token, string name) : base(token)
		{
 			Name = name;
		}
		
		internal LocalImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal LocalImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Local;
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
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Local resultingTypedNode;
			transformer.OnLocal((Local)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
