#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Ast
{
	using System;
	using System.Collections;
	using System.IO;

	/// <summary>
	/// Base class for every node in the AST.
	/// </summary>
	[Serializable]
	public abstract class Node : ICloneable
	{
		protected LexicalInfo _lexicalInfo = LexicalInfo.Empty;

		protected Node _parent;
		
		protected string _documentation;
		
		protected Boo.Lang.Compiler.TypeSystem.IEntity _entity;
		
		protected System.Collections.Hashtable _properties;

		protected Node()
		{
			_lexicalInfo = LexicalInfo.Empty;
		}

		protected Node(LexicalInfo lexicalInfo)
		{
			if (null == lexicalInfo)
			{
				throw new ArgumentNullException("lexicalInfo");
			}
			_lexicalInfo = lexicalInfo;
		}

		protected void InitializeFrom(Node other)
		{
			_lexicalInfo = other.LexicalInfo;
		}
		
		public Node CloneNode()
		{
			return (Node)Clone();
		}
		
		public Boo.Lang.Compiler.TypeSystem.IEntity Entity
		{
			get
			{
				return _entity;
			}
			
			set
			{
				_entity = value;
			}
		}
		
		public Node ParentNode
		{
			get
			{
				return _parent;
			}
		}
		
		public string Documentation
		{
			get
			{
				return _documentation;
			}
			
			set
			{
				_documentation = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public LexicalInfo LexicalInfo
		{
			get
			{				
				if (LexicalInfo.Empty != _lexicalInfo)
				{
					return _lexicalInfo;
				}
				if (null != _parent)
				{
					return _parent.LexicalInfo;
				}
				return _lexicalInfo;
			}

			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("LexicalInfo");
				}
				_lexicalInfo = value;
			}
		}
		
		public virtual bool Replace(Node existing, Node newNode)
		{
			if (null == existing)
			{
				throw new ArgumentNullException("existing");
			}
			return false;
		}

		internal void InitializeParent(Node parent)
		{			
			_parent = parent;
		}

		public abstract void Accept(IAstVisitor visitor);
		
		public abstract object Clone();
		
		public abstract NodeType NodeType
		{
			get;
		}
	}

}
