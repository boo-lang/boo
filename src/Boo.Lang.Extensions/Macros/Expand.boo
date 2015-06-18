namespace Boo.Lang.Extensions
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

def GetCode(filename as string) as string:
   filename = Path.GetFullPath(filename)
   code = File.ReadAllText(filename)
   parser = ExpandMacros()
   params = CompilerParameters()
   params.Input.Add(FileInput(code))
   context = CompilerContext(params)
   parser.Run(context)
   return context.CompileUnit.Modules.First.ToCodeString()   

macro expand:
   macro filename:
      expand['filename'] = filename.Arguments[0].ToString()
   
   macro to:
      expand['to'] = to.Arguments[0].ToString()
      
   code as string
   if expand['filename'] is null:
      code = expand.Body.ToCodeString()
   else:
      code = GetCode(expand['filename'])

   if expand['to'] is null:     
      return ExpressionStatement([| System.Console.WriteLine($code) |])
   else:
      filename = Path.GetFullPath(expand['to'].ToString())
      File.WriteAllText(filename, code)