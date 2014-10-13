${header}
namespace Boo.Lang.Compiler.Ast

import System;

[Serializable]
<% if node.Attributes.Contains("Flags"):
%>[Flags]
<% end
%>public enum ${node.Name}:
<%
for field as EnumMember in node.Members:
	if field.Initializer:
		initializer = " = ${field.Initializer.ToCodeString()}"
	else:
		initializer = ""
	end
%>	${field.Name}${initializer}
<%
end
%>