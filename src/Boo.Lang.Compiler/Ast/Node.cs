#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
		
		public object this[object key]
		{
			get
			{
				if (null == _properties)
				{
					return null;
				}
				return _properties[key];
			}
			
			set
			{
				if (null == key)
				{
					throw new ArgumentNullException("key");
				}
				
				if (null == _properties)
				{
					_properties = new Hashtable();
				}
				_properties[key] = value;
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
