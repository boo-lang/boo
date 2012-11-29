# rewrites all assembly references to match wp8 assemblies:
#   Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
# and change the assembly runtime to net-4.0

import Mono.Cecil

def Main(assemblies as (string)):
  rewriter = AssemblyReferenceRewriter()
  for a in assemblies:
    rewriter.RewriteReferencesOf(a)

class AssemblyReferenceRewriter:
  _version = System.Version(2, 0, 5, 0)
  _publicKeyToken = ParsePublicKeyToken('7cec85d7bea7798e')
  _runtime = TargetRuntime.Net_4_0
  _strongNameKeyPair = System.Reflection.StrongNameKeyPair('src/boo.snk')

  def RewriteReferencesOf(assembly as string):
    m = ModuleDefinition.ReadModule(assembly)
    m.Runtime = _runtime
    for assemblyRef in m.AssemblyReferences:
      assemblyRef.Version = _version
      assemblyRef.PublicKeyToken = _publicKeyToken
    m.Write(assembly, WriterParameters(StrongNameKeyPair: _strongNameKeyPair))

def ParsePublicKeyToken(token as string):
  return array(ParseHexByte(token[i:i+2]) for i in range(0, len(token), 2))

def ParseHexByte(s as string):
  return byte.Parse(s, System.Globalization.NumberStyles.HexNumber)
