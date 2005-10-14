${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]
	public enum NodeType
	{
<%
nodes = array(model.GetConcreteAstNodes())
last = nodes[-1]
separator = ","
for item in nodes:	
	separator = "" if item is last
%>		${item.Name}${separator}
<%
end
%>	}
}

