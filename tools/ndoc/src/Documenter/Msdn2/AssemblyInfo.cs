using System;
using System.Reflection;

[assembly: CLSCompliantAttribute(true)]
[assembly: AssemblyTitle("NDoc MSDN Documenter")]
[assembly: AssemblyDescription("MSDN-like documenter for the NDoc code documentation generator.")]

#if (!DEBUG)
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("NDoc.snk")]
[assembly: AssemblyKeyName("")]
#endif
