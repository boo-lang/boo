"""
[System.Reflection.DefaultMemberAttribute('Item')]
public class LockedList(object):

	protected _list as Boo.Lang.List

	public Item[index as int] as object:
		public get:
			__monitor1__ = self._list
			System.Threading.Monitor.Enter(__monitor1__)
			try:
				return self._list.get_Item(index)
			ensure:
				System.Threading.Monitor.Exit(__monitor1__)

	public def constructor():
		super()
		self.$initializer$()

	def $initializer$() as void:
		self._list = []
"""
class LockedList:
	
	_list = []
	
	self[index as int]:
		[lock(_list)]
		get:
			return _list[index]
