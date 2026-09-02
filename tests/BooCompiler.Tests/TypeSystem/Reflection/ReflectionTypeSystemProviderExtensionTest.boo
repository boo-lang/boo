namespace BooCompiler.Tests.TypeSystem.Reflection

import System
import System.Collections.Generic
import Boo.Lang.Compiler.TypeSystem.Reflection
import NUnit.Framework
import Boo.Lang.Compiler.TypeSystem

[TestFixture]
class ReflectionTypeSystemProviderExtensionTest:
	class BeanAwareTypeSystemProvider(ReflectionTypeSystemProvider):
		class BeanAwareType(ExternalType):
			def constructor(services as IReflectionTypeSystemProvider, type as Type):
				super(services, type)

			protected override def CreateMembers() as (IEntity):
				originalMembers as (IEntity) = super.CreateMembers()
				beanProperties as (IEntity) = BeanPropertyFinder(originalMembers).FindAll()
				return Boo.Lang.Runtime.RuntimeServices.AddArrays(typeof(IEntity), originalMembers, beanProperties)

		public override def CreateEntityForRegularType(type as Type) as IType:
			return BeanAwareType(self, type)

	class Bean:
		public def getName() as string:
			return null

		public def setName(value as string):
			pass

		public def settle():
			pass

	[Test]
	def TypeCreationCanBeOverriden():
		members = List[of IEntity](BeanAwareTypeSystemProvider().Map(typeof(Bean)).GetMembers()).ToArray()
		Assert.AreEqual(5, members.Length)
		Array.Sort(members, { l as IEntity, r as IEntity | l.Name.CompareTo(r.Name) })

		Assert.AreEqual("constructor", members[0].Name)
		Assert.AreEqual("getName", members[1].Name)

		beanProperty = members[2] as IProperty
		Assert.AreEqual("name", beanProperty.Name)
		Assert.AreSame(members[1], beanProperty.GetGetMethod())
		Assert.AreSame(members[3], beanProperty.GetSetMethod())

		Assert.AreEqual("setName", members[3].Name)
		Assert.AreEqual("settle", members[4].Name)
