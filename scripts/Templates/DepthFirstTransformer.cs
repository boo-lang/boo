${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;

	public partial class DepthFirstTransformer : IAstVisitor
	{
<%
	for item as TypeMember in model.GetConcreteAstNodes():
			
		visitableFields = model.GetVisitableFields(item)
		resultingNodeType = model.GetResultingTransformerNode(item)
			
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{	
<%
		if len(visitableFields):
			
%>			if (Enter${item.Name}(node))
			{
<%
			for field as Field in visitableFields:
				if model.IsCollectionField(field):

%>				Visit(node.${field.Name});
<%
				else:

%>				${field.Type} current${field.Name}Value = node.${field.Name};
				if (null != current${field.Name}Value)
				{			
					${field.Type} newValue = (${field.Type})VisitNode(current${field.Name}Value);
					if (!object.ReferenceEquals(newValue, current${field.Name}Value))
					{
						node.${field.Name} = newValue;
					}
				}
<%
				end
			end
%>
				Leave${item.Name}(node);
			}
<%
		end
%>		}
<%
		
		if len(visitableFields):
		
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public virtual bool Enter${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
			return true;
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public virtual void Leave${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
<%
		end
	end
%>
	}
}
