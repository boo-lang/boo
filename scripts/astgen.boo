import System
import System.IO
import Boo.Antlr from Boo.Antlr
import Boo.Lang.Ast

def WriteNodeTypeEnum(module as Module):
	using writer=OpenFile(GetPath("NodeType.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.WriteLine("""
namespace Boo.Lang.Ast
{
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
namespace Boo.Lang.Ast
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

def WriteClassImpl(node as ClassDefinition):
	using writer=OpenFile(GetPath("Impl/${node.Name}Impl.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write("""
namespace Boo.Lang.Ast.Impl
{
	using System;
	using Boo.Lang.Ast;
	
	[Serializable]
	public abstract class ${node.Name}Impl : ${join(node.BaseTypes, ', ')}
	{
		protected ${node.Name}Impl()
		{
			InitializeFields();
		}
		
		protected ${node.Name}Impl(LexicalInfo info) : base(info)
		{
			InitializeFields();
		}
	}
}
		""")
		

def WriteClass(node as ClassDefinition):
	path = GetPathFromNode(node);
	return if File.Exists(path)
	
	using writer=OpenFile(path):
		WriteLicense(writer)
		writer.Write("""
namespace Boo.Lang.Ast
{
	using System;
	
	[Serializable]
	public class ${node.Name} : Boo.Lang.Ast.Impl.${node.Name}Impl
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
namespace Boo.Lang.Ast
{
	using System;
	
	public class ${node.Name} : Boo.Lang.Ast.Impl.${node.Name}Impl
	{
		public ${node.Name}()
		{
		}
		
		public ${node.Name}(Boo.Lang.Ast.Node parent) : base(parent)
		{
		}
	}
}
""")

def GetCollectionItemType(node as ClassDefinition):
	attribute = node.Attributes.Get("collection")[0]
	reference as ReferenceExpression = attribute.Arguments[0]
	return reference.Name

def WriteCollectionImpl(node as ClassDefinition):
	path = GetPath("Impl/${node.Name}Impl.cs")
	using writer=OpenFile(path):
		WriteLicense(writer)
		WriteWarning(writer)
		
		itemType = "Boo.Lang.Ast." + GetCollectionItemType(node)
		writer.Write("""
namespace Boo.Lang.Ast.Impl
{
	using System;
	using Boo.Lang.Ast;
	
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
		
		public void Add(params ${itemType}[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (${itemType} item in items)
			{
				base.Add(item);
			}
		}
		
		public void AddClones(System.Collections.ICollection items)
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
	return Path.Combine("src/Boo/Lang/Ast", fname)
	
def GetPathFromNode(node as TypeMember):
	return GetPath("${node.Name}.cs")
	
def IsCollection(node as TypeMember):
	return node.Attributes.Contains("collection")
	
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
""")

def WriteSwitcher(module as Module):
	using writer=OpenFile(GetPath("IAstSwitcher.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write(
"""
namespace Boo.Lang.Ast
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
	
def GetPrivateName(name as string):
	return "_" + name[0:1].ToLower() + name[1:]

def GetSwitchableFields(item as ClassDefinition):
	fields = []
	
	module as Module = item.ParentNode
	
	for item as TypeDefinition in GetTypeHierarchy(item):	
		for field as Field in item.Members:
			type = module.Members[(field.Type as SimpleTypeReference).Name]
			if type:
				if not IsEnum(type):
					fields.Add(field)
	
	return fields

def WriteDepthFirstSwitch(writer as TextWriter, item as ClassDefinition):
	fields = GetSwitchableFields(item)
	
	if len(fields):
		writer.WriteLine("""
		public virtual void On${item.Name}(Boo.Lang.Ast.${item.Name} node)
		{				
			if (Enter${item.Name}(node))
			{""")
			
		for field as Field in fields:
			writer.WriteLine("\t\t\t\tSwitch(node.${field.Name});")
			
		writer.Write(
"""				Leave${item.Name}(node);
			}
		}
			
		public virtual bool Enter${item.Name}(Boo.Lang.Ast.${item.Name} node)
		{
			return true;
		}
		
		public virtual void Leave${item.Name}(Boo.Lang.Ast.${item.Name} node)
		{
		}
			""")
	else:
		writer.Write("""
		public virtual void On${item.Name}(Boo.Lang.Ast.${item.Name} node)
		{
		}
			""")

def WriteDepthFirstSwitcher(module as Module):
	using writer=OpenFile(GetPath("DepthFirstSwitcher.cs")):
		WriteLicense(writer)
		WriteWarning(writer)
		writer.Write(
"""
namespace Boo.Lang.Ast
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
