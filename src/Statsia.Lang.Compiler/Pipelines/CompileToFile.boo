namespace Statsia.Lang.Compiler.Pipelines

import System
import Statsia.Lang.Compiler.Steps

class CompileToFile(Boo.Lang.Compiler.Pipelines.CompileToFile):
    def constructor():
        super()
        Replace(Boo.Lang.Compiler.Steps.ProcessMethodBodiesWithDuckTyping, ProcessMethodBodies() )