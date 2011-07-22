namespace Statsia.Random

import System.Collections.Generic

interface IRandomVariable[of T]:
	def Sample() as T
	Simulations as IList[of T]:
		get
