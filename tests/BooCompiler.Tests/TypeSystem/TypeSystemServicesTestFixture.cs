using System;
using System.Collections.Generic;
using NUnit.Framework;
using Boo.Lang.Compiler.TypeSystem;

namespace BooCompiler.Tests.TypeSystem
{
	[TestFixture]
	public class TypeSystemServicesTestFixture
	{
		class BeanAwareTypeSystemServices : TypeSystemServices
		{
			class BeanAwareType : ExternalType
			{
				public BeanAwareType(BeanAwareTypeSystemServices services, Type type) : base(services, type)
				{
				}

				protected override IEntity[] CreateMembers()
				{
					IEntity[] originalMembers = base.CreateMembers();
					IEntity[] beanProperties = new BeanPropertyFinder(originalMembers).FindAll();
					return (IEntity[]) Boo.Lang.Runtime.RuntimeServices.AddArrays(typeof(IEntity), originalMembers, beanProperties);
				}
			}

			protected override IType CreateEntityForRegularType(System.Type type)
			{
				return new BeanAwareType(this, type);
			}
		}

		class Bean
		{
			public string getName() { return null; }
			public void setName(string value) {  }
			public void settle() { }

		}

		[Test]
		public void TypeCreationCanBeOverriden()
		{
			IEntity[] members = new BeanAwareTypeSystemServices().Map(typeof(Bean)).GetMembers();
			Assert.AreEqual(5, members.Length);
			Array.Sort(members, delegate(IEntity l, IEntity r) { return l.Name.CompareTo(r.Name);  });

			Assert.AreEqual("constructor", members[0].Name);
			Assert.AreEqual("getName", members[1].Name);

			IProperty beanProperty = (IProperty) members[2];
			Assert.AreEqual("name", beanProperty.Name);
			Assert.AreSame(members[1], beanProperty.GetGetMethod());
			Assert.AreSame(members[3], beanProperty.GetSetMethod());

			Assert.AreEqual("setName", members[3].Name);
			Assert.AreEqual("settle", members[4].Name);
		}
	}
}