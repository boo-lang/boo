
namespace Statsia.Random

import System
import System.Collections.Generic
import MathNet.Numerics
import Statsia.Math

public class RandomVariable[of T](IRandomVariable[of T]):
	
	_sims = List[of T]()
	def constructor():
		pass

	def constructor(values as IEnumerable[of T]):
		for value in values:
			_sims.Add(value)
		
	Simulations as IList[of T]: 
		get:
			return _sims.AsReadOnly()
	
	def Sample():
		return _sims.Sample()
		
	def ToString():
		n = System.Math.Min(5, _sims.Count)
		repr = "RandomVariable: "
		for i in range(n):
			repr += _sims[i].ToString() + " "
		repr += "..."
		return repr
		