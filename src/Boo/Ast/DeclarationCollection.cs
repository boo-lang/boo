using System;

namespace Boo.Ast
{
	public class DeclarationCollection : Boo.Ast.Impl.DeclarationCollectionImpl
	{
		public DeclarationCollection()
		{
		}
		
		public DeclarationCollection(Boo.Ast.Node parent) : base(parent)
		{
		}
		
		public Declaration this[string name]
		{
			get
			{
				foreach (Declaration d in InnerList)
				{
					if (name == d.Name)
					{
						return d;
					}
				}
				return null;
			}
		}
	}
}
