using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;

namespace Boo.Ast.Compilation.Binding
{
	public class BindingManager
	{		
		public ITypeBinding ObjectTypeBinding;
	
		public ITypeBinding VoidTypeBinding;
		
		public ITypeBinding StringTypeBinding;
		
		public ITypeBinding BoolTypeBinding;
		
		public ITypeBinding IntTypeBinding;
		
		public ITypeBinding RuntimeServicesBinding;
		
		System.Collections.Hashtable _bindingCache = new System.Collections.Hashtable();
		
		public BindingManager()
		{
			Cache(VoidTypeBinding = new VoidTypeBindingImpl());
			Cache(ObjectTypeBinding = new ExternalTypeBinding(this, Types.Object));
			Cache(StringTypeBinding = new ExternalTypeBinding(this, Types.String));
			Cache(BoolTypeBinding = new ExternalTypeBinding(this, Types.Bool));
			Cache(IntTypeBinding = new ExternalTypeBinding(this, Types.Int));
			Cache(new ExternalTypeBinding(this, Types.Date));
			Cache(RuntimeServicesBinding = new ExternalTypeBinding(this, Types.RuntimeServices));
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
			Bind(type, ToTypeBinding(type, builder));
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
		
		public ITypeBinding GetTypeBinding(Node node)
		{
			return ((ITypedBinding)GetBinding(node)).BoundType;
		}
		
		public System.Type GetBoundType(Node node)
		{
			return GetTypeBinding(node).Type;
		}	
		
		public ITypeBinding ToTypeBinding(System.Type type)
		{
			ITypeBinding binding = (ITypeBinding)_bindingCache[type];
			if (null == binding)
			{
				Cache(binding = new ExternalTypeBinding(this, type));
			}
			return binding;
		}
		
		public ITypeBinding ToTypeBinding(TypeDefinition typeDefinition, TypeBuilder builder)
		{
			ITypeBinding binding = (ITypeBinding)_bindingCache[typeDefinition];
			if (null == binding)
			{
				Cache(binding = new InternalTypeBinding(this, typeDefinition, builder));
			}
			return binding;
		}
		
		public ITypedBinding ToTypeReference(ITypeBinding type)
		{
			return new TypeReferenceBinding(type);
		}
		
		public ITypedBinding ToTypeReference(System.Type type)
		{
			return ToTypeReference(ToTypeBinding(type));
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
				
				case MemberTypes.Event:
				{
					return new ExternalEventBinding(this, (System.Reflection.EventInfo)mi);
				}
				
				default:
				{
					throw new NotImplementedException(mi.ToString());
				}
			}
		}
		
		public ITypedBinding ResolvePrimitive(string name)
		{
			ITypedBinding binding = null;
			switch (name)
			{
				case "void":
				{
					binding = ToTypeReference(VoidTypeBinding);
					break;
				}
				
				case "date":
				{
					binding = ToTypeReference(Types.Date);
					break;
				}
				
				case "string":
				{
					binding = ToTypeReference(StringTypeBinding);
					break;
				}
				
				case "int":
				{
					binding = ToTypeReference(IntTypeBinding);
					break;
				}
			}
			return binding;
		}
		
		void Cache(ITypeBinding binding)
		{
			_bindingCache[binding.Type] = binding;
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
		
		#region VoidTypeBindingImpl
		class VoidTypeBindingImpl : ITypeBinding
		{			
			internal VoidTypeBindingImpl()
			{				
			}
			
			public string Name
			{
				get
				{
					return "void";
				}
			}
			
			public BindingType BindingType
			{
				get
				{
					return BindingType.Type;
				}
			}
			
			public ITypeBinding BoundType
			{
				get
				{
					return this;
				}
			}
			
			public Type Type
			{
				get
				{
					return Types.Void;
				}
			}
			
			public IConstructorBinding[] GetConstructors()
			{
				return new IConstructorBinding[0];
			}
			
			public IBinding Resolve(string name)
			{	
				return null;
			}
	
		}

		#endregion
	}
}
