namespace Boo.Lang.Compiler.TypeSystem
{
	using System.Reflection;
	
	public class ExternalParameter : IParameter
	{
		TypeSystemServices _typeSystemServices;
		ParameterInfo _parameter;
		
		public ExternalParameter(TypeSystemServices service, ParameterInfo parameter)
		{
			_typeSystemServices = service;
			_parameter = parameter;
		}
		
		public string Name
		{
			get
			{
				return _parameter.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _parameter.Name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Parameter;
			}
		}
		
		public IType Type
		{
			get
			{
				return _typeSystemServices.Map(_parameter.ParameterType);
			}
		}
	}
}
