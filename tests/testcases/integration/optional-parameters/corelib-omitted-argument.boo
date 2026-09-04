"""
ok
"""
import System

# Calling a real BCL method by leaving its optional parameter out. ThrowIfNull
# has no one-argument overload, so nothing but a filled default can resolve it.
# This test tells us whether Boo can handle call targets with optional params.
o = "x"
ArgumentNullException.ThrowIfNull(o)
Console.WriteLine("ok")
