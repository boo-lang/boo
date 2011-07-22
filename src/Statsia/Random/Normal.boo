
namespace Statsia.Random
import System
import System.Collections.Generic
import Statsia.Math

class Normal(IRandomVariable[of double]):
	
	_sims = List[of double]()
	
	def constructor():
		self(0, 1)
		
	def constructor(mean as double, sd as double):
		for i in range(Statsia.Control.Simulations):
			_sims.Add(MathNet.Numerics.Distributions.Normal.Sample(Statsia.Control.RandomSource, mean, sd))

	def constructor(mean as IRandomVariable[of double], sd as double):
		for i in range(mean.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Normal.Sample(Statsia.Control.RandomSource, mean.Simulations[i], sd))

	def constructor(mean as double, sd as IRandomVariable[of double]):
		for i in range(sd.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Normal.Sample(Statsia.Control.RandomSource, mean, sd.Simulations[i]))

	def constructor(mean as IRandomVariable[of double], sd as IRandomVariable[of double]):
		if mean.Simulations.Count != sd.Simulations.Count:
			raise ArgumentException("Random parameters must have the same number of simulations.")			
		for i in range(mean.Simulations.Count):
			_sims.Add(MathNet.Numerics.Distributions.Normal.Sample(Statsia.Control.RandomSource, mean.Simulations[i], sd.Simulations[i]))
																								
	def Sample():
		return _sims.Sample()
		
	Simulations:
		get:
			return _sims.AsReadOnly()
