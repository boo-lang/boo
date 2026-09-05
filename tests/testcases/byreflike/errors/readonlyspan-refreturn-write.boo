"""
readonlyspan-refreturn-write.boo(7,26): BCE0049: Expression 'span.GetPinnableReference()' cannot be assigned to.
"""
import System

span = "Hi".AsSpan()
span.GetPinnableReference() = char('X')
