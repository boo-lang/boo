using System;
using System.Reflection;

namespace Boo.Ast.Compilation.Binding
{
	public class ExternalMethodBinding : IMethodBinding
	{
		BindingManager _manager;
		
		MethodBase _mi;
		
		internal ExternalMethodBinding(BindingManager manager, MethodBase mi)
		{
			_manager = manager;
			_mi = mi;
		}
		
		public virtual BindingType BindingType
		{
			get
			{
				return BindingType.Method;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _mi.GetParameters().Length;
			}
		}
		
		public Type GetParameterType(int parameterIndex)
		{
			return _mi.GetParameters()[parameterIndex].ParameterType;
		}
		
		public ITypeBinding ReturnType
		{
			get
			{
				return _manager.ToTypeBinding(((MethodInfo)_mi).ReturnType);
			}
		}
		
		public MethodBase MethodInfo
		{
			get
			{
				return _mi;
			}
		}
		
		public override string ToString()
		{
			return BindingManager.GetSignature(this);
		}
	}
	
	public class ExternalConstructorBinding : ExternalMethodBinding, IConstructorBinding
	{
		public ExternalConstructorBinding(BindingManager manager, ConstructorInfo ci) : base(manager, ci)
		{			
		}
		
		public override BindingType BindingType
		{
			get
			{
				return BindingType.Constructor;
			}
		}
		
		public ConstructorInfo ConstructorInfo
		{
			get
			{
				return (ConstructorInfo)MethodInfo;
			}
		}
	}
}
