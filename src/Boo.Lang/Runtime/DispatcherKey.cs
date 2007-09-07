using System;
using System.Collections.Generic;

namespace Boo.Lang.Runtime
{
	public class DispatcherKey
	{
		public static readonly Type[] NoArguments = new Type[0];

		public static readonly IEqualityComparer<DispatcherKey> EqualityComparer = new _EqualityComparer();

		private readonly Type _type;
		private readonly string _name;
		private readonly Type[] _arguments;

		public DispatcherKey(Type type, string name) : this(type, name, NoArguments)
		{
		}

		public DispatcherKey(Type type, string name, Type[] arguments)
		{
			_type = type;
			_name = name;
			_arguments = arguments;
		}

		public DispatcherKey(Type type, string name, object[] arguments) : this(type, name, MethodResolver.GetArgumentTypes(arguments))
		{	
		}

		public Type[] Arguments
		{
			get { return _arguments;  }
		}

		class _EqualityComparer : IEqualityComparer<DispatcherKey>
		{
			public int GetHashCode(DispatcherKey key)
			{
				return key._type.GetHashCode() ^ key._name.GetHashCode() ^ key._arguments.Length;
			}

			public bool Equals(DispatcherKey x, DispatcherKey y)
			{
				if (x._type != y._type) return false;
				if (x._arguments.Length != y._arguments.Length) return false;
				if (x._name != y._name) return false;
				for (int i = 0; i < x._arguments.Length; ++i)
				{
					if (x._arguments[i] != y._arguments[i]) return false;
				}
				return true;
			}
		}
	}
}