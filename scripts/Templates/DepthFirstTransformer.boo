${header}
namespace Boo.Lang.Compiler.Ast

import System

public partial class DepthFirstTransformer(IAstVisitor):
<%
	for item as TypeMember in model.GetConcreteAstNodes():
			
		visitableFields = model.GetVisitableFields(item)
		resultingNodeType = model.GetResultingTransformerNode(item)
			
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def On${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
<%
		if len(visitableFields):
			
%>		if Enter${item.Name}(node):
<%
			for field as Field in visitableFields:
				if model.IsCollectionField(field):

%>			Visit(node.${field.Name})
<%
				else:

%>			${field.Type} current${field.Name}Value = node.${field.Name};
			if current${field.Name}Value is not null:
				newValue = VisitNode(current${field.Name}Value) cast ${field.Type}
				unless object.ReferenceEquals(newValue, current${field.Name}Value):
					node.${field.Name} = newValue
<%
				end
			end
%>
			Leave${item.Name}(node);
<%
		else:
%>
		pass
<%		end
		
		if len(visitableFields):
		
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual def Enter${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as bool:
		return true

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public virtual Leave${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
		pass
<%
		end
	end
%>