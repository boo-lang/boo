using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class TypeReferenceImpl : Node
	{
		protected string _name;
		
		protected TypeReferenceImpl()
		{
 		}
		
		protected TypeReferenceImpl(string name)
		{
 			Name = name;
		}
		
		protected TypeReferenceImpl(antlr.Token token, string name) : base(token)
		{
 			Name = name;
		}
		
		internal TypeReferenceImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal TypeReferenceImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.TypeReference;
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
			TypeReference resultingTypedNode;
			transformer.OnTypeReference((TypeReference)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
