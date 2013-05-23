namespace booi

from System import Environment
from System.IO import Path
from Boo.Lang.Useful.CommandLine import *


class CommandLine(AbstractCommandLine):

    [getter(References)]
    _references = List[of string]()

    [getter(Files)]
    _files = List[of string]()

    [getter(Defines)]
    _defines = List[of string]()

    IsValid:
        get: return true

    [Option("Turns on duck typing by default", LongForm: "ducky")]
    public Ducky = false

    [Option("Generate debugging information (default: +)", LongForm: "debug")]
    public Debug = true

    [Option("Turns on strict mode", LongForm: "strict")]
    public Strict = false

    [Option("Enables white-space-agnostic build", LongForm: "wsa")]
    public Wsa = false

    [Option("Report warnings (default: -)", ShortForm: "w", LongForm: "warnings")]
    public Warnings = false

    [Option("Generate verbose information (default: -)", ShortForm: "v", LongForm: "verbose")]
    public Verbose = false

    [Option("Defines a {symbol}s with optional values (=val)", ShortForm: "d", LongForm: "define")]
    def AddDefine(define as string):
        if not define:
            raise CommandLineException("No value given for the define")
        _defines.Add(define)

    [Option("Adds the comma-separated DIRS to the assembly search path", LongForm: "lib")]
    public LibPaths = ''

    [Option("References {assembly}", ShortForm: 'r', LongForm: "reference", MaxOccurs: int.MaxValue)]
    def AddReference(reference as string):
        if not reference:
            raise CommandLineException("No reference given (ie: -r:my.project.reference)")
        _references.AddUnique(TranslatePath(reference))

    [Option("Display this help and exit", ShortForm: "h", LongForm: "help")]
    public DoHelp = false

    [Option("Display program version", LongForm: "version")]
    public DoVersion = false

    [Argument]
    def AddFile([required] file as string):
        _files.Add(TranslatePath(file))

    def constructor(argv):
        Parse(argv)

    IsCygwin:
        get: return Environment.GetEnvironmentVariable("TERM") == "cygwin"
    
    CygwinRoot:
        get:
            home = Environment.GetEnvironmentVariable("HOME")
            assert home is not null
            return home[:home.IndexOf("\\home")]        

    protected def TranslatePath(path as string):
        if path.StartsWith('"') or path.StartsWith("'"):
            path = path[1:-1]

        if not IsCygwin or not path.StartsWith('/'):
            return path

        if path.StartsWith('/cygdrive/'):
            path = path[len('/cygdrive/'):]
            drive = path[0]
            return drive + ':' + path[1:]

        return Path.Combine(CygwinRoot, path[1:])

