"""
BCE0120-6.boo(9,32): BCE0120: 'BooSupportingClasses.Setters.set_ProtectedSet' is inaccessible due to its protection level.
BCE0120-6.boo(12,32): BCE0120: 'BooSupportingClasses.Setters.set_MacroProtectedSet' is inaccessible due to its protection level.
"""
import BooSupportingClasses

#external type
print BooSupportingClasses.Setters().ProtectedSet
BooSupportingClasses.Setters().ProtectedSet &= true #ERR

print BooSupportingClasses.Setters().MacroProtectedSet
BooSupportingClasses.Setters().MacroProtectedSet &= true #ERR

