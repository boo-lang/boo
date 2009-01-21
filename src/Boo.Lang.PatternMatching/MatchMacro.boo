namespace Boo.Lang.PatternMatching

import Boo.Lang.PatternMatching.Impl

# TODO: check for unreacheable patterns
macro match:
"""
Pattern matching facility:

	match <expression>:
		case <Pattern1>:
			<block1>
			.
			.
			.
		case <PatternN>:
			<blockN>
		otherwise:
			<blockOtherwise>

The following patterns are supported:
    
    Type() -- type test pattern
    Type(Property1: Pattern1, ...) -- object pattern
    Pattern1 | Pattern2 -- either pattern
	Pattern1 & Pattern2 -- both pattern (NOT IMPLEMENTED)
    Pattern1 and condition -- constrained pattern  (NOT IMPLEMENTED)
    Pattern1 or condition -- constrained pattern  (NOT IMPLEMENTED)
    (Pattern1, Pattern2) -- fixed size iteration pattern
    [Pattern1, Pattern2] -- arbitrary size iteration pattern (NOT IMPLEMENTED)
    x = Pattern -- variable binding
    x -- variable binding
    BinaryOperatorType.Assign -- constant pattern
    42 -- constant pattern
    "42" -- constant pattern
    null -- null test pattern
    
If no pattern matches MatchError is raised.
"""
	macro case:
		caseListFor(match).Add(case)
		
	macro otherwise:
		assert match["otherwise"] is null
		match["otherwise"] = otherwise
		
	assert 0 == len(match.Block.Statements)
	return MatchExpansion(Context, match).value


