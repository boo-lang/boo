"""
public class LockedList(System.Object):

	protected _list as Boo.Lang.List

	public Item(index as System.Int32) as System.Object:
		public get:
			__monitor1__ = self._list
			System.Threading.Monitor.Enter(__monitor1__)
			try:
				return self._list.get_Item(index)
			ensure:
				System.Threading.Monitor.Exit(__monitor1__)

	public def constructor():
		super()
		self._list = []
"""
class LockedList:
	
	_list = []
	
	Item(index as int):
		[lock(_list)]
		get:
			return _list[index]
