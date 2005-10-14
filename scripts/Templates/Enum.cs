${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;

	[Serializable]
<% if node.Name.EndsWith("Modifiers"):
%>	[Flags]
<% end
%>	public enum ${node.Name}
	{
<%
last = node.Members[-1]

separator = ","
for field as EnumMember in node.Members:
	if field.Initializer:
		initializer = " = ${field.Initializer.Value}"
	else:
		initializer = ""
	end
	separator = "" if field is last
%>		${field.Name}${initializer}${separator}
<%
end
%>	}
}
