${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	/// <summary>
	/// Visitor implementation that avoids the overhead of cloning collections
	/// before visiting them.
	///
	/// Avoid mutating collections when using this implementation.
	/// </summary>
	public partial class FastDepthFirstVisitor : IAstVisitor
	{
<%

for item in model.GetConcreteAstNodes():
	
	fields = model.GetVisitableFields(item)
	
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{				
<%
		for field in fields:
			localName = GetParameterName(field)
			if model.IsCollectionField(field):
%>			{
				var ${localName} = node.${field.Name};
				if (${localName} != null)
				{
					var count = ${localName}.Count;
					for (var i=0; i<count; ++i)
						${localName}[i].Accept(this);
				}
			}
<%
			else:
%>			{
				var ${localName} = node.${field.Name};
				if (${localName} != null)
					${localName}.Accept(this);
			}
<%
			end
		end
%>		}
<%
end
%>
		protected void Visit(Node node)
		{
			if (node == null)
				return;
			node.Accept(this);
		}
		
		protected void Visit<T>(NodeCollection<T> nodes) where T: Node
		{
			if (nodes == null)
				return;
			for (var i = 0; i<nodes.Count; ++i)
				nodes[i].Accept(this);
		}
	}
}

