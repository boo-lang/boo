"""
pass
"""
import System
import System.Collections.Generic

static class Foo:
	
    def Bar[of TResult](method as Func[of List[of TResult]]) as TResult:
        return {return method()[0]}()

var list = List[of string](){'pass'}
print Foo.Bar[of string]({return list})