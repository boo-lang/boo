using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;

namespace Boo.Ast.Compilation.NameBinding
{
	public class TypeManager
	{
		public static readonly Type VoidType = typeof(void);
		
		public static readonly Type StringType = typeof(string);
		
		public bool HasNameInfo(Node node)
		{
			return null != node[NameInfoKey];
		}
		
		public void SetNameInfo(Node node, INameInfo mi)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			if (null == mi)
			{
				throw new ArgumentNullException("mi");
			}
			
			node[NameInfoKey] = mi;
		}
		
		public void SetNameInfo(TypeDefinition type, TypeBuilder builder)
		{
			SetNameInfo(type, new InternalTypeInfo(this, type, builder));
		}
		
		public void SetNameInfo(Method method, MethodBuilder builder)
		{
			SetNameInfo(method, new InternalMethodInfo(this, method, builder));
		}
		
		public void SetNameInfo(Expression expression, Type type)
		{
			SetNameInfo(expression, ToTypeInfo(type));
		}		
		
		public INameInfo GetNameInfo(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			INameInfo info = (INameInfo)node[NameInfoKey];
			if (null == info)
			{
				throw new Error(node, ResourceManager.GetString("TypeManager.UnresolvedNode"));
			}
			return info;
		}	
		
		public ITypeInfo ToTypeInfo(System.Type type)
		{
			return new ExternalTypeInfo(this, type);
		}
		
		public INameInfo ToNameInfo(System.Reflection.MemberInfo[] info)
		{
			if (info.Length > 1)
			{
				throw new NotImplementedException();
			}
			return ToNameInfo(info[0]);
		}
		
		public INameInfo ToNameInfo(System.Reflection.MemberInfo mi)
		{
			switch (mi.MemberType)
			{
				case MemberTypes.Method:
				{
					return new ExternalMethodInfo(this, (System.Reflection.MethodInfo)mi);
					break;
				}
				
				default:
				{
					throw new NotImplementedException();
				}
			}
		}
		
		public TypeBuilder GetTypeBuilder(TypeDefinition type)
		{
			return ((InternalTypeInfo)GetNameInfo(type)).TypeBuilder;
		}
		
		public MethodBuilder GetMethodBuilder(Method method)
		{
			return ((InternalMethodInfo)GetNameInfo(method)).MethodBuilder;
		}
		
		public MethodInfo GetMethodInfo(Node node)
		{
			return ((IMethodInfo)GetNameInfo(node)).MethodInfo;
		}
		
		public ITypeInfo GetTypeInfo(Node node)
		{
			return (ITypeInfo)GetNameInfo(node);
		}
		
		public System.Type GetType(Node node)
		{
			return GetTypeInfo(node).Type;
		}		
		
		public LocalInfo GetLocalInfo(Local local)
		{
			return (LocalInfo)GetNameInfo(local);
		}
		
		static object NameInfoKey = new object();
	}
}
