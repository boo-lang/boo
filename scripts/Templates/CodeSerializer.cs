${header}
namespace Boo.Lang.Compiler.Ast
{	
	public partial class CodeSerializer
	{
<%
for item in model.GetEnums():
%>		public bool ShouldSerialize(${item.Name} value)
		{
			return value != ${item.Name}.None;
		}
		
		public Expression Serialize(${item.Name} value)
		{
			return SerializeEnum("${item.Name}", (long)value);
		}

<%
end

for item in model.GetConcreteAstNodes():
	continue if item.Attributes.Contains("ignore")
	
	fields = model.GetAllFields(item)
	itemType = "Boo.Lang.Compiler.Ast.${item.Name}"
%>		override public void On${item.Name}(${itemType} node)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(
					node.LexicalInfo,
					CreateReference("${itemType}"));
<%
	for field in fields:
	
%>			if (ShouldSerialize(node.${field.Name}))
			{
<%
		if model.IsCollectionField(field):
			
%>				mie.NamedArguments.Add(
					new ExpressionPair(
						new ReferenceExpression("${field.Name}"),
						SerializeCollection("Boo.Lang.Compiler.Ast.${field.Type}", node.${field.Name})));
<%		else:

%>				mie.NamedArguments.Add(
					new ExpressionPair(
						new ReferenceExpression("${field.Name}"),
						Serialize(node.${field.Name})));
<%
		end
%>			}
<%
	end
%>			Push(mie);
		}

<%
end
%>	}
}

