using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	/// <summary>
	/// Permite diferenar entre uma expresso que no foi
	/// passada de uma que foi omitidade como nosso caso
	/// de slices na forma [expression:] ou [:expression]
	/// </summary>
	public sealed class OmittedExpression : Expression
	{
		public static readonly Expression Default = new OmittedExpression();

		public OmittedExpression()
		{
		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.OmittedExpression;
			}
		}

		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnOmittedExpression(this);
		}
		
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Expression result = this;
			transformer.OnOmittedExpression(this, ref result);
			resultingNode = result;
		}
	}	
}
