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
			i as int = 0
			for field as Field in visitableFields:
				++i
				if model.IsCollectionField(field):

%>			Visit(node.${field.Name})
<%
				else:

%>			current${field.Name}Value as ${field.Type} = node.${field.Name};
			if current${field.Name}Value is not null:
				newValue$i = VisitNode(current${field.Name}Value) cast ${field.Type}
				unless object.ReferenceEquals(newValue$i, current${field.Name}Value):
					node.${field.Name} = newValue$i
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
	public virtual def Leave${item.Name}(node as Boo.Lang.Compiler.Ast.${item.Name}) as void:
		pass
<%
		end
	end
%>