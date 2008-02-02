${header}
namespace Boo.Lang.Compiler.Ast
{	
	public partial class CodeSerializer
	{
<%
for item in model.GetEnums():
%>		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public bool ShouldSerialize(${item.Name} value)
		{
			return value != ${item.Name}.None;
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public Expression Serialize(${item.Name} value)
		{
			return SerializeEnum("${item.Name}", (long)value);
		}

<%
end

for item in model.GetConcreteAstNodes():
	continue if item.Attributes.Contains("ignore")
	continue if item.Name.StartsWith("Splice")
	continue if item.Name == "ExpressionStatement"
	
	fields = model.GetAllFields(item)
	itemType = "Boo.Lang.Compiler.Ast.${item.Name}"
%>		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		override public void On${item.Name}(${itemType} node)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(
					node.LexicalInfo,
					CreateReference(node, "${itemType}"));
<%
	for field in fields:
	
%>			if (ShouldSerialize(node.${field.Name}))
			{
<%
		if model.IsCollectionField(field):
			
%>				mie.NamedArguments.Add(
					new ExpressionPair(
						CreateReference(node, "${field.Name}"),
						SerializeCollection(node, "Boo.Lang.Compiler.Ast.${field.Type}", node.${field.Name})));
<%		else:

%>				mie.NamedArguments.Add(
					new ExpressionPair(
						CreateReference(node, "${field.Name}"),
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

