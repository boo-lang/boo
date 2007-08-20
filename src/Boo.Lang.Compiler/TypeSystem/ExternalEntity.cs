using System;
using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IExternalEntity : IEntity
	{
		System.Reflection.MemberInfo Member
		{
			get;
		}
	}

	public abstract class ExternalEntity<T> : IExternalEntity
		where T: System.Reflection.MemberInfo
	{
		protected readonly T _memberInfo;

		private string _cachedFullName;

		protected readonly TypeSystemServices _typeSystemServices;

		public ExternalEntity(TypeSystemServices typeSystemServices, T memberInfo)
		{
			_typeSystemServices = typeSystemServices;
			_memberInfo = memberInfo;
		}

		public MemberInfo Member
		{
			get { return _memberInfo;  }
		}

		public string Name
		{
			get { return _memberInfo.Name; }
		}

		public string FullName
		{
			get
			{
				if (_cachedFullName != null) return _cachedFullName;
				return _cachedFullName = BuildFullName();
			}
		}

		protected virtual string BuildFullName()
		{
			return Map(_memberInfo.DeclaringType).FullName + "." + _memberInfo.Name;
		}

		protected IType Map(Type type)
		{
			return _typeSystemServices.Map(type);
		}

		public abstract EntityType EntityType { get; }

		public override string ToString()
		{
			return FullName;
		}
	}
}
