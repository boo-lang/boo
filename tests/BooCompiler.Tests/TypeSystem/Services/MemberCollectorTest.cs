using System;
using System.Linq;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class MemberCollectorTest
	{
		class Foo : object
		{	
			public Foo() {}

			public virtual string Name { get; set; }

			public override string ToString() { return "Foo";  }
		}

		class Bar : Foo
		{
			public override string Name
			{
				get { return base.Name; }
				set { base.Name = value; }
			}
		}

		[Test]
		public void GetAllMembers()
		{
			new CompilerContext().Run(ctx =>
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
	}
}