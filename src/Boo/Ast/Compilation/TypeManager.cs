using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;

namespace Boo.Ast.Compilation
{
	public class TypeManager
	{
		public static readonly Type VoidType = typeof(void);
		
		public static readonly Type StringType = typeof(string);
		
		public void SetMemberInfo(Node node, MemberInfo mi)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			if (null == mi)
			{
				throw new ArgumentNullException("mi");
			}
			
			node[MemberInfoKey] = mi;
		}
		
		public MemberInfo GetMemberInfo(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			MemberInfo mi = (MemberInfo)node[MemberInfoKey];
			if (null == mi)
			{
				throw new Error(node, ResourceManager.GetString("TypeManager.UnresolvedNode"));
			}
			return mi;
		}	
		
		static object MemberInfoKey = new object();
	}
}
