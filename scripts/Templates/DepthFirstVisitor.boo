${header}
namespace Boo.Lang.Compiler.Ast

import System

public partial class DepthFirstVisitor(IAstVisitor):
<%

for item in model.GetConcreteAstNodes():
	
	fields = model.GetVisitableFields(item)	
	if len(fields):
	
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def On${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
		if Enter${item.Name}(node):
<%
		for field in fields:
%>			Visit(node.${field.Name})
<%
		end
%>			Leave${item.Name}(node)

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def Enter${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as bool:
		return true

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def Leave${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
		pass
<%
	else:
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def On${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
		pass
<%
	end
end
%>