"""
1
"""
import System.Collections.Generic

struct Batch:
	class CompMat(IComparer[of Batch]):
		def Compare(a as Batch, b as Batch) as int:
			return 1
			
	public static cMat = CompMat()
		
	public dummy as bool
		
print Batch.cMat.Compare(Batch(), Batch())
