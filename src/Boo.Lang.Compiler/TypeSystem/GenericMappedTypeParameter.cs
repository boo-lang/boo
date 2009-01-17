using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	class GenericMappedTypeParameter : AbstractGenericParameter
	{
		IGenericParameter _source;
		GenericMapping _mapping;

		public GenericMappedTypeParameter(TypeSystemServices tss, IGenericParameter source, GenericMapping mapping) : base(tss)
		{
			_source = source;
			_mapping = mapping;
		}

		public IGenericParameter Source
		{
			get { return _source; }
		}

		public GenericMapping GenericMapping
		{
			get { return _mapping; }
		}

		public override int GenericParameterPosition
		{
			get { return Source.GenericParameterPosition; }
		}

		public override IType[] GetTypeConstraints()
		{
			return Array.ConvertAll<IType, IType>(Source.GetTypeConstraints(), _mapping.MapType);
		}

		public override IEntity DeclaringEntity
		{
			get { return _mapping.Map(Source.DeclaringEntity); }
		}

		public override string Name
		{
			get { return Source.Name; }
		}

		public override bool MustHaveDefaultConstructor
		{
			get { return Source.MustHaveDefaultConstructor; }
		}

		public override Variance Variance
		{
			get { return Source.Variance; }
		}

		public override bool IsClass
		{
			get { return Source.IsClass; }
		}

		public override bool IsValueType
		{
			get { return Source.IsValueType; }
		}
	}
}
