using System // Environment
using System.IO // Directory
using Boo.IO // TextFile

_, glob, expression = Environment.GetCommandLineArgs()
print("searching for ${expression} in ${glob}...")
