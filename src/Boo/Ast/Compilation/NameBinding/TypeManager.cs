using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;

namespace Boo.Ast.Compilation.Binding
{
	public class BindingManager
	{
		public static readonly Type ObjectType = typeof(object);
		
		public static readonly Type ObjectArrayType = Type.GetType("System.Object[]");
		
		public static readonly Type VoidType = typeof(void);
		
		public static readonly Type StringType = typeof(string);
		
		public bool IsBound(Node node)
		{
			return null != node[BindingKey];
		}
		
		public void Bind(Node node, IBinding mi)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			if (null == mi)
			{
				throw new ArgumentNullException("mi");
			}
			
			node[BindingKey] = mi;
		}
		
		public void Bind(TypeDefinition type, TypeBuilder builder)
		{
			Bind(type, new InternalTypeBinding(this, type, builder));
		}
		
		public void Bind(Method method, MethodBuilder builder)
		{
			Bind(method, new InternalMethodBinding(this, method, builder));
		}
		
		public void Bind(Expression expression, Type type)
		{
			Bind(expression, ToTypeBinding(type));
		}		
		
		public IBinding GetBinding(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			IBinding binding = (IBinding)node[BindingKey];
			if (null == binding)
			{
				throw new Error(node, ResourceManager.Format("BindingManager.UnboundNode", node, node.LexicalInfo));
			}
			return binding;
		}	
		
		public ITypeBinding ToTypeBinding(System.Type type)
		{
			return new ExternalTypeBinding(this, type);
		}
		
		public IBinding ToBinding(System.Reflection.MemberInfo[] info)
		{
			if (info.Length > 1)
			{
				throw new NotImplementedException();
			}
			return ToBinding(info[0]);
		}
		
		public IBinding ToBinding(System.Reflection.MemberInfo mi)
		{
			switch (mi.MemberType)
			{
				case MemberTypes.Method:
				{
					return new ExternalMethodBinding(this, (System.Reflection.MethodInfo)mi);
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
			return ((InternalTypeBinding)GetBinding(type)).TypeBuilder;
		}
		
		public MethodBuilder GetMethodBuilder(Method method)
		{
			return ((InternalMethodBinding)GetBinding(method)).MethodBuilder;
		}
		
		public MethodInfo GetMethodInfo(Node node)
		{
			return ((IMethodBinding)GetBinding(node)).MethodInfo;
		}
		
		public ITypeBinding GetTypeBinding(Node node)
		{
			return (ITypeBinding)GetBinding(node);
		}
		
		public System.Type GetBoundType(Node node)
		{
			return GetTypeBinding(node).Type;
		}		
		
		public LocalBinding GetLocalBinding(Local local)
		{
			return (LocalBinding)GetBinding(local);
		}
		
		static object BindingKey = new object();
	}
}
