using System;
using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{
	/// <summary>
	/// </summary>
	public class MetadataUtil
	{
		private MetadataUtil()
		{
		}

		public static bool IsAttributeDefined(MemberInfo member, Type attributeType)
		{
			return Attribute.IsDefined(member, attributeType);
#if CHECK_ATTRIBUTES_BY_NAME
			// check attribute by name to account for different 
			// loaded modules (and thus different type identities)
			string attributeName = attributeType.FullName;
			Attribute[] attributes = Attribute.GetCustomAttributes(member);
			foreach (Attribute a in attributes)
			{
				if (a.GetType().FullName == attributeName) return true;
			}
			return false;
#endif
		}
	}
}
