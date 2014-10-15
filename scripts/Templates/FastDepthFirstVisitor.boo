${header}
namespace Boo.Lang.Compiler.Ast

import System

/// <summary>
/// Visitor implementation that avoids the overhead of cloning collections
/// before visiting them.
///
/// Avoid mutating collections when using this implementation.
/// </summary>
public partial class FastDepthFirstVisitor(IAstVisitor):
<%

for item in model.GetConcreteAstNodes():
	
	fields = model.GetVisitableFields(item)
	
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def On${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
<%
		empty = true
		i as int = 0
		for field in fields:
			++i
			empty = false
			localName = 'l' + field.Name
			if model.IsCollectionField(field):
%>		${localName} = node.${field.Name}
		if ${localName} is not null:
			innerList$i = ${localName}.InnerList
			for i in range(0, innerList$i.Count):
				innerList$i.FastAt(i).Accept(self)
<%
			else:
%>		${localName} = node.${field.Name}
		if ${localName} is not null:
			${localName}.Accept(self)
<%
			end
		end
		if empty:
%>
		pass
<%		end
end
%>
	protected virtual def Visit(node as Node) as void:
		node.Accept(self) unless node is null
	
	protected virtual def Visit[of T(Node)](nodes as NodeCollection[of T]) as void:
		return if nodes is null
		innerList = nodes.InnerList
		for i in range(0, innerList.Count):
			innerList.FastAt(i).Accept(this)
