using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(Module))]
	[System.Xml.Serialization.XmlInclude(typeof(ClassDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(InterfaceDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(EnumDefinition))]
	[Serializable]
	public abstract class TypeDefinition : TypeDefinitionImpl
	{		
		public TypeDefinition()
		{
			_members = new TypeMemberCollection(this);
			_baseTypes = new TypeReferenceCollection(this);
 		}
		
		internal TypeDefinition(antlr.Token token) : base(token)
		{
		}
		
		internal TypeDefinition(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public virtual string FullyQualifiedName
		{
			get
			{
				Package package = EnclosingPackage;
				if (null != package)
				{
					return package.Name + "." + Name;
				}
				return Name;
			}
		}
		
		public virtual Package EnclosingPackage
		{
			get
			{
				Node parent = _parent;
				while (parent != null)
				{
					Module module = parent as Module;
					if (null != module)
					{
						return module.Package;
					}
					parent = parent.ParentNode;
				}
				return null;
			}
		}
	}
}
