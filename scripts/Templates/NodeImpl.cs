${header}
namespace Boo.Lang.Compiler.Ast.Impl
{	
	using Boo.Lang.Compiler.Ast;
	using System.Collections;
	using System.Runtime.Serialization;
	
	[System.Serializable]
	public abstract class ${node.Name}Impl : ${join(node.BaseTypes, ', ')}
	{
<%

	allFields = model.GetAllFields(node)

	for field as Field in node.Members:

%>		protected ${field.Type} ${GetPrivateName(field)};

<%
	end
%>		protected ${node.Name}Impl()
		{
			InitializeFields();
		}
		
		protected ${node.Name}Impl(LexicalInfo info) : base(info)
		{
			InitializeFields();
		}
		
<%
	simpleFields = model.GetSimpleFields(node)
	if len(simpleFields):
	
%>		protected ${node.Name}Impl(${FormatParameterList(simpleFields)})
		{
			InitializeFields();
<%
		for field as Field in simpleFields:
%>			${field.Name} = ${GetParameterName(field)};
<%
		end
%>		}
			
		protected ${node.Name}Impl(LexicalInfo lexicalInfo, ${FormatParameterList(simpleFields)}) : base(lexicalInfo)
		{
			InitializeFields();
<%
		for field as Field in simpleFields:
%>			${field.Name} = ${GetParameterName(field)};
<%
		end
%>		}

<%
	end
	
%>		new public ${node.Name} CloneNode()
		{
			return Clone() as ${node.Name};
		}

<%
	unless model.IsAbstract(node):
%>		override public NodeType NodeType
		{
			get
			{
				return NodeType.${node.Name};
			}
		}
		
<%
	end

%>		override public bool Matches(Node node)
		{	
			${node.Name} other = node as ${node.Name};
			if (null == other) return false;
<%
	for field in allFields:
		fieldName = GetPrivateName(field)
		fieldType = model.ResolveFieldType(field)
		if fieldType is null or model.IsEnum(fieldType):
		
%>			if (${fieldName} != other.${fieldName}) return false;
<%
		else:
%>			if (!Node.Matches(${fieldName}, other.${fieldName})) return false;
<%
		end
	end
%>			return true;
		}
	
		override public bool Replace(Node existing, Node newNode)
		{
			if (base.Replace(existing, newNode))
			{
				return true;
			}
<%			
	for field in allFields:				
		fieldType = model.ResolveFieldType(field)
		continue if fieldType is null
		continue if model.IsEnum(fieldType)
		
		fieldName = GetPrivateName(field)					
		if model.IsCollection(fieldType):
			collectionItemType = model.GetCollectionItemType(fieldType)
			
%>			if (${fieldName} != null)
			{
				${collectionItemType} item = existing as ${collectionItemType};
				if (null != item)
				{
					${collectionItemType} newItem = (${collectionItemType})newNode;
					if (${fieldName}.Replace(item, newItem))
					{
						return true;
					}
				}
			}
<%
		else:
			
%>			if (${fieldName} == existing)
			{
				this.${field.Name} = (${field.Type})newNode;
				return true;
			}
<%
		end
	end

%>			return false;
		}
		
		override public object Clone()
		{
			${node.Name} clone = (${node.Name})FormatterServices.GetUninitializedObject(typeof(${node.Name}));
			clone._lexicalInfo = _lexicalInfo;
			clone._endSourceLocation = _endSourceLocation;
			clone._documentation = _documentation;
			//clone._entity = _entity;
			clone._annotations = (Hashtable)_annotations.Clone();
		
<%			
	if model.IsExpression(node):
	
%>			clone._expressionType = _expressionType;
<%
	end	
	
	
	for field in allFields:
		fieldType = model.ResolveFieldType(field)
		fieldName = GetPrivateName(field)
		if fieldType is not null and not model.IsEnum(fieldType):
		
%>			if (null != ${fieldName})
			{
				clone.${fieldName} = ${fieldName}.Clone() as ${field.Type};
				clone.${fieldName}.InitializeParent(clone);
			}
<%
		else:
			
%>			clone.${fieldName} = ${fieldName};
<%
		end
	end

%>			return clone;
		}
		
		override internal void ClearTypeSystemBindings()
		{
			_annotations.Clear();
			//_entity = null;
<%
	if model.IsExpression(node):
%>			_expressionType = null;
<%
	end
		
	for field in allFields:
		fieldType = model.ResolveFieldType(field)
		fieldName = GetPrivateName(field)
		if fieldType is not null and not model.IsEnum(fieldType):
		
%>			if (null != ${fieldName})
			{
				${fieldName}.ClearTypeSystemBindings();
			}
<%
		end
	end
%>
		}
	
<%
	for field as Field in node.Members:
		if field.Name == "Name":
			Output.WriteLine("""
		[System.Xml.Serialization.XmlAttribute]""")
		elif field.Name == "Modifiers":
			Output.WriteLine("""
		[System.Xml.Serialization.XmlAttribute,
		System.ComponentModel.DefaultValue(${field.Type}.None)]""")
		else:
			Output.WriteLine("""
		[System.Xml.Serialization.XmlElement]""")
		end
		
%>		public ${field.Type} ${field.Name}
		{
			get
			{
				return ${GetPrivateName(field)};
			}

<%			
		fieldType = model.ResolveFieldType(field)
		if fieldType is not null and not model.IsEnum(fieldType):
		
%>			set
			{
				if (${GetPrivateName(field)} != value)
				{
					${GetPrivateName(field)} = value;
					if (null != ${GetPrivateName(field)})
					{
						${GetPrivateName(field)}.InitializeParent(this);
					}
				}
			}
<%
		else:

%>			set
			{
				${GetPrivateName(field)} = value;
			}
<%
		end
%>
		}
		
<%
	end

%>		private void InitializeFields()
		{
<%
	for field as Field in node.Members:
		if model.IsCollectionField(field):
%>
			${GetPrivateName(field)} = new ${field.Type}(this);
<%
		elif field.Attributes.Contains("auto"):
%>			${GetPrivateName(field)} = new ${field.Type}();
			${GetPrivateName(field)}.InitializeParent(this);
<%
		end
	end
%>
		}
	}
}

