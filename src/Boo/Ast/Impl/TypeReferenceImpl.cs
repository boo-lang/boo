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
