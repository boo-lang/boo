"""
namespace TypeResolution

public class Item(System.Object):

	public def constructor():
		super()

public class Collection(System.Object):

	public Item(index as System.Int32) as TypeResolution.Item:
		public get:
			pass

	public def constructor():
		super()
"""
namespace TypeResolution

class Item:
	pass
	
class Collection:
	Item(index as int) as Item:
		get:
			pass
