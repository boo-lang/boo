#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import System
import System.IO
import Boo.AntlrParser from Boo.AntlrParser
import Boo.Lang.Compiler.Ast

def WriteNodeTypeEnum(module as Module):
	using writer=OpenFile(GetPath("NodeType.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.WriteLine("""
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]
	public enum NodeType
	{""")
	
		nodes = GetConcreteAstNodes(module)
		last = nodes[-1]
		for item as TypeDefinition in nodes:
			writer.Write("\t\t${item.Name}")
			if item is not last:
				writer.WriteLine(", ")
	
		writer.Write("""
	}
}""")

def WriteEnum(node as EnumDefinition):
	using writer=OpenFile(GetPathFromNode(node)):
		WriteLicense(writer)
		writer.Write("""
namespace Boo.Lang.Compiler.Ast
{
	using System;

	[Serializable]
	public enum ${node.Name}
	{	
""")
		last = node.Members[-1]
		for field as EnumMember in node.Members:
			writer.Write("\t\t${field.Name}")
			if field.Initializer:
				writer.Write(" = ${field.Initializer.Value}")
			if field is not last:
				writer.Write(",")
			writer.WriteLine()

		writer.Write("""
	}
}
""")

def FormatParameterList(fields as List):
	buffer = System.Text.StringBuilder()
	last = fields[-1]
	
	for field as Field in fields:
		buffer.Append(field.Type)
		buffer.Append(" ")
		buffer.Append(GetParameterName(field))
		if field is not last:
			buffer.Append(", ")
			
	return buffer.ToString()
	
def GetParameterName(field as Field):
	name = field.Name
	name = name[0:1].ToLower() + name[1:]
	if name in "namespace", "operator":
		name += "_"
	return name
		
def WriteAssignmentsFromParameters(writer as TextWriter, fields as List):
	for field as Field in fields:
		writer.Write("""
			${field.Name} = ${GetParameterName(field)};""")
	
def WriteClassImpl(node as ClassDefinition):
	
	allFields = GetAllFields(node)
	
	using writer=OpenFile(GetPath("Impl/${node.Name}Impl.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.WriteLine("""
namespace Boo.Lang.Compiler.Ast.Impl
{	
	using Boo.Lang.Compiler.Ast;
	using System.Collections;
	using System.Runtime.Serialization;
	
	[System.Serializable]
	public abstract class ${node.Name}Impl : ${join(node.BaseTypes, ', ')}
	{
""")
	
		for field as Field in node.Members:
			writer.WriteLine("\t\tprotected ${field.Type} ${GetPrivateName(field)};");
	
		writer.WriteLine("""
		protected ${node.Name}Impl()
		{
			InitializeFields();
		}
		
		protected ${node.Name}Impl(LexicalInfo info) : base(info)
		{
			InitializeFields();
		}
		""")
		
		simpleFields = GetSimpleFields(node)
		if len(simpleFields):
			writer.Write("""
		protected ${node.Name}Impl(${FormatParameterList(simpleFields)})
		{
			InitializeFields();""")
				
			WriteAssignmentsFromParameters(writer, simpleFields)
			writer.Write("""
		}
			
		protected ${node.Name}Impl(LexicalInfo lexicalInfo, ${FormatParameterList(simpleFields)}) : base(lexicalInfo)
		{
			InitializeFields();""")
			WriteAssignmentsFromParameters(writer, simpleFields)	
			writer.Write("""
		}
			""")
			
		writer.WriteLine("""
		new public ${node.Name} CloneNode()
		{
			return Clone() as ${node.Name};
		}""")
		
		unless IsAbstract(node):
			writer.WriteLine("""
		override public NodeType NodeType
		{
			get
			{
				return NodeType.${node.Name};
			}
		}
		
