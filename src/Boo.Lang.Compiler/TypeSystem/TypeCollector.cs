using System;
using Boo.Lang;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class TypeCollector : TypeVisitor
	{
		Predicate<IType> _predicate;
		List<IType> _matches = new List<IType>();

		public TypeCollector(Predicate<IType> predicate)
		{
			_predicate = predicate;
		}

		public IEnumerable<IType> Matches
		{
			get { return _matches; }
		}

		public override void Visit(IType type)
		{
			if (_predicate(type)) _matches.AddUnique(type);
			
			base.Visit(type);
		}
	}
}
