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

using System;
using System.Collections;
using System.IO;

namespace Boo.Lang.Ast
{
	/// <summary>
	/// Classe base para todos os ns da AST que mantm
	/// detalhes lxicos.
	/// </summary>
	[Serializable]
	public abstract class Node
	{
		protected LexicalInfo _lexicalInfo = LexicalInfo.Empty;

		protected Node _parent;
		
		protected Hashtable _properties = new Hashtable();
		
		protected string _documentation;

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
		
		public abstract NodeType NodeType
		{
			get;
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
		
		public object this[object key]
		{
			get
			{
				if (null == key)
				{
					throw new ArgumentNullException("key");
				}
				return _properties[key];
			}
			
			set
			{
				if (null == key)
				{
					throw new ArgumentNullException("key");
				}
				_properties[key] = value;
			}
		}

		internal void InitializeParent(Node parent)
		{			
			_parent = parent;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object rhs)
		{
			Node other = rhs as Node;
			if (null == other)
			{
				return false;
			}
			if (GetType() != other.GetType())
			{
				return false;
			}
			return Equals(other);
		}

		protected virtual bool Equals(Node rhs)
		{
			return true;
		}

		protected string GetString(string name)
		{
			return ResourceManager.GetString(name);
		}

		public abstract void Switch(IAstSwitcher switcher);
		
		public abstract void Switch(IAstTransformer transformer, out Node resultingNode);
	}

}