		override public void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			${node.Name} thisNode = (${node.Name})this;
			${GetResultingTransformerNode(node)} resultingTypedNode = thisNode;
			transformer.On${node.Name}(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}""")
		
			writer.WriteLine("""
		override public bool Replace(Node existing, Node newNode)
		{
			if (base.Replace(existing, newNode))
			{
				return true;
			}""")
			
			for field as Field in allFields:				
				fieldType=ResolveFieldType(field)
				if not fieldType:
					continue
					
				fieldName=GetPrivateName(field)					
				if IsCollection(fieldType):
					collectionItemType = GetCollectionItemType(fieldType)
					writer.WriteLine("""
			if (${fieldName} != null)
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
			}""")
				else:
					unless IsEnum(fieldType):
						writer.WriteLine("""
			if (${fieldName} == existing)
			{
				this.${field.Name} = (${field.Type})newNode;
				return true;
			}""")
			
			writer.WriteLine("""
			return false;
		}""")
		
			writer.WriteLine("""
		override public object Clone()
		{
			${node.Name} clone = FormatterServices.GetUninitializedObject(typeof(${node.Name})) as ${node.Name};
			clone._lexicalInfo = _lexicalInfo;
			clone._documentation = _documentation;
			clone._properties = _properties.Clone() as Hashtable;
			""")
			
			for field as Field in allFields:
				fieldType = ResolveFieldType(field)
				fieldName = GetPrivateName(field)
				if fieldType and not IsEnum(fieldType):
					writer.WriteLine("""
			if (null != ${fieldName})
			{
				clone.${fieldName} = ${fieldName}.Clone() as ${field.Type};
				clone.${fieldName}.InitializeParent(clone);
			}""")
				else:
					writer.WriteLine("""
			clone.${fieldName} = ${fieldName};""")
			
			writer.Write("""			
			return clone;
		}
			""")
		
		for field as Field in node.Members:
			writer.WriteLine("""
		public ${field.Type} ${field.Name}
		{
			get
			{
				return ${GetPrivateName(field)};
			}
			""")
			
			fieldType = ResolveFieldType(field)
			if fieldType and not IsEnum(fieldType):
				writer.WriteLine("""
			set
			{
				if (${GetPrivateName(field)} != value)
				{
					${GetPrivateName(field)} = value;
					if (null != ${GetPrivateName(field)})
					{
						${GetPrivateName(field)}.InitializeParent(this);""")
						
				if field.Attributes.Contains("LexicalInfo"):
					writer.WriteLine("""
						LexicalInfo = value.LexicalInfo;""")
						
				writer.WriteLine("""
					}
				}
			}
			""")
			else:
				writer.WriteLine("""
			set
			{
				${GetPrivateName(field)} = value;
			}""")
			
			writer.WriteLine("""
		}
		""")
		
		writer.WriteLine("""
		private void InitializeFields()
		{""")
		
		for field as Field in node.Members:
			if IsCollectionField(field):
				writer.WriteLine("\t\t\t${GetPrivateName(field)} = new ${field.Type}(this);")
			else:
				if field.Attributes.Contains("auto"):
					writer.WriteLine("""
			${GetPrivateName(field)} = new ${field.Type}();
			${GetPrivateName(field)}.InitializeParent(this);
			""")
		
		writer.WriteLine("""
		}
	}
}""")
		

def WriteClass(node as ClassDefinition):
	path = GetPathFromNode(node);
	return if File.Exists(path)
	
	using writer=OpenFile(path):
		WriteLicense(writer)
		writer.Write("""
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]
	public class ${node.Name} : Boo.Lang.Compiler.Ast.Impl.${node.Name}Impl
	{
		public ${node.Name}()
		{
		}
		
		public ${node.Name}(LexicalInfo lexicalInfo) : base(lexicalInfo)
		{
		}
	}
}
""")
	
def WriteCollection(node as ClassDefinition):
	path = GetPathFromNode(node)
	return if File.Exists(path)
	
	using writer=OpenFile(path):
		WriteLicense(writer)
		writer.Write("""
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	[Serializable]
	public class ${node.Name} : Boo.Lang.Compiler.Ast.Impl.${node.Name}Impl
	{
		public ${node.Name}()
		{
		}
		
		public ${node.Name}(Boo.Lang.Compiler.Ast.Node parent) : base(parent)
		{
		}
	}
}
""")

def GetCollectionItemType(node as ClassDefinition):
	attribute = node.Attributes.Get("collection")[0]
	reference as ReferenceExpression = attribute.Arguments[0]
	return reference.Name
	
def ResolveFieldType(field as Field):
	return field.DeclaringType.DeclaringType.Members[(field.Type as SimpleTypeReference).Name]
	
def GetResultingTransformerNode(node as ClassDefinition):
	for subclass in "Statement", "Expression", "TypeReference":
		if IsSubclassOf(node, subclass):
			return subclass
	return node.Name
	
def IsSubclassOf(node as ClassDefinition, typename as string):
	for typeref as SimpleTypeReference in node.BaseTypes:
		if typename == typeref.Name:
			return true
		baseType = node.DeclaringType.Members[typeref.Name]
		if baseType and IsSubclassOf(baseType, typename):
			return true
	return false

def WriteCollectionImpl(node as ClassDefinition):
	path = GetPath("Impl/${node.Name}Impl.cs")
	using writer=OpenFile(path):
		WriteLicense(writer)
		WriteWarning(writer)
		
		itemType = "Boo.Lang.Compiler.Ast." + GetCollectionItemType(node)
		writer.Write("""
