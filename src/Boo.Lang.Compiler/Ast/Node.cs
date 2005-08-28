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
	using System.Xml.Serialization;

	/// <summary>
	/// Base class for every node in the AST.
	/// </summary>
	[Serializable]
	public abstract class Node : ICloneable
	{
		protected LexicalInfo _lexicalInfo = LexicalInfo.Empty;
		
		protected SourceLocation _endSourceLocation = LexicalInfo.Empty;

		protected Node _parent;
		
		protected string _documentation;
		
		protected System.Collections.Hashtable _annotations = new System.Collections.Hashtable();
		
		protected bool _isSynthetic;

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
		
		/// <summary>
		/// true when the node was constructed by the compiler.
		/// </summary>
		[XmlAttribute]
		[System.ComponentModel.DefaultValue(false)]
		public bool IsSynthetic
		{
			get
			{
				return _isSynthetic;
			}
			
			set
			{
				_isSynthetic = value;
			}
		}
		
		[XmlIgnore]
		internal Boo.Lang.Compiler.TypeSystem.IEntity Entity
		{
			get
			{
				return Boo.Lang.Compiler.TypeSystem.TypeSystemServices.GetOptionalEntity(this);
			}
			
			set
			{
				Boo.Lang.Compiler.TypeSystem.TypeSystemServices.Bind(this, value);
			}
		}
		
		public object this[object key]
		{
			get
			{
				return _annotations[key];
			}
			
			set			
			{
				_annotations[key] = value;
			}
		}
		
		public bool ContainsAnnotation(object key)
		{
			return _annotations.ContainsKey(key);
		}
		
		public void RemoveAnnotation(object key)
		{
			_annotations.Remove(key);
		}
		
		internal virtual void ClearTypeSystemBindings()
		{
			_annotations.Clear();
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

		[XmlIgnore]
		public LexicalInfo LexicalInfo
		{
			get
			{	
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
		
		/// <summary>
		/// Where this element ends in the source file.
		/// This information is generally available and/or accurate
		/// only for blocks and type definitions.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual SourceLocation EndSourceLocation
		{
			get
			{
				return _endSourceLocation;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("EndSourceLocation");
				}
				_endSourceLocation = value;
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
		
		private class ReplaceVisitor : DepthFirstTransformer
		{
			Node _pattern;
			Node _template;	
			int _matches;
			
			public ReplaceVisitor(Node pattern, Node template)
			{
				_pattern = pattern;
				_template = template;
			}
			
			public int Matches
			{
				get
				{
					return _matches;
				}
			}
	
			override protected void OnNode(Node node)
			{
				if (_pattern.Matches(node))
				{
					++_matches;
					ReplaceCurrentNode(_template.CloneNode());
				}
				else
				{
					base.OnNode(node);
				}
			}
		}
		
		/// <summary>
		/// Replaces all occurrences of the pattern pattern anywhere in the tree
		/// with a clone of template.
		/// </summary>
		/// <returns>the number of which matched the specified pattern</returns>
		public int ReplaceNodes(Node pattern, Node template)
		{
			ReplaceVisitor visitor = new ReplaceVisitor(pattern, template);
			Accept(visitor);
			return visitor.Matches;
		}

		internal void InitializeParent(Node parent)
		{			
			_parent = parent;
		}

		public abstract void Accept(IAstVisitor visitor);
		
		public abstract object Clone();
		
		public abstract bool Matches(Node other);
		
		public static bool Matches(Node lhs, Node rhs)
		{
			return lhs == null
				? rhs == null
				: lhs.Matches(rhs);
		}
		
		public static bool Matches(NodeCollection lhs, NodeCollection rhs)
		{
			return lhs == null
				? rhs == null
				: lhs.Matches(rhs);
		}
		
		public abstract NodeType NodeType
		{
			get;
		}
		
		override public string ToString()
		{
			return ToCodeString();
		}
		
		public string ToCodeString()
		{
			System.IO.StringWriter writer = new System.IO.StringWriter();
			new Visitors.BooPrinterVisitor(writer).Visit(this);
			return writer.ToString();
		}
	}

}
