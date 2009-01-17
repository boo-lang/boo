using System;
using System.Collections.Generic;
using System.Text;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class MappedParameter : IParameter
	{
		private IType _type;
		private IParameter _baseParameter;

		public MappedParameter(IParameter baseParameter, IType type)
		{
			_baseParameter = baseParameter;
			_type = type;
		}

		public bool IsByRef
		{
			get { return _baseParameter.IsByRef; }
		}

		public IType Type
		{
			get { return _type; }
		}

		public string Name
		{
			get { return _baseParameter.Name; }
		}

		public string FullName
		{
			get { return _baseParameter.FullName; }
		}

		public EntityType EntityType
		{
			get { return EntityType.Parameter; }
		}
	}
}
