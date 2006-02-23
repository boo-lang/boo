"""
namespace TypeResolution

import TypeResolution

public class Item(System.Object):

	public def constructor():
		super()

[System.Reflection.DefaultMemberAttribute('Item')]
public class Collection(System.Object):

	public Item[index as System.Int32] as TypeResolution.Item:
		public get:
			pass

	public def constructor():
		super()
"""
namespace TypeResolution

class Item:
	def constructor():
		pass
	
class Collection:	
	Item[index as int] as Item:
		get:
			pass
