"""
True
"""
namespace kri.frame

struct DirtyHolder[of T]:
	[getter(Dirty)]
	private dirty as bool
	private val	as T
	def constructor(t as T):
		Value = t
	Value as T:
		get: return val
		set: dirty, val = true,value
	def clean() as void:
		dirty = false

public class Unit:
	public dTex	= DirtyHolder[of Texture](null)
	
class Texture:
	pass
	
print Unit().dTex.Dirty
