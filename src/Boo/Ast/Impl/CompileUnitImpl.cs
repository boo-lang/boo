using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class CompileUnitImpl : Node
	{
		protected ModuleCollection _modules;
		
		protected CompileUnitImpl()
		{
			_modules = new ModuleCollection(this);
 		}
		
		internal CompileUnitImpl(antlr.Token token) : base(token)
		{
			_modules = new ModuleCollection(this);
 		}
		
		internal CompileUnitImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_modules = new ModuleCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.CompileUnit;
			}
		}
		public ModuleCollection Modules
		{
			get
			{
				return _modules;
			}
			
			set
			{
				
				if (_modules != value)
				{
					_modules = value;
					if (null != _modules)
					{
						_modules.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			CompileUnit thisNode = (CompileUnit)this;
			CompileUnit resultingTypedNode = thisNode;
			transformer.OnCompileUnit(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
