using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;

namespace Boo.Ast.Compilation.Binding
{
	public class BindingManager
	{
		public static readonly Type RuntimeServicesType = typeof(Boo.Lang.RuntimeServices);
		
		public static readonly Type IEnumerableType = typeof(System.Collections.IEnumerable);
		
		public static readonly Type IEnumeratorType = typeof(System.Collections.IEnumerator);
		
		public static readonly Type ObjectType = typeof(object);
		
		public static readonly Type ObjectArrayType = Type.GetType("System.Object[]");
		
		public static readonly Type VoidType = typeof(void);
		
		public static readonly Type StringType = typeof(string);
		
		public static readonly Type IntType = typeof(int);
		
		public static readonly Type BoolType = typeof(bool);
		
		public ITypeBinding ObjectTypeBinding;
		
		public ITypeBinding StringTypeBinding;
		
		public ITypeBinding BoolTypeBinding;
		
		public ITypeBinding RuntimeServicesBinding;
		
		public BindingManager()
		{
			ObjectTypeBinding = ToTypeBinding(ObjectType);
			StringTypeBinding = ToTypeBinding(StringType);
			BoolTypeBinding = ToTypeBinding(BoolType);
			RuntimeServicesBinding = ToTypeBinding(RuntimeServicesType);
		}
		
		public bool IsBound(Node node)
		{
			return null != node[BindingKey];
		}
		
		public void Bind(Node node, IBinding binding)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			if (null == binding)
			{
				throw new ArgumentNullException("binding");
			}
			
			node[BindingKey] = binding;
		}
		
		public void Bind(TypeDefinition type, TypeBuilder builder)
		{
			Bind(type, new InternalTypeBinding(this, type, builder));
		}		
		
		public void Error(Node node)
		{
			Bind(node, ErrorBinding.Default);
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
				NodeNotBound(node);
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
				IBinding[] bindings = new IBinding[info.Length];
				for (int i=0; i<bindings.Length; ++i)
				{
					bindings[i] = ToBinding(info[i]);
				}
				return new AmbiguousBinding(bindings);
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
				}
				
				case MemberTypes.Field:
				{
					return new ExternalFieldBinding(this, (System.Reflection.FieldInfo)mi);
				}
				
				case MemberTypes.Property:
				{
					return new ExternalPropertyBinding(this, (System.Reflection.PropertyInfo)mi);
				}
				
				default:
				{
					throw new NotImplementedException(mi.ToString());
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
			return (MethodInfo)((IMethodBinding)GetBinding(node)).MethodInfo;
		}
		
		public ITypeBinding GetTypeBinding(Node node)
		{
			return (ITypeBinding)GetBinding(node);
		}
		
		public System.Type GetBoundType(Node node)
		{
			return ((ITypedBinding)GetBinding(node)).BoundType.Type;
		}		
		
		public LocalBinding GetLocalBinding(Local local)
		{
			return (LocalBinding)GetBinding(local);
		}
		
		public static string GetSignature(IMethodBinding binding)
		{
			MethodBase mi = binding.MethodInfo;
			System.Text.StringBuilder sb = new System.Text.StringBuilder(mi.DeclaringType.FullName);
			sb.Append(".");
			sb.Append(mi.Name);
			sb.Append("(");
			for (int i=0; i<binding.ParameterCount; ++i)
			{				
				if (i>0) 
				{
					sb.Append(", ");
				}
				sb.Append(binding.GetParameterType(i).FullName);
			}
			sb.Append(") as ");
			sb.Append(binding.ReturnType.Type.FullName);
			return sb.ToString();
		}
		
		private static void NodeNotBound(Node node)
		{
			throw new Error(node, ResourceManager.Format("BindingManager.UnboundNode", node, node.LexicalInfo));
		}
		
		static object BindingKey = new object();
	}
}
