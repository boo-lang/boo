#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Reflection;
using Boo.Lang.Ast;

namespace Boo.Lang.Compiler.Bindings
{
	public class BindingManager
	{		
		public ExternalTypeBinding ApplicationExceptionBinding;
		
		public ExternalTypeBinding ObjectTypeBinding;
		
		public ExternalTypeBinding TypeTypeBinding;
		
		public ExternalTypeBinding ObjectArrayBinding;
	
		public ExternalTypeBinding VoidTypeBinding;
		
		public ExternalTypeBinding StringTypeBinding;
		
		public ExternalTypeBinding BoolTypeBinding;
		
		public ExternalTypeBinding IntTypeBinding;
		
		public ExternalTypeBinding SingleTypeBinding;
		
		public ExternalTypeBinding RuntimeServicesBinding;
		
		public ExternalTypeBinding ListTypeBinding;
		
		System.Collections.Hashtable _bindingCache = new System.Collections.Hashtable();
		
		System.Collections.Hashtable _referenceCache = new System.Collections.Hashtable();
		
		IBinding _typeOfBinding;
		
		public BindingManager()		
		{
			_typeOfBinding = new SpecialFunctionBinding(SpecialFunction.Typeof);
			
			Cache(VoidTypeBinding = new VoidTypeBindingImpl(this));
			Cache(ObjectTypeBinding = new ExternalTypeBinding(this, Types.Object));
			Cache(TypeTypeBinding = new ExternalTypeBinding(this, Types.Type));
			Cache(StringTypeBinding = new ExternalTypeBinding(this, Types.String));
			Cache(BoolTypeBinding = new ExternalTypeBinding(this, Types.Bool));
			Cache(IntTypeBinding = new ExternalTypeBinding(this, Types.Int));
			Cache(SingleTypeBinding = new ExternalTypeBinding(this, Types.Single));
			Cache(new ExternalTypeBinding(this, Types.Date));
			Cache(RuntimeServicesBinding = new ExternalTypeBinding(this, Types.RuntimeServices));
			Cache(ListTypeBinding = new ExternalTypeBinding(this, Types.List));
			Cache(ObjectArrayBinding = new ExternalTypeBinding(this, Types.ObjectArray));
			Cache(ApplicationExceptionBinding = new ExternalTypeBinding(this, Types.ApplicationException));
		}
		
		public Boo.Lang.Ast.TypeReference CreateBoundTypeReference(ITypeBinding binding)
		{
			TypeReference typeReference = new TypeReference(binding.FullName);
			Bind(typeReference, ToTypeReference(binding));
			return typeReference;
		}
		
		public static bool IsBound(Node node)
		{
			return null != node[BindingKey];
		}
		
		public static void Bind(Node node, IBinding binding)
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
		
		public void Bind(TypeDefinition type)
		{
			Bind(type, ToTypeBinding(type));
		}
		
		public static void Error(Node node)
		{
			Bind(node, ErrorBinding.Default);
		}
		
		public static IBinding GetOptionalBinding(Node node)
		{
			if (null == node)
			{
				throw new ArgumentNullException("node");
			}
			
			return (IBinding)node[BindingKey];
		}
		
		public static IBinding GetBinding(Node node)
		{
			IBinding binding = GetOptionalBinding(node);
			if (null == binding)
			{
				NodeNotBound(node);
			}
			return binding;
		}	
		
		public ITypeBinding GetBoundType(Node node)
		{
			return ((ITypedBinding)GetBinding(node)).BoundType;
		}
		
		public ITypeBinding ToTypeBinding(System.Type type)
		{
			ExternalTypeBinding binding = (ExternalTypeBinding)_bindingCache[type];
			if (null == binding)
			{
				Cache(binding = new ExternalTypeBinding(this, type));
			}
			return binding;
		}
		
		public ITypeBinding ToTypeBinding(TypeDefinition typeDefinition)
		{
			ITypeBinding binding = (ITypeBinding)_bindingCache[typeDefinition];
			if (null == binding)
			{
				Cache(typeDefinition, binding = new InternalTypeBinding(this, typeDefinition));
			}
			return binding;
		}
		
		public ITypedBinding ToTypeReference(ITypeBinding type)
		{
			ITypedBinding cached = (ITypedBinding)_referenceCache[type];
			if (null == cached)
			{
				cached = new TypeReferenceBinding(type);
				_referenceCache[type] = cached;
			}
			return cached;
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
				
				case MemberTypes.Constructor:
				{
					return new ExternalConstructorBinding(this, (System.Reflection.ConstructorInfo)mi);
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
		
		public IBinding ResolvePrimitive(string name)
		{
			IBinding binding = null;
			switch (name)
			{
				case "void":
				{
					binding = ToTypeReference(VoidTypeBinding);
					break;
				}
				
				case "bool":
				{
					binding = ToTypeReference(BoolTypeBinding);
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
				
				case "object":
				{
					binding = ToTypeReference(ObjectTypeBinding);
					break;
				}
				
				case "int":
				{
					binding = ToTypeReference(IntTypeBinding);
					break;
				}
				
				case "typeof":
				{
					binding = _typeOfBinding;
					break;
				}
			}
			return binding;
		}
		
		void Cache(ExternalTypeBinding binding)
		{
			_bindingCache[binding.Type] = binding;
		}
		
		void Cache(object key, ITypeBinding binding)
		{
			_bindingCache[key] = binding;
		}
		
		public static string GetSignature(IMethodBinding binding)
		{			
			System.Text.StringBuilder sb = new System.Text.StringBuilder(binding.DeclaringType.FullName);
			sb.Append(".");
			sb.Append(binding.Name);
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
			sb.Append(binding.ReturnType.FullName);
			return sb.ToString();
		}
		
		private static void NodeNotBound(Node node)
		{
			throw CompilerErrorFactory.NodeNotBound(node);
		}
		
		static object BindingKey = new object();		
		
		#region VoidTypeBindingImpl
		class VoidTypeBindingImpl : ExternalTypeBinding
		{			
			internal VoidTypeBindingImpl(BindingManager manager) : base(manager, Types.Void)
			{				
			}		
			
			public override IBinding Resolve(string name)
			{	
				return null;
			}	
		}

		#endregion
	}
}