namespace Boo.Lang.Compiler.Ast.Impl
{
	using System;
	using Boo.Lang.Compiler.Ast;
	
	[Serializable]
	public class ${node.Name}Impl : NodeCollection
	{
		protected ${node.Name}Impl()
		{
		}
		
		protected ${node.Name}Impl(Node parent) : base(parent)
		{
		}
		
		public ${itemType} this[int index]
		{
			get
			{
				return (${itemType})InnerList[index];
			}
		}

		public void Add(${itemType} item)
		{
			base.Add(item);			
		}
		
		public void Extend(params ${itemType}[] items)
		{
			base.Add(items);			
		}
		
		public void Extend(System.Collections.ICollection items)
		{
			foreach (${itemType} item in items)
			{
				base.Add(item);
			}
		}
		
		public void ExtendWithClones(System.Collections.ICollection items)
		{
			foreach (${itemType} item in items)
			{
				base.Add(item.CloneNode());
			}
		}
		
		public void Insert(int index, ${itemType} item)
		{
			base.Insert(index, item);
		}
		
		public bool Replace(${itemType} existing, ${itemType} newItem)
		{
			return base.Replace(existing, newItem);
		}
		
		public new ${itemType}[] ToArray()
		{
			return (${itemType}[])InnerList.ToArray(typeof(${itemType}));
		}
	}
}
""")

def OpenFile(fname as string):	
	print(fname)
	return StreamWriter(fname, false, System.Text.Encoding.UTF8)
	
def GetPath(fname as string):
	return Path.Combine("src/Boo.Lang.Compiler/Ast", fname)
	
def GetPathFromNode(node as TypeMember):
	return GetPath("${node.Name}.cs")
	
def IsCollection(node as TypeMember):
	return node.Attributes.Contains("collection")
	
def IsCollectionField(field as Field):
	return (field.Type as SimpleTypeReference).Name.EndsWith("Collection")
	
def IsEnum(node as TypeMember):
	return NodeType.EnumDefinition == node.NodeType
	
def IsAbstract(member as TypeMember):
	return member.IsModifierSet(TypeMemberModifiers.Abstract)
	
def GetConcreteAstNodes(module as Module):
	nodes = []
	for member in module.Members:
		nodes.Add(member) if IsConcreteAstNode(member)
	return nodes	
	
def IsConcreteAstNode(member as TypeMember):
	return not (IsCollection(member) or IsEnum(member) or IsAbstract(member))
	
def WriteWarning(writer as TextWriter):
	writer.Write(
"""
//
// DO NOT EDIT THIS FILE!
//
// This file was generated automatically by
// astgenerator.boo on ${date.Now}
//
""")
	
def WriteLicense(writer as TextWriter):
	writer.Write(
"""#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion
""")

def WriteTransformer(module as Module):
	using writer=OpenFile(GetPath("IAstTransformer.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write("""
