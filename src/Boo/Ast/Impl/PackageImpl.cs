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
	}
}
