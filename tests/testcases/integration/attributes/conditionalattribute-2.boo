"""
genericool
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

#this method will not be emitted thus we'll print nothing
ConditionalClass.PrintNoT[of int](42)
#this method will be emitted thus we'll print something
ConditionalClass.PrintSomeT[of string]("genericool")