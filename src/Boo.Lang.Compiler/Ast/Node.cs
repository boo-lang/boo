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

using System.Collections.Generic;

namespace Boo.Lang.Compiler.Ast
{
	using System;
	using System.Xml.Serialization;
	
	public delegate bool NodePredicate(Node node);

	/// <summary>
	/// Base class for every node in the AST.
	/// </summary>
	[Serializable]
	public abstract class Node : ICloneable
	{
		public static bool Matches<T>(T lhs, T rhs) where T : Node
		{
			return lhs == null ? rhs == null : lhs.Matches(rhs);
		}
		
		public static bool Matches(Block lhs, Block rhs)
		{
			return lhs == null ? rhs == null || rhs.IsEmpty : lhs.Matches(rhs);
		}

		public static bool AllMatch<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) where T : Node
		{
			if (lhs == null) return rhs == null || IsEmpty(rhs);
			if (rhs == null) return IsEmpty(lhs);

			IEnumerator<T> r = rhs.GetEnumerator();
			foreach (T item in lhs)
			{
				if (!r.MoveNext()) return false;
				if (!Matches(item, r.Current)) return false;
			}
			if (r.MoveNext()) return false;
			return true;
		}

		private static bool IsEmpty<T>(IEnumerable<T> e)
		{
			return !e.GetEnumerator().MoveNext();
		}

		protected LexicalInfo _lexicalInfo = LexicalInfo.Empty;
		
		protected SourceLocation _endSourceLocation = LexicalInfo.Empty;

		protected Node _parent;
		
		protected string _documentation;
		
		protected System.Collections.Hashtable _annotations;
		
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
			_isSynthetic = other.IsSynthetic;
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
				if (_annotations == null) return null;
				return _annotations[key];
			}
			
			set			
			{
				if (_annotations == null) _annotations = new System.Collections.Hashtable();
				_annotations[key] = value;
			}
		}



		public bool HasAnnotations
		{
			get { return _annotations != null && _annotations.Count > 0; }
		}
		
		public void Annotate(object key)
		{
			Annotate(key, key);
		}
		
		public void Annotate(object key, object value)
		{
			if (_annotations == null) _annotations = new System.Collections.Hashtable();
			_annotations.Add(key, value);
		}
		
		public bool ContainsAnnotation(object key)
		{
			if (_annotations == null) return false;
			return _annotations.ContainsKey(key);
		}
		
		public void RemoveAnnotation(object key)
		{
			if (_annotations == null) return;
			_annotations.Remove(key);
		}
		
		internal virtual void ClearTypeSystemBindings()
		{
			_annotations = null;
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
				if(_lexicalInfo.Equals(LexicalInfo.Empty) && null != ParentNode && null != ParentNode.LexicalInfo)
				{
					_lexicalInfo = ParentNode.LexicalInfo;
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
		
		private sealed class ReplaceVisitor : DepthFirstTransformer
		{
			NodePredicate _predicate;
			Node _template;	
			int _matches;
			
			public ReplaceVisitor(NodePredicate predicate, Node template)
			{
				_predicate = predicate;
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
				if (_predicate(node))
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
		/// <returns>the number of nodes replaced</returns>
		public int ReplaceNodes(Node pattern, Node template)
		{
			NodePredicate predicate = new NodePredicate(pattern.Matches);
			return ReplaceNodes(predicate, template);
		}

		/// <summary>
		/// Replaces all node for which predicate returns true anywhere in the tree
		/// with a clone of template.
		/// </summary>
		/// <returns>the number of nodes replaced</returns>
		public int ReplaceNodes(NodePredicate predicate, Node template)
		{
			ReplaceVisitor visitor = new ReplaceVisitor(predicate, template);
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

		protected bool NoMatch(string fieldName)
		{
			//helps debugging Node.Matches logic
			//Console.Error.WriteLine("No match for '{0}'.", fieldName);
			return false;
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


		///<summary>
		///Returns the closest ancestor node of type <paramref name="ancestorType"/>
		///or null if no ancestor of requested type has been found.
		///</summary>
		///<param name="ancestorType">The type of node you request.</param>
		public Node GetAncestor(NodeType ancestorType)
		{
			return GetAncestor(ancestorType, int.MaxValue);
		}

		///<summary>
		///Returns the closest ancestor node of type <paramref name="ancestorType"/>
		///within <paramref name="limitDepth"/> or null if no ancestor of requested
		///type has been found.
		///</summary>
		///<param name="ancestorType">The type of node you request.</param>
		///<param name="limitDepth">Maximum depth difference from this node to ancestor.</param>
		public Node GetAncestor(NodeType ancestorType, int limitDepth)
		{
			Node parent = this.ParentNode;
			while (null != parent && limitDepth > 0)
			{
				if (ancestorType == parent.NodeType)
					return parent;
				parent = parent.ParentNode;
				limitDepth--;
			}
			return null;
		}

		///<summary>
		///Returns the closest ancestor node of type <paramref name="TAncestor"/>
		///or null if no ancestor of requested type has been found.
		///</summary>
		///<param name="TAncestor">The type of node you request.</param>
		public TAncestor GetAncestor<TAncestor>() where TAncestor : Node
		{
			Node parent = this.ParentNode;
			while (parent != null)
			{
				TAncestor ancestor = parent as TAncestor;
				if (null != ancestor)
					return ancestor;
				parent = parent.ParentNode;
			}
			return null;
		}

		///<summary>
		///Returns the farthest ancestor node of type <paramref name="TAncestor"/>
		///or null if no ancestor of requested type has been found.
		///</summary>
		///<param name="TAncestor">The type of node you request.</param>
		public TAncestor GetRootAncestor<TAncestor>() where TAncestor : Node
		{
			TAncestor root = null;
			foreach (TAncestor ancestor in GetAncestors<TAncestor>())
				root = ancestor;
			return root;
		}

		///<summary>
		///Yields <paramref name="TAncestor"/> ancestors in order from closest to farthest from this node.
		///</summary>
		///<param name="TAncestor">The type of node you request.</param>
		public IEnumerable<TAncestor> GetAncestors<TAncestor>() where TAncestor : Node
		{
			Node parent = this.ParentNode;
			while (parent != null)
			{
				TAncestor ancestor = parent as TAncestor;
				if (null != ancestor)
					yield return ancestor;
				parent = parent.ParentNode;
			}
		}
	}
}

