using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class DeclarationImpl : Node
	{
		protected string _name;
		protected TypeReference _type;
		
		protected DeclarationImpl()
		{
 		}
		
		protected DeclarationImpl(string name, TypeReference type)
		{
 			Name = name;
			Type = type;
		}
		
		protected DeclarationImpl(antlr.Token token, string name, TypeReference type) : base(token)
		{
 			Name = name;
			Type = type;
		}
		
		internal DeclarationImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal DeclarationImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
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
	}
}
