using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
	public class TranparentIdentifierInjector: QueryTransformerStep
	{
		private Declaration _l;
		private Declaration _r;
		private string _name;
		
		public TranparentIdentifierInjector(Declaration l, Declaration r, string name)
		{
			_l = l;
			_r = r;
			_name = name;
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			base.OnReferenceExpression(node);
			if ((node.Name.Equals(_l.Name)) || (node.Name.Equals(_r.Name)))
				node.Name = _name + '.' + node.Name;
			else if (node.Name.StartsWith(_l.Name + '.'))
				node.Name = node.Name.Replace(_l.Name + '.', _name + '.');
		}
	}
	
	public class TransparentIdentifierFixer: QueryTransformerStep
	{
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			base.OnReferenceExpression(node);
			var idx = node.Name.LastIndexOf('.');
			if (idx == -1)
				return;
			var newref = new MemberReferenceExpression(
				new ReferenceExpression(node.Name.Substring(0, idx)),
				node.Name.Substring(idx + 1));
			Visit(newref);
			ReplaceCurrentNode(newref);
		}
	}
}