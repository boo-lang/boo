using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class InternalEntity<T> : IInternalEntity where T : TypeMember
	{
		protected readonly T _node;

		public InternalEntity(T node)
		{
			_node = node;
		}

		#region IInternalEntity Members
		public Node Node
		{
			get { return _node; }
		}
		#endregion

		#region IEntity Members
		public string Name
		{
			get { return _node.Name; }
		}

		public virtual string FullName
		{
			get { return _node.DeclaringType.FullName + "." + _node.Name; }
		}
		#endregion

		public IType DeclaringType
		{
			get { return (IType)TypeSystemServices.GetEntity(_node.DeclaringType); }
		}

		public bool IsDefined(IType type)
		{
			return MetadataUtil.IsAttributeDefined(_node, type);
		}

		public virtual bool IsStatic
		{
			get { return _node.IsStatic; }
		}

		public virtual bool IsPublic
		{
			get { return _node.IsPublic; }
		}

		public virtual bool IsProtected
		{
			get { return _node.IsProtected; }
		}

		public virtual bool IsPrivate
		{
			get { return _node.IsPrivate; }
		}

		public virtual bool IsInternal
		{
			get { return _node.IsInternal; }
		}

		public abstract EntityType EntityType { get; }
	}
}
