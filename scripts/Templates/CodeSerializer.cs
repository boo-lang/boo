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
			return (long)value != 0;
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
	continue if item.Name in ("ExpressionStatement", "QuasiquoteExpression")
	
	fields = model.GetAllFields(item)
	itemType = "Boo.Lang.Compiler.Ast.${item.Name}"
	
	if item.Name.StartsWith("Splice"):
		methodDeclaration = "internal void Serialize" + item.Name
	else:
		methodDeclaration = "override public void On" + item.Name
	end

%>		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		${methodDeclaration}(${itemType} node)
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