namespace Boo.Lang.Compiler.Ast
{
	public interface IAstTransformer
	{""")
		for member as TypeMember in module.Members:
			if IsConcreteAstNode(member):
				writer.WriteLine("""			
		void On${member.Name}(Boo.Lang.Compiler.Ast.${member.Name} node, ref Boo.Lang.Compiler.Ast.${GetResultingTransformerNode(member)} newNode);""")
			
		writer.Write("""
	}
}""")

def WriteDepthFirstTransformer(module as Module):
	
	using writer=OpenFile(GetPath("DepthFirstTransformer.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write("""
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public class DepthFirstTransformer : IAstTransformer
	{""")
	
		for item as TypeMember in module.Members:
			continue unless IsConcreteAstNode(item)
			
			switchableFields = GetSwitchableFields(item)
			resultingNodeType = GetResultingTransformerNode(item)
			
			writer.WriteLine("""
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node, ref Boo.Lang.Compiler.Ast.${resultingNodeType} resultingNode)
		{""")
		
			if len(switchableFields):
				writer.WriteLine("""
			if (Enter${item.Name}(node, ref resultingNode))
			{""")
				for field as Field in switchableFields:
					if IsCollectionField(field):
						writer.WriteLine("""
				Switch(node.${field.Name});""")
					else:
						writer.WriteLine("""
				${field.Type} current${field.Name}Value = node.${field.Name};
				if (null != current${field.Name}Value)
				{	
					Node resulting${field.Name}Value;				
					current${field.Name}Value.Switch(this, out resulting${field.Name}Value);					
					node.${field.Name} = resulting${field.Name}Value as ${field.Type};
				}""")
				
				writer.WriteLine("""
				Leave${item.Name}(node, ref resultingNode);
			}""")
			
			writer.WriteLine("""
		}""")
		
			if len(switchableFields):
				writer.WriteLine("""				
		public virtual bool Enter${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node, ref Boo.Lang.Compiler.Ast.${resultingNodeType} resultingNode)
		{
			return true;
		}
		
		public virtual void Leave${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node, ref Boo.Lang.Compiler.Ast.${resultingNodeType} resultingNode)
		{
		}""")
		
		writer.WriteLine("""
		public bool Switch(Node node, out Node resultingNode)
		{			
			if (null != node)
			{			
				node.Switch(this, out resultingNode);
				return true;
			}
			resultingNode = node;
			return false;
		}
		
		public Node SwitchNode(Node node)
		{
			if (null != node)
			{
				Node resultingNode;
				node.Switch(this, out resultingNode);
				return resultingNode;
			}
			return null;
		}
		
		public Node Switch(Node node)
		{
			return SwitchNode(node);
		}
		
		public Expression Switch(Expression node)
		{
			return (Expression)SwitchNode(node);
		}
		
		public Statement Switch(Statement node)
		{
			return (Statement)SwitchNode(node);
		}
		
		public bool Switch(NodeCollection collection)
		{
			if (null != collection)
			{
				int removed = 0;
				
				Node[] nodes = collection.ToArray();
				for (int i=0; i<nodes.Length; ++i)
				{
					Node resultingNode;
					Node currentNode = nodes[i];
					currentNode.Switch(this, out resultingNode);
					if (currentNode != resultingNode)
					{
						int actualIndex = i-removed;
						if (null == resultingNode)
						{
							collection.RemoveAt(actualIndex);
						}
						else
						{
							collection.ReplaceAt(actualIndex, resultingNode);
						}
					}
				}
				return true;
			}
			return false;
		}
	}
}
""")

def WriteSwitcher(module as Module):
	using writer=OpenFile(GetPath("IAstSwitcher.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write(
"""
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public interface IAstSwitcher
	{
""")
		for member as TypeMember in module.Members:
			if IsConcreteAstNode(member):
				writer.WriteLine("\t\tvoid On${member.Name}(${member.Name} node);")

		writer.Write(
"""
	}
}
""")

def GetTypeHierarchy(item as ClassDefinition):
	types = []
	ProcessTypeHierarchy(types, item)
	return types
	
def ProcessTypeHierarchy(types as List, item as ClassDefinition):
	module as Module = item.ParentNode
	for baseTypeRef as SimpleTypeReference in item.BaseTypes:
		if baseType = module.Members[baseTypeRef.Name]:
			ProcessTypeHierarchy(types, baseType)
	types.Add(item)
	
def GetPrivateName(field as Field):
	name = field.Name
	return "_" + name[0:1].ToLower() + name[1:]
	
def GetSimpleFields(node as ClassDefinition):
	
	fields = []
	for field as Field in node.Members:
		if IsCollectionField(field) or field.Attributes.Contains("auto"):
			continue
		fields.Add(field)
	return fields
	
def GetAllFields(node as ClassDefinition):
	fields = []
	module as Module = node.ParentNode
	
	for item as TypeDefinition in GetTypeHierarchy(node):
		for field as Field in item.Members:
			fields.Add(field)
	return fields

def GetSwitchableFields(item as ClassDefinition):
	fields = []
	
	module as Module = item.ParentNode
	
	for item as TypeDefinition in GetTypeHierarchy(item):	
		for field as Field in item.Members:
			type = ResolveFieldType(field)
			if type:
				if not IsEnum(type):
					fields.Add(field)
	
	return fields

def WriteDepthFirstSwitch(writer as TextWriter, item as ClassDefinition):
	fields = GetSwitchableFields(item)
	
	if len(fields):
		writer.WriteLine("""
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{				
			if (Enter${item.Name}(node))
			{""")
			
		for field as Field in fields:
			writer.WriteLine("\t\t\t\tSwitch(node.${field.Name});")
			
		writer.Write(
"""				Leave${item.Name}(node);
			}
		}
			
		public virtual bool Enter${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
			return true;
		}
		
		public virtual void Leave${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
			""")
	else:
		writer.Write("""
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
			""")

def WriteDepthFirstSwitcher(module as Module):
	using writer=OpenFile(GetPath("DepthFirstSwitcher.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write(
"""
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public class DepthFirstSwitcher : IAstSwitcher
	{
		public bool Switch(Node node)
		{			
			if (null != node)
			{
				try
				{
					node.Switch(this);
					return true;
				}
				catch (Boo.Lang.Compiler.CompilerError)
				{
					throw;
				}
				catch (Exception error)
				{
					throw Boo.Lang.Compiler.CompilerErrorFactory.InternalError(node, error);
				}
			}
			return false;
		}
		
		public bool Switch(NodeCollection collection, NodeType nodeType)
		{
			if (null != collection)
			{
				foreach (Node node in collection.ToArray())
				{
					if (node.NodeType == nodeType)
					{
						Switch(node);
					}
				}
				return true;
			}
			return false;
		}
		
		public bool Switch(NodeCollection collection)
		{
			if (null != collection)
			{
				foreach (Node node in collection.ToArray())
				{
					Switch(node);
				}
				return true;
			}
			return false;
		}
		""")
		
		for member as TypeMember in module.Members:
			if IsConcreteAstNode(member):
				WriteDepthFirstSwitch(writer, member)
		
		writer.Write("""
	}
}
""")

start = date.Now

module = BooParser.ParseFile("ast.model.boo").Modules[0]

WriteSwitcher(module)
WriteDepthFirstSwitcher(module)
WriteTransformer(module)
WriteDepthFirstTransformer(module)
WriteNodeTypeEnum(module)
for member as TypeMember in module.Members:

	if member.Attributes.Contains("ignore"):
		continue
		
	if IsEnum(member):
		WriteEnum(member)
	else:
		if IsCollection(member):
			WriteCollection(member)
			WriteCollectionImpl(member)
		else:
			WriteClass(member)
			WriteClassImpl(member)
			
stop = date.Now
print("ast classes generated in ${stop-start}.")
