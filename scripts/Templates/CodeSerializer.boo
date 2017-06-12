${header}
namespace Boo.Lang.Compiler.Ast

public partial class CodeSerializer:
	
<%
for item in model.GetEnums():
%>	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def ShouldSerialize(value as ${item.Name}) as bool:
		return (value cast long) != 0;
	
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def Serialize(value as ${item.Name}) as Expression:
		return SerializeEnum("${item.Name}", (value cast long))

<%
end

for item in model.GetConcreteAstNodes():
	continue if item.Attributes.Contains("ignore")
	continue if item.Name in ("ExpressionStatement", "QuasiquoteExpression")
	
	fields = model.GetAllFields(item)
	itemType = "Boo.Lang.Compiler.Ast.${item.Name}"
	
	if item.Name.StartsWith("Splice"):
		methodDeclaration = "internal def Serialize" + item.Name
	else:
		methodDeclaration = "override public def On" + item.Name
	end

%>	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	${methodDeclaration}(node as ${itemType}) as void:
		mie = MethodInvocationExpression(
				node.LexicalInfo,
				CreateReference(node, "${itemType}"))
		mie.Arguments.Add(Serialize(node.LexicalInfo))
<%
	for field in fields:
	
%>		if ShouldSerialize(node.${field.Name}):
<%
		if model.IsCollectionField(field):
			
%>			mie.NamedArguments.Add(
				ExpressionPair(
					CreateReference(node, "${field.Name}"),
					SerializeCollection(node, "Boo.Lang.Compiler.Ast.${field.Type}", node.${field.Name})))
<%		else:

%>			mie.NamedArguments.Add(
				ExpressionPair(
					CreateReference(node, "${field.Name}"),
					Serialize(node.${field.Name})));
<%
		end
%>
<%
	end
%>		Push(mie);

<%
end
%>