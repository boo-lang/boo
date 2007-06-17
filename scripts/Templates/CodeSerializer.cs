${header}
namespace Boo.Lang.Compiler.Ast
{	
	public partial class CodeSerializer
	{
<%

for item in model.GetConcreteAstNodes():
	
	fields = model.GetAllFields(item)
	itemType = "Boo.Lang.Compiler.Ast.${item.Name}"
%>
		override public void On${item.Name}(${itemType} node)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(
					node.LexicalInfo,
					CreateReference("${itemType}"));
<%
	for field in fields:
		if model.IsCollectionField(field):
			
%>			mie.NamedArguments.Add(
				new ExpressionPair(
					new ReferenceExpression("${field.Name}"),
					SerializeCollection("Boo.Lang.Compiler.Ast.${field.Type}", node.${field.Name})));
<%		else:

%>			mie.NamedArguments.Add(
				new ExpressionPair(
					new ReferenceExpression("${field.Name}"),
					Serialize(node.${field.Name})));
<%
		end
	end
%>			Push(mie);
		}
<%
end
%>	}
}

