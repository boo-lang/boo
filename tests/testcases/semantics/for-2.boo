"""
import System.Collections

public class Foo(object, System.Collections.IEnumerable):

	private virtual def System.Collections.IEnumerable.GetEnumerator() as System.Collections.IEnumerator:
		return InternalEnumerator()

	public class InternalEnumerator(object, System.Collections.IEnumerator):

		protected i as int

		private virtual def System.Collections.IEnumerator.MoveNext() as bool:
			self.i = (self.i + 1)
			if self.i == 1:
				return true

		private virtual System.Collections.IEnumerator.Current as object:
			private virtual get:
				return 'foo'

		private virtual def System.Collections.IEnumerator.Reset() as void:
			pass

		public def constructor():
			super()

	public def constructor():
		super()

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class For_2Module(object):

	private static def Main(argv as (string)) as void:
		\$iterator\$1 = Foo().GetEnumerator()
		while \$iterator\$1.MoveNext():
			i = \$iterator\$1.get_Current()
			System.Console.WriteLine(i)

	private def constructor():
		super()
"""
import System.Collections

class Foo (IEnumerable):
	def IEnumerable.GetEnumerator() as IEnumerator:
		return InternalEnumerator()

	class InternalEnumerator (IEnumerator):
		i = 0

		def IEnumerator.MoveNext():
			i++
			return true if i == 1

		IEnumerator.Current as object:
			get:
				return "foo"

		def IEnumerator.Reset():
			pass


for i in Foo(): #NO DISPOSE IS REQUIRED
	print i

