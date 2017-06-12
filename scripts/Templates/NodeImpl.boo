${header}<%

if model.IsAbstract(node):
	abstractClass = "abstract "
end

%>namespace Boo.Lang.Compiler.Ast

import System.Collections
import System.Runtime.Serialization

[System.Serializable]
public ${abstractClass}partial class ${node.Name} (${join(node.BaseTypes, ', ')}):
<%

allFields = model.GetAllFields(node)

for field as Field in node.Members:

%>	protected ${GetPrivateName(field)} as ${field.Type}

<%
end
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def CloneNode() as ${node.Name}:
		return Clone() cast ${node.Name}
	
	/// <summary>
	/// <see cref="Node.CleanClone"/>
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def CleanClone() as ${node.Name}:
		return super.CleanClone() as ${node.Name}
<%
unless model.IsAbstract(node):
%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	override public NodeType as NodeType:
		get: return NodeType.${node.Name}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	override public def Accept(visitor as IAstVisitor) as void:
		visitor.On${node.Name}(self)
<%
end

%>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	override public def Matches(node as Node) as bool:
		return false if (node is null) or (NodeType != node.NodeType)
		other = node as ${node.Name}
<%
for field in allFields:
	fieldName = GetPrivateName(field)
	fieldType = model.ResolveFieldType(field)
	if fieldType is null or model.IsEnum(fieldType):
	
%>		return NoMatch("${node.Name}.${fieldName}") unless ${fieldName} == other.${fieldName}
<%
	elif model.IsCollectionField(field):
%>		return NoMatch("${node.Name}.${fieldName}") unless Node.AllMatch(${fieldName}, other.${fieldName})
<%		else:
%>		return NoMatch("${node.Name}.${fieldName}") unless Node.Matches(${fieldName}, other.${fieldName})
<%
	end
end
%>		return true

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	override public def Replace(existing as Node, newNode as Node) as bool:
		return true if super.Replace(existing, newNode)
<%			
i as int = 0
for field in allFields:
	++i
	fieldType = model.ResolveFieldType(field)
	continue if fieldType is null
	continue if model.IsEnum(fieldType)
	
	fieldName = GetPrivateName(field)
	if model.IsCollection(fieldType):
		collectionItemType = model.GetCollectionItemType(fieldType)
		
%>		if ${fieldName} is not null:
			item$i = existing as ${collectionItemType}
			if item$i is not null:
				newItem$i = newNode as ${collectionItemType}
				return true if ${fieldName}.Replace(item$i, newItem$i)
<%
	else:
		
%>		if ${fieldName} == existing:
			self.${field.Name} = newNode as ${field.Type}
			return true;
<%
	end
end

%>		return false;

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	override public def Clone() as object:
<%
		if model.IsAbstract(node):
%>
		raise System.InvalidOperationException("Cannot clone abstract class: ${node.Name}")
<% else: %>		
		clone = ${node.Name}()
		clone._lexicalInfo = _lexicalInfo
		clone._endSourceLocation = _endSourceLocation
		clone._documentation = _documentation
		clone._isSynthetic = _isSynthetic
		clone._entity = _entity
		if _annotations is not null: clone._annotations = _annotations.Clone() as Hashtable
<%			
if model.IsExpression(node):

%>		clone._expressionType = _expressionType
<%
end	


for field in allFields:
	fieldName = GetPrivateName(field)
	if model.IsNodeField(field):
	
%>		if ${fieldName} is not null:
			clone.${fieldName} = ${fieldName}.Clone() as ${field.Type}
			clone.${fieldName}.InitializeParent(clone)
<%
	else:
		
%>		clone.${fieldName} = ${fieldName}
<%
	end
end

%>		return clone;

<%
end 
%>
	

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	override internal def ClearTypeSystemBindings() as void:
		_annotations = null;
		_entity = null;
<%
if model.IsExpression(node):
%>		_expressionType = null;
<%
end
	
for field in allFields:
	fieldType = model.ResolveFieldType(field)
	fieldName = GetPrivateName(field)
	if fieldType is not null and not model.IsEnum(fieldType):
	
%>		if ${fieldName} is not null:
			${fieldName}.ClearTypeSystemBindings()
<%
	end
end
%>

<%
for field as Field in node.Members:
	if field.Name == "Name":
		Output.WriteLine("""
	[System.Xml.Serialization.XmlAttribute]""")
	elif field.Name == "Modifiers":
		Output.WriteLine("""
	[System.Xml.Serialization.XmlAttribute,
	System.ComponentModel.DefaultValue(${field.Type}.None)]""")
	elif model.IsCollectionField(field):
		Output.WriteLine("""
	[System.Xml.Serialization.XmlArray]
	[System.Xml.Serialization.XmlArrayItem(typeof(${model.GetCollectionItemType(model.ResolveFieldType(field))}))]""")
	else:
		Output.WriteLine("""
	[System.Xml.Serialization.XmlElement]""")
	end
	
%>	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public ${field.Name} as ${field.Type}:
		
<%
if model.IsCollectionField(field):
%>
		get:
			${GetPrivateName(field)} = ${field.Type}(self) if ${GetPrivateName(field)} is null
			return ${GetPrivateName(field)} 
<%
elif field.Attributes.Contains("auto"):
%>		get:
			if ${GetPrivateName(field)} is null:
				${GetPrivateName(field)} = ${field.Type}()
				${GetPrivateName(field)}.InitializeParent(self)
			return ${GetPrivateName(field)}
<%
else:
%>		get: return ${GetPrivateName(field)}
<%
end

fieldType = model.ResolveFieldType(field)
if fieldType is not null and not model.IsEnum(fieldType):
	
%>		set:
			if ${GetPrivateName(field)} != value:
				${GetPrivateName(field)} = value;
				if ${GetPrivateName(field)} is not null:
					${GetPrivateName(field)}.InitializeParent(self);
<%
			if field.Attributes.Contains("LexicalInfo"):
%>					self.LexicalInfo = value.LexicalInfo;
<%
			end
%>
<%
	else:

%>		set: ${GetPrivateName(field)} = value
<%
	end
%>
<%
end
%>