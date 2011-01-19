using System;
using Boo.Lang;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Reflection
{
	[TestFixture]
	public class EntityFormatterTest : EntityFormatterTestBase
	{
		protected override IType SimpleType()
		{
			return Map(typeof(object));
		}

		protected override IType ArrayType()
		{
			return Map(typeof(object[]));
		}

		protected override IType CallableType()
		{
			return Map(typeof(System.Action));
		}

		protected override IType GenericType()
		{
			return Map(typeof(List<>));
		}

		private IType Map(Type type)
		{
			return _subject.Map(type);
		}

		private readonly IReflectionTypeSystemProvider _subject = new ReflectionTypeSystemProvider();
	}
}
