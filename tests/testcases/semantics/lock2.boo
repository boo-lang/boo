"""
[System.Reflection.DefaultMemberAttribute('Item')]
public class LockedList(object):

	protected _list as Boo.Lang.List

	public Item[index as int] as object:
		public get:
			\$lock\$monitor\$1 = self._list
			System.Threading.Monitor.Enter(\$lock\$monitor\$1)
			try:
				return self._list.get_Item(index)
			ensure:
				System.Threading.Monitor.Exit(\$lock\$monitor\$1)

	public def constructor():
		super()
		self._list = []
"""
class LockedList:
	
	_list = []
	
	self[index as int]:
		[lock(_list)]
		get:
			return _list[index]
