${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public delegate void NodeEvent<T>(T node) where T:Node;
	
	public partial class DepthFirstGuide : IAstVisitor
	{
<%

for item in model.GetConcreteAstNodes():

%>		public event NodeEvent<${item.Name}> On${item.Name};
<%
	fields = model.GetVisitableFields(item)	
	if len(fields):
	
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		void IAstVisitor.On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{	
<%
		for field in fields:
			localName = GetParameterName(field)
			if model.IsCollectionField(field):
%>			{
				var ${localName} = node.${field.Name};
				if (${localName} != null)
				{
					var innerList = ${localName}.InnerList;
					var count = innerList.Count;
					for (var i=0; i<count; ++i)
						innerList.FastAt(i).Accept(this);
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
%>			var handler = On${item.Name};
			if (handler != null)
				handler(node);
		}
<%
	else:
%>
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		void IAstVisitor.On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
			var handler = On${item.Name}; 
			if (handler == null)
				return;
			handler(node);
		}
<%
	end
end
%>				
	}
}

