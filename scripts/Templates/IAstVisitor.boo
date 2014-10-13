${header}
namespace Boo.Lang.Compiler.Ast

import System

public interface IAstVisitor:	
<%
	for member as TypeDefinition in model.GetConcreteAstNodes():
%>	def On${member.Name}(node as ${member.Name}) as void:
		pass
<%
	end
%>