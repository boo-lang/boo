namespace Boo.Lang.Compiler.TypeSystem
{
	using System.Reflection;
	
	public class ExternalParameter : IParameter
	{
		TypeSystemServices _tagService;
		ParameterInfo _parameter;
		
		public ExternalParameter(TypeSystemServices service, ParameterInfo parameter)
		{
			_tagService = service;
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
		
		public ElementType ElementType
		{
			get
			{
				return ElementType.Parameter;
			}
		}
		
		public IType Type
		{
			get
			{
				return _tagService.Map(_parameter.ParameterType);
			}
		}
	}
}
