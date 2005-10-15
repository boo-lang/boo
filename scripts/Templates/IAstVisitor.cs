${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public interface IAstVisitor
	{
<%
	for member in model.GetConcreteAstNodes():
%>		void On${member.Name}(${member.Name} node);
<%
	end
%>
	}
}

