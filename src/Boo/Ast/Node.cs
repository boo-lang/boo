using System;
using System.Collections;
using System.IO;
using antlr;

namespace Boo.Ast
{
	/// <summary>
	/// Classe base para todos os ns da AST que mantm
	/// detalhes lxicos.
	/// </summary>
	[Serializable]
	public abstract class Node
	{
		protected LexicalInfo _lexicalData;

		protected Node _parent;
		
		protected Hashtable _properties = new Hashtable();

		protected Node()
		{
			_lexicalData = LexicalInfo.Empty;
		}

		protected Node(Token token)
		{			
			_lexicalData = new LexicalInfo(token);
		}

		protected Node(Node lexicalInfoProvider)
		{
			_lexicalData = lexicalInfoProvider.LexicalInfo;
		}

		protected void InitializeFrom(Node other)
		{
			_lexicalData = other.LexicalInfo;
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

		[System.Xml.Serialization.XmlIgnore]
		public LexicalInfo LexicalInfo
		{
			get
			{
				// se temos informaes lxicas
				if (null != _lexicalData)
				{
					return _lexicalData;
				}
				// se no temos mas temos um
				// n pai perguntamos a ele
				if (null != _parent)
				{
					return _parent.LexicalInfo;
				}
				// infelizmente, nada de informaes
				// lxicas
				return null;
			}

			set
			{
				_lexicalData = value;
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
