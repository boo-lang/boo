using System;
using System.Linq;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class MemberCollectorTest
	{
		class Foo : object
		{
			protected Foo() {}

			public virtual string Name { get; set; }

			public override string ToString() { return "Foo";  }
		}

		class Bar : Foo
		{
// ReSharper disable RedundantOverridenMember
			public override string Name
			{
				get { return base.Name; }
				set { base.Name = value; }
			}
// ReSharper restore RedundantOverridenMember
		}

		[Test]
		public void GetAllMembers()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var barName = typeof(Bar).FullName.Replace('+', '.');
				var fooName = typeof(Foo).FullName.Replace('+', '.');
				var members = MemberCollector.CollectAllMembers(My<TypeSystemServices>.Instance.Map(typeof(Bar)));
				var expected = new[]
				               	{
									barName + ".constructor",
									barName + ".get_Name",
									barName + ".Name",
									barName + ".set_Name",
									fooName + ".ToString",
                                    "object.Equals",
                                    "object.Equals", // static
                                    "object.GetHashCode",
                                    "object.GetType",                                        
                                    "object.ReferenceEquals",
				               	};
				var actual = members.OfType<IAccessibleMember>().Where(m => m.IsPublic).Select(m => m.FullName).ToArray();
				Array.Sort(actual);
				Assert.AreEqual(expected, actual);
			});
		}

		private void RunInCompilerContextEnvironment(Action action)
		{
			ActiveEnvironment.With(new CompilerContext().Environment, () => {
				action();
			});
		}
	}
}
