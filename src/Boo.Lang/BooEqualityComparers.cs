using System;
using System.Collections.Generic;

namespace Boo.Lang
{
	public static class BooEqualityComparers
	{
		public static readonly IEqualityComparer<object> Default = new ObjectEqualityComparer();

		public static readonly IEqualityComparer<object> CaseInsensitive = new CaseInsensitiveComparer();

		private abstract class EqualityComparer : IEqualityComparer<object>
		{
			public abstract bool Equals(object lhs, object rhs);

			public virtual int GetHashCode(object o)
			{
				if (o == null)
					return 0;

				var array = o as Array;
				return array != null ? GetArrayHashCode(array) : o.GetHashCode();
			}

			private int GetArrayHashCode(Array array)
			{
				var code = 1;
				var position = 0;
				foreach (object item in array)
					code ^= GetHashCode(item) * (++position);
				return code;
			}
		}

		[Serializable]
		private class ObjectEqualityComparer : EqualityComparer
		{
			public override bool Equals(object lhs, object rhs)
			{
				return BooComparer.Default.Compare(lhs, rhs) == 0;
			}
		}

		[Serializable]
		private class CaseInsensitiveComparer : EqualityComparer
		{
			public override int GetHashCode(object o)
			{
				var s = o as string;
				if (s != null)
					return Comparer.GetHashCode(s);

				return base.GetHashCode();
			}

			public override bool Equals(object x, object y)
			{
				return Comparer.Equals(x, y);
			}

			private static StringComparer Comparer
			{
				get { return StringComparer.OrdinalIgnoreCase; }
			}
		}
	}
}