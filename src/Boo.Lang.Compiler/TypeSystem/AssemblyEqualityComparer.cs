using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{
	class AssemblyEqualityComparer : IEqualityComparer<Assembly>
	{
		public static readonly IEqualityComparer<Assembly> Default = new AssemblyEqualityComparer();

		private AssemblyEqualityComparer()
		{	
		}

		public bool Equals(Assembly x, Assembly y)
		{
			return x.FullName == y.FullName;
		}

		public int GetHashCode(Assembly obj)
		{
			return obj.FullName.GetHashCode();
		}
	}
}
