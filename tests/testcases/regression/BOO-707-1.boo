"""
foo called
"""
import System
import System.IO
import System.Reflection
import System.Reflection.Emit
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

def createUIntPtrDelegateAssembly():
	name = AssemblyName(Name: "UIntPtrDelegateAssembly")
	assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save, Path.GetTempPath())
	module = assembly.DefineDynamicModule("UIntPtrDelegateAssembly.dll")
	
	t = module.DefineType("UIntPtrDelegate", TypeAttributes.Public|TypeAttributes.Sealed, MulticastDelegate)
	ctor = t.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, (object, System.UIntPtr))
	ctor.SetImplementationFlags(MethodImplAttributes.Runtime|MethodImplAttributes.Managed)
	
	invoke = t.DefineMethod("Invoke", MethodAttributes.Public, CallingConventions.HasThis, void, array(Type, 0))
	invoke.SetImplementationFlags(MethodImplAttributes.Runtime|MethodImplAttributes.Managed)
	
	t.CreateType()
	assembly.Save("UIntPtrDelegateAssembly.dll")
	return Assembly.LoadFrom(module.FullyQualifiedName)

code = """
def foo():
	print 'foo called'
	
d as UIntPtrDelegate = foo
d()
"""	

compiler = BooCompiler()
compiler.Parameters.References.Add(createUIntPtrDelegateAssembly())
compiler.Parameters.Input.Add(StringInput("test", code))
compiler.Parameters.Pipeline = Pipelines.CompileToMemory()
compiler.Parameters.GenerateInMemory = true
result = compiler.Run()
assert 0 == len(result.Errors), result.Errors.ToString(false)

result.GeneratedAssembly.EntryPoint.Invoke(null, (array(string,0),))

