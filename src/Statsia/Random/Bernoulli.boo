
namespace Statsia.Random

import System.Collections.Generic
import Statsia.Math

class Bernoulli(IRandomVariable[of int]):
	
	_sims = List[of int]()
	
	def constructor(p as double):
		for i in range(Statsia.Control.Simulations):
			_sims.Add(MathNet.Numerics.Distributions.Bernoulli.Sample(Statsia.Control.RandomSource, p))
	
	def constructor(p as IRandomVariable[of double]):
		for i in range(p.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Bernoulli.Sample(Statsia.Control.RandomSource, p.Simulations[i]))
		
	def Sample():
		return _sims.Sample()
		
	Simulations:
		get:
			return _sims.AsReadOnly()
