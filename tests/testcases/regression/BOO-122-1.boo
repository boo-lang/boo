"""
Level 1 cleared.
Level 2 cleared.
Level 3 cleared.
bleh
Level 4 cleared.
"""
import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

def run(src as string, references):
	compiler = BooCompiler()
	compiler.Parameters.Input.Add(StringInput("level${len(references)+1}", src))
	compiler.Parameters.References.Extend(references)
	compiler.Parameters.Pipeline = Run()
	result = compiler.Run()
	Assert.Fail(result.Errors.ToString()) if len(result.Errors)
	return result.GeneratedAssembly

level1 = """
class Level1:
        static def MethodFromLevel1():
                print("bleh")
print("Level 1 cleared.")
"""

level2 = """
class Level2(Level1):
        pass
print("Level 2 cleared.")
"""

level3 = """
class Level3(Level2):
        pass
print("Level 3 cleared.")
"""

level4 = """
class TestStatic(Level3):
        def RunForestRun():
                MethodFromLevel1()

t = TestStatic()
t.RunForestRun()
print("Level 4 cleared.")
"""

references = []
for level in level1, level2, level3, level4:
	references.Add(run(level, references))

