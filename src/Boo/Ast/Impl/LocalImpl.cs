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
