namespace booi

from System import DateTime, Convert, AttributeUsageAttribute, AttributeTargets, Attribute
from System.IO import File, Directory, Path, StreamReader, StreamWriter
from Boo.Lang.Compiler import CompilerContext
from Boo.Lang.Compiler.TypeSystem.Reflection import IAssemblyReference



[AttributeUsage(AttributeTargets.Assembly)]
class CompilationTimeAttribute(Attribute):
    _date as long

    def constructor(timestamp as long):
        _date = timestamp

    def GetDateTime():
        return DateTime.FromFileTimeUtc(_date)


class CompilationCache:
""" Represents a compilation cache file used to speed up the import of source directories
    mapped to namespaces.

    The format is very simple and text based to ease troubleshooting, the overhead
    of using a text format should be negligible when compared to running the compiler.
    File sections are delimited by lines with values separated by tab '\t' characters.
    In the example below lines starting with '#' are comments to describe the format and
    are not part of the actual file format.

        # Header identifying the file and the format version
        BOO-COMP-CACHE 1.0
        # Absolute path of the imported namespace
        path    /Users/drslump/www/Boo.Hints/src/Boo.Hints
        # Set of source files compiled and the last modified timestamp (Windows File Time in UTC)
        source  DummySymbolFinder.boo   130147536580000000
        source  ISymbolFinder.boo       130147536520000000
        # Set of references active when the source was compiled. They are represented with
        # their IAssemblyReference name followed by either a timestamp of their compilation
        # or their main module's ModuleVersionId GUID.
        reference       booi    696c0d16-ffa0-4a2b-b00c-6429ff1de98e
        reference       Boo.Hints.Utils 130148414727967560        
        # The assembly payload encoded as base64
        assembly        <base64>
        # The symbols file payload encoded as base64 (for mono it's 'mdb' instead of 'pdb')
        pdb <base64>

"""
    static final HEADER = 'BOO-COMP-CACHE 1.0'

    [getter(Sources)]
    _sources = {}

    [getter(References)]
    _references = {}

    property BasePath as string

    static IsMono:
        get: return null != System.Type.GetType('Mono.Runtime')

    _assembly as (byte)
    _symbols as (byte)

    static def Load(fname as string):
        return CompilationCache(fname)

    def constructor():
        pass

    internal def constructor(fname as string):
        using fs = StreamReader(fname):
            version = fs.ReadLine()
            if version != HEADER:
                raise 'Unsupported file'

            while fs.Peek() != -1:
                line = fs.ReadLine()
                parts = line.Split(char('\t'))
                if parts[0] == 'path':
                    BasePath = parts[1]
                elif parts[0] == 'source':
                    timestamp = long.Parse(parts[2])
                    Sources[parts[1]] = DateTime.FromFileTimeUtc(timestamp)
                elif parts[0] == 'reference':
                    try:
                        token = System.Guid.Parse(parts[2])
                        References[parts[1]] = token
                    except:
                        timestamp = long.Parse(parts[2])
                        References[parts[1]] = DateTime.FromFileTimeUtc(timestamp)
                elif parts[0] == 'assembly':
                    _assembly = Convert.FromBase64String(parts[1])
                elif parts[0] == 'mdb':
                    if IsMono:
                        _symbols = Convert.FromBase64String(parts[1])
                elif parts[0] == 'pdb':
                    if not IsMono:
                        _symbols = Convert.FromBase64String(parts[1])
                else:
                    raise 'Malformed file'

    def Save(fname as string, asmpath as string):
        using fs = StreamWriter(fname):
            fs.WriteLine(HEADER)
            fs.WriteLine("path\t$(BasePath)")

            for src in Sources:
                fs.WriteLine("source\t$(src.Key)\t$(src.Value)")

            for item in References:
                fs.WriteLine("reference\t$(item.Key)\t$(item.Value)")

            b64 = Convert.ToBase64String(File.ReadAllBytes(asmpath))
            fs.WriteLine("assembly\t$(b64)")

            if IsMono and File.Exists(asmpath + '.mdb'):
                b64 = Convert.ToBase64String(File.ReadAllBytes(asmpath + '.mdb'))
                fs.WriteLine("mdb\t$(b64)")
            elif not IsMono and File.Exists(asmpath[:-3] + 'pdb'):
                b64 = Convert.ToBase64String(File.ReadAllBytes(asmpath[:-3] + 'pdb'))
                fs.WriteLine("pdb\t$(b64)")

    def GetAssembly():
        if _symbols:
            return Assembly.Load(_assembly, _symbols)
        else:
            return Assembly.Load(_assembly)

    def DumpAssembly(path as string, ns as string):
        path = Path.Combine(path, ns) + '.dll'
        File.WriteAllBytes(path, _assembly)
        if _symbols:
            path = (path + '.mdb' if IsMono else path[:-3] + 'pdb')
            File.WriteAllBytes(path, _symbols)

    def CheckValidity(context as CompilerContext):
        # Check if the number of files is the same
        files = Directory.GetFiles(BasePath, '*.boo')
        if len(files) != len(Sources):
            context.TraceInfo("Directory contents at '{0}' have changed", BasePath)
            return false

        # Check if any source file was modified
        for source in Sources:
            fpath = Path.Combine(BasePath, source.Key)
            if fpath not in files:
                context.TraceInfo("File '{0}' was removed", fpath)
                return false
            if source.Value != File.GetLastWriteTimeUtc(fpath):
                context.TraceInfo("File '{0}' was modified", fpath)
                return false

        # Check if any of the references was modified
        for rf in References:
            asmref as IAssemblyReference = context.Parameters.FindAssembly(rf.Key) or \
                                           context.Parameters.LoadAssembly(rf.Key, false)
            if not asmref:
                context.TraceInfo("Reference '{0}' not found", rf.Key)
                return false

            # Check if the assembly includes our custom compilation date attribute
            attrs = asmref.Assembly.GetCustomAttributes(CompilationTimeAttribute, false)
            if len(attrs):
                dt = (attrs[0] as CompilationTimeAttribute).GetDateTime()
                if dt != (rf.Value cast DateTime):
                    context.TraceInfo("Reference '{0}' was modified (CompilationTime)", rf.Key)
                    return false

            # Otherwise just compare their module version id
            elif asmref.Assembly.ManifestModule.ModuleVersionId != rf.Value:
                context.TraceInfo("Reference '{0}' was modified (ModuleVersionId)", rf.Key)
                return false

        return true