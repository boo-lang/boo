"""
D
"""
/*
class B:
	pass
    
class D(B):
	pass

class C[of T]:
	
	public items as List

	def Select[of U(T)]() as U*:
		for item in items:
			yield item if item isa U
			
for d in C[of B](items: [B(), D()]).Select[of D]():
	print d
*/

import Boo.Lang
import System
import System.Collections
import System.Collections.Generic
import System.Runtime.CompilerServices

public class B(object):

	public def constructor():
		super()

public class D(B):

	public def constructor():
		super()

public class C[of T](object):

	public items as Boo.Lang.List

	public def Select[of U(T)]() as System.Collections.Generic.IEnumerable[of U]:
		return _Select_Enumerable_4[of U](self)

	public def constructor():
		super()

	internal final class _Select_Enumerable_4[of U(T)](Boo.Lang.GenericGenerator[of U]):

		public virtual def GetEnumerator() as System.Collections.Generic.IEnumerator[of U]:
			return _Enumerator(self._self__8)

		public def constructor(self_ as C[of T]):
			super()
			self._self__8 = self_

		internal final class _Enumerator(Boo.Lang.GenericGeneratorEnumerator[of U], System.Collections.IEnumerator):

			public def constructor(self_ as C[of T]):
				super()
				self._self__7 = self_

			public virtual def MoveNext() as bool:
				try:
					__switch__(self._state, _state_0, _state_1, _state_1, _state_3)
					:_state_0
					self.__iterator_2_6 = self._self__7.items.GetEnumerator()
					self._state = 2
					while self.__iterator_2_6.MoveNext():
						self._item_5 = self.__iterator_2_6.get_Current()
						if self._item_5 isa U:
							return self.Yield(3, self._item_5)
							:_state_3
					self._state = 1
					self._ensure2()
					self.YieldDefault(1)
					:_state_1
				failure:
					self.Dispose()

			internal _item_5 as object

			internal __iterator_2_6 as System.Collections.Generic.IEnumerator[of object]

			internal _self__7 as C[of T]

			private def _ensure2() as void:
				(self.__iterator_2_6 cast System.IDisposable).Dispose()

			public virtual def Dispose() as void:
				__switch__(self._state, noEnsure, noEnsure, _ensure_2, _ensure_2)
				:noEnsure
				self._state = 1
				return
				:_ensure_2
				self._state = 1
				self._ensure2()
				return

		internal _self__8 as C[of T]

_iterator_3 = @((_1 = C[of B]()), (_1.items = [B(), D()]), _1).Select[of D]().GetEnumerator()
try:
	while _iterator_3.MoveNext():
		d = _iterator_3.get_Current()
		System.Console.WriteLine(d)
ensure:
	(_iterator_3 cast System.IDisposable).Dispose()