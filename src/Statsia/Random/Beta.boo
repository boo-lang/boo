namespace Statsia.Random

import System
import System.Collections.Generic
import Statsia.Math

class Beta(IRandomVariable[of double]):
	
	_sims = List[of double]()
	
	def constructor(a as double, b as double):
		for i in range(Statsia.Control.Simulations):
			_sims.Add(MathNet.Numerics.Distributions.Beta.Sample(Statsia.Control.RandomSource, a, b))

	def constructor(a as IRandomVariable[of double], b as double):
		for i in range(a.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Beta.Sample(Statsia.Control.RandomSource, a.Simulations[i], b))

	def constructor(a as double, b as IRandomVariable[of double]):
		for i in range(b.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Beta.Sample(Statsia.Control.RandomSource, a, b.Simulations[i]))

	def constructor(a as IRandomVariable[of double], b as IRandomVariable[of double]):
		if a.Simulations.Count != b.Simulations.Count:
			raise ArgumentException("Random parameters must have the same number of simulations.")	
		for i in range(b.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Beta.Sample(Statsia.Control.RandomSource, a.Simulations[i], b.Simulations[i]))
										
	def Sample():
		return _sims.Sample()
	
	static def LogPdf(x as double, a as double, b as double):
		return 1.0
	
	Simulations:
		get:
			return _sims.AsReadOnly()
