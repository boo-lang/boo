using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.NameBinding
{
	public class InternalTypeInfo : ITypeInfo
	{		
		TypeManager _manager;
		TypeDefinition _typeDefinition;
		TypeBuilder _builder;
		
		internal InternalTypeInfo(TypeManager manager, TypeDefinition typeDefinition, TypeBuilder builder)
		{
			_manager = manager;
			_typeDefinition = typeDefinition;
			_builder = builder;
		}
		
		public NameInfoType InfoType
		{
			get
			{
				return NameInfoType.Type;
			}
		}
		
		public TypeDefinition TypeDefinition
		{
			get
			{
				return _typeDefinition;
			}
		}
		
		public Type Type
		{
			get
			{
				return _builder;
			}
		}
		
		public TypeBuilder TypeBuilder
		{
			get
			{
				return _builder;
			}
		}
	}
}
