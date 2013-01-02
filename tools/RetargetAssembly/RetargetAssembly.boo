# rewrites all system assembly references to match wp8 assemblies:
#   Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
# and change the assembly runtime to net-4.0

import Mono.Cecil
import Useful.CommandLine
import System.IO
import System.Reflection(StrongNameKeyPair)

def Main(args as (string)):
  options = Options(args)
  rewriter = AssemblyReferenceRewriter(options.KeyFile)
  for a in options.Assemblies:
    rewriter.RewriteReferencesOf(a)

class AssemblyReferenceRewriter:
  _version = System.Version(2, 0, 5, 0)
  _publicKeyToken = ParsePublicKeyToken('7cec85d7bea7798e')
  _runtime = TargetRuntime.Net_4_0
  _strongNameKeyPair as StrongNameKeyPair

  def constructor(keyFile as string):
    _strongNameKeyPair = (StrongNameKeyPair(File.ReadAllBytes(keyFile)) if keyFile else null)

  def RewriteReferencesOf(assembly as string):
    m = ModuleDefinition.ReadModule(assembly)
    m.Runtime = _runtime
    for assemblyRef in RetargetableAssemblyReferencesOf(m):
      assemblyRef.Version = _version
      assemblyRef.PublicKeyToken = _publicKeyToken
    m.Write(assembly, WriterParameters(StrongNameKeyPair: _strongNameKeyPair))

  def RetargetableAssemblyReferencesOf(m as ModuleDefinition):
    return (asmRef for asmRef in m.AssemblyReferences if IsRetargetable(asmRef))

  def IsRetargetable(asmRef as AssemblyNameReference):
    name = asmRef.Name
    return name.StartsWith('System') or name == 'mscorlib'

class Options(AbstractCommandLine):

  [Option('The key {file} with which to resign the assembly', LongForm: 'keyfile')]
  public KeyFile as string

  public Assemblies = List of string()

  def constructor(args as (string)):
    Parse(args)

  [Argument]
  def AddAssembly(assembly as string):
    Assemblies.Add(assembly)

def ParsePublicKeyToken(token as string):
  return array(ParseHexByte(token[i:i+2]) for i in range(0, len(token), 2))

def ParseHexByte(s as string):
  return byte.Parse(s, System.Globalization.NumberStyles.HexNumber)
