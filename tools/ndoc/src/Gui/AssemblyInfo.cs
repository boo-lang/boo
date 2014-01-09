using System;
using System.Reflection;

[assembly: CLSCompliantAttribute(true)]
[assembly: AssemblyTitle("NDoc Gui")]
[assembly: AssemblyDescription("Winform user interface for the NDoc code documentation generator.")]

#if (OFFICIAL_RELEASE)
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("NDoc.snk")]
[assembly: AssemblyKeyName("")]
#endif
