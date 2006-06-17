namespace Boo.Lang.Compiler.TypeSystem
{
	using System;

#if NET_2_0
	public class ExternalGenericTypeDefinition : ExternalType, IGenericTypeDefinition
	{
		private IGenericParameter[] _parameters;

		public ExternalGenericTypeDefinition(TypeSystemServices tss, Type type) : base(tss, type)
		{	
		}

		public IGenericParameter[] GetGenericParameters()
		{
			if (null == _parameters) _parameters = CreateParameters();
			return _parameters;
		}

		public IType MakeGenericType(IType[] arguments)
		{
			Type[] externalTypes = new Type[arguments.Length];
			for (int i = 0; i < arguments.Length; ++i)
			{
				ExternalType externalType = arguments[i] as ExternalType;
				if (null == externalType) throw new NotImplementedException("only generics for externally defined types for now");
				externalTypes[i] = externalType.ActualType;
			}
			return _typeSystemServices.Map(ActualType.MakeGenericType(externalTypes));
		}

		protected override string BuildFullName()
		{
			string name = ActualType.FullName;
			return name.Substring(0, name.IndexOf('`'));
		}

		private IGenericParameter[] CreateParameters()
		{
			Type[] arguments = this.ActualType.GetGenericArguments();
			IGenericParameter[] parameters = new IGenericParameter[arguments.Length];
			for (int i=0; i<arguments.Length; ++i)
			{
				parameters[i] = new ExternalGenericParameter(arguments[i]);
			}
			return parameters;
		}

		class ExternalGenericParameter : IGenericParameter
		{
			Type _type;
			
			public ExternalGenericParameter(Type type)
			{	
				_type = type;
			}
			
			public string Name
			{
				get { return _type.Name; }
			}

			public string FullName
			{
				get { return _type.FullName; }
			}

			public EntityType EntityType
			{
				get { return TypeSystem.EntityType.GenericParameter; }
			}
		}
	}
#else
	public class ExternalGenericTypeDefinition : ExternalType, IGenericTypeDefinition
	{
		public ExternalGenericTypeDefinition(TypeSystemServices tss, Type type) : base(tss, type)
		{
		}

		public IGenericParameter[] GetGenericParameters()
		{
			throw new NotImplementedException();
		}

		public IType MakeGenericType(IType[] arguments)
		{
			throw new NotImplementedException();
		}
	}
#endif
}
