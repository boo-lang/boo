"""
worked
"""
interface ICondition:
    def RenameThisHasWornOut() as bool
 
interface IAffectsStrength:
    def DetermineStrength(currentStrength as int) as int
 
interface IAffectsDexterity:
    def DetermineDexterity(currentDexterity as int) as int
 
interface IAffectsStrengthCondition(ICondition, IAffectsStrength):
    pass
 
interface IAffectsDexterityCondition(ICondition, IAffectsDexterity):
    pass
 
class ExhaustedCondition(IAffectsStrengthCondition, IAffectsDexterityCondition):
 
    public def RenameThisHasWornOut() as bool:
        return true
 
    public def DetermineStrength(currentStrength as int) as int:
        return currentStrength - 6
 
    public def DetermineDexterity(currentDexterity as int) as int:
        return currentDexterity - 6

    public def Run():
        print "worked"


ExhaustedCondition().Run()
