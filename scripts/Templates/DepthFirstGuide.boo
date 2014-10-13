${header}
namespace Boo.Lang.Compiler.Ast

import System

public callable NodeEvent[of T(Node)](node as T)

public partial class DepthFirstGuide(IAstVisitor):
<%

for item in model.GetConcreteAstNodes():

%>	public event On${item.Name} as NodeEvent[of ${item.Name}]
<%
	fields = model.GetVisitableFields(item)	
	if len(fields):
	
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	def IAstVisitor.On${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
	
<%
		for field in fields:
			localName = 'l' + field.Name
			if model.IsCollectionField(field):
%>		${localName} = node.${field.Name}
		if ${localName} is not null:
			innerList = ${localName}.InnerList
			for i in range(0, innerList.Count):
				innerList.FastAt(i).Accept(self)
<%
			else:
%>		${localName} = node.${field.Name}
		if ${localName} is not null:
			${localName}.Accept(self)
<%
			end
		end
%>		handler = On${item.Name}
		if handler is not null:
			handler(node);
<%
	else:
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	def IAstVisitor.On${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
		handler = On${item.Name}
		return if handler is null
		handler(node)
<%
	end
end
%>