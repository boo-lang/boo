"""
readonlyspan-write.boo(7,5): BCE0053: Property 'System.ReadOnlySpan[of char].Item' is read only.
"""
import System

span = "Hi".AsSpan()
span[0] = char('X')
