${header}
namespace Boo.Lang.Compiler.Ast

import System

[Serializable]
public enum NodeType:
<%
nodes = array(model.GetConcreteAstNodes())
for item in nodes:	
%>	${item.Name}
<%
end
%>