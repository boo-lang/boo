namespace BooCompiler.Tests.TypeSystem.Services

import System
import System.Linq
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class MemberCollectorTest:
	class Foo(object):
		protected def constructor():
			pass

		public virtual Name as string:
			get: return _name
			set: _name = value

		private _name as string

		override def ToString() as string:
			return "Foo"

	class Bar(Foo):
		public override Name as string:
			get: return super.Name
			set: super.Name = value

	[Test]
	def GetAllMembers():
		RunInCompilerContextEnvironment() do:
			barName = typeof(Bar).FullName.Replace(char('+'), char('.'))
			fooName = typeof(Foo).FullName.Replace(char('+'), char('.'))
			members = MemberCollector.CollectAllMembers(My[of TypeSystemServices].Instance.Map(typeof(Bar)))
			expected = (
				barName + ".constructor",
				barName + ".get_Name",
				barName + ".Name",
				barName + ".set_Name",
				fooName + ".ToString",
				"object.Equals",
				"object.Equals", # static
				"object.GetHashCode",
				"object.GetType",
				"object.ReferenceEquals",
			)
			actual = members.OfType[of IAccessibleMember]().Where({ m as IAccessibleMember | m.IsPublic }).Select({ m as IAccessibleMember | m.FullName }).ToArray()
			Array.Sort(actual)
			Assert.AreEqual(expected, actual)

	private def RunInCompilerContextEnvironment(action as Action):
		ActiveEnvironment.With(CompilerContext().Environment):
			action()
