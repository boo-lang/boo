#region license
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
	public abstract class Node : ICloneable
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
		
		public Node CloneNode()
		{
			return (Node)Clone();
		}
		
		public abstract object Clone();
		
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

		protected string GetString(string name)
		{
			return ResourceManager.GetString(name);
		}

		public abstract void Switch(IAstSwitcher switcher);
		
		public abstract void Switch(IAstTransformer transformer, out Node resultingNode);
	}

}
