using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class PackageImpl : Node
	{
		protected string _name;
		
		protected PackageImpl()
		{
 		}
		
		protected PackageImpl(string name)
		{
 			Name = name;
		}
		
		protected PackageImpl(antlr.Token token, string name) : base(token)
		{
 			Name = name;
		}
		
		internal PackageImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal PackageImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Package;
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
			Package thisNode = (Package)this;
			Package resultingTypedNode = thisNode;
			transformer.OnPackage(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
