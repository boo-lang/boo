${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public partial class DepthFirstVisitor : IAstVisitor
	{
<%

for item in model.GetConcreteAstNodes():
	
	fields = model.GetVisitableFields(item)	
	if len(fields):
	
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{				
			if (Enter${item.Name}(node))
			{
<%
		for field in fields:
%>				Visit(node.${field.Name});
<%
		end
%>				Leave${item.Name}(node);
			}
		}

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
	else:
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
<%
	end
end
%>				
	}
}

