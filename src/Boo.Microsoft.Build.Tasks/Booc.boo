#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Microsoft.Build.Tasks

import System
import System.IO
import System.Text.RegularExpressions
import Microsoft.Build.Utilities
import Microsoft.Build.Framework

class Booc(ToolTask):

    private bag = {}

    [property(AdditionalLibPaths)]
    additionalLibPaths as (string)
    # Allows to compile unsafe code.
    [property(AllowUnsafeBlocks)]
    allowUnsafeBlocks as bool
    # Gets/sets if integer overlow checking is enabled.
    [property(CheckForOverflowUnderflow)]
    checkForOverflowUnderflow as bool
    # Gets/sets the culture.
    [property(Culture)]
    culture as string
    # Gets/sets the conditional compilation symbols.
    [property(DefineSymbols)]
    defineSymbols as string
    [property(DelaySign)]
    delaySign as bool
    # Gets/sets a comma-separated list of warnings that should be disabled.
    [property(DisabledWarnings)]
    disabledWarnings as string
    # Gets/sets if we want to use ducky mode.
    [property(Ducky)]
    ducky as bool
    [property(EmitDebugInformation)]
    emitDebugInformation as bool
    # If set to true the task will output warnings and errors with full file paths
    [property(GenerateFullPaths)]
    generateFullPaths as bool
    [property(KeyContainer)]
    keyContainer as string
    [property(KeyFile)]
    keyFile as string
    [property(NoConfig)]
    noConfig as bool
    [property(NoLogo)]
    noLogo as bool
    # Gets/sets if we want to link to the standard libraries or not.
    [property(NoStandardLib)]
    noStandardLib as bool
    # Gets/sets a comma-separated list of optional warnings that should be enabled.
    [property(OptionalWarnings)]
    optionalWarnings as string
    # Gets/sets a specific pipeline to add to the compiler process.
    [property(Pipeline)]
    pipeline as string
    # Specifies target platform.
    [property(Platform)]
    platform as string
    # Gets/sets the source directory.
    [property(SourceDirectory)]
    sourceDirectory as string
    # Gets/sets whether strict mode is enabled.
    [property(Strict)]
    strict as bool
    [property(TargetType)]
    targetType as string
    [property(TargetFrameworkVersion)]
    targetFrameworkVersion as string
    [property(TreatWarningsAsErrors)]
    treatWarningsAsErrors as bool
    [property(Utf8Output)]
    utf8Output as bool
    # Gets/sets the verbosity level.
    [property(Verbosity)]
    verbosity as string
    # Gets/sets a comma-separated list of warnings that should be treated as errors.
    [property(WarningsAsErrors)]
    warningsAsErrors as string
    # Gets/sets if we want to use whitespace agnostic mode.
    [property(WhiteSpaceAgnostic)]
    whiteSpaceAgnostic as bool

    [Output]
    OutputAssembly as ITaskItem:
        get: return bag['output-assembly']
        set: bag['output-assembly'] = value

    [Required]
    References as (ITaskItem):
        get: return bag['references']
        set: bag['references'] = value

    [Required]
    ResponseFiles as (ITaskItem):
        get: return bag['response-files']
        set: bag['response-files'] = value

    [Required]
    Resources as (ITaskItem):
        get: return bag['resources']
        set: bag['resources'] = value

    [Required]
    Sources as (ITaskItem):
        get: return bag['sources']
        set: bag['sources'] = value


    protected override def GenerateFullPathToTool():
        path = ""
        if ToolPath:
            path = Path.Combine(ToolPath, ToolName)
        return path if File.Exists(path)

        path = Path.Combine(
            Path.GetDirectoryName(GetType().Assembly.Location),
            ToolName)
        return path if File.Exists(path)

        path = ToolLocationHelper.GetPathToDotNetFrameworkFile(
            ToolName,
            TargetDotNetFrameworkVersion.VersionLatest)
        return path if File.Exists(path)

        return "booc"

    protected override ToolName:
        get: return 'booc.exe'

    protected override def GenerateCommandLineCommands():
        commandLine = CommandLineBuilder()

        commandLine.AppendSwitchIfNotNull("-t:", TargetType.ToLower())
        commandLine.AppendSwitchIfNotNull("-o:", OutputAssembly)
        commandLine.AppendSwitchIfNotNull("-c:", Culture)
        commandLine.AppendSwitchIfNotNull("-srcdir:", SourceDirectory)
        commandLine.AppendSwitchIfNotNull("-keyfile:", KeyFile)
        commandLine.AppendSwitchIfNotNull("-keycontainer:", KeyContainer)
        commandLine.AppendSwitchIfNotNull("-p:", Pipeline)
        commandLine.AppendSwitchIfNotNull("-define:", DefineSymbols)
        commandLine.AppendSwitchIfNotNull("-lib:", AdditionalLibPaths, ",")
        commandLine.AppendSwitchIfNotNull("-nowarn:", DisabledWarnings)
        commandLine.AppendSwitchIfNotNull("-warn:", OptionalWarnings)
        commandLine.AppendSwitchIfNotNull("-platform:", Platform)
 
        if TreatWarningsAsErrors:
            commandLine.AppendSwitch("-warnaserror");  # all warnings are errors
        else:
            commandLine.AppendSwitchIfNotNull("-warnaserror:", WarningsAsErrors)  # only specific warnings are errors
    
        if NoLogo:
            commandLine.AppendSwitch("-nologo")

        if NoConfig:
            commandLine.AppendSwitch("-noconfig")

        if NoStandardLib:
            commandLine.AppendSwitch("-nostdlib")

        if DelaySign:
            commandLine.AppendSwitch("-delaysign")

        if WhiteSpaceAgnostic:
            commandLine.AppendSwitch("-wsa")

        if Ducky:
            commandLine.AppendSwitch("-ducky")

        if Utf8Output:
            commandLine.AppendSwitch("-utf8")

        if Strict:
            commandLine.AppendSwitch("-strict")

        if AllowUnsafeBlocks:
            commandLine.AppendSwitch("-unsafe")

        commandLine.AppendSwitch(('-debug+' if EmitDebugInformation else '-debug-'))

        commandLine.AppendSwitch(('-checked+' if CheckForOverflowUnderflow else '-checked-'))

        if ResponseFiles:
            for rsp in ResponseFiles:
                commandLine.AppendSwitchIfNotNull('@', rsp.ItemSpec)        

        if References:
            for reference in References:
                commandLine.AppendSwitchIfNotNull('-r:', reference.ItemSpec)
            
        if Resources:
            for resource in Resources:
                type = resource.GetMetadata('Type')
                if type == 'Resx':
                    commandLine.AppendSwitchIfNotNull("-resource:", resource.ItemSpec + "," + resource.GetMetadata("LogicalName"))
                else:  # if type == 'Non-Resx':
                    commandLine.AppendSwitchIfNotNull("-embedres:", resource.ItemSpec + "," + resource.GetMetadata("LogicalName"))

        if not String.IsNullOrEmpty(Verbosity):
            verbosity = Verbosity.ToLower()
            if verbosity == 'normal':       pass
            elif verbosity == 'warning':    commandLine.AppendSwitch("-v")
            elif verbosity == 'info':       commandLine.AppendSwitch("-vv")
            elif verbosity == 'verbose':    commandLine.AppendSwitch("-vvv");
            else:
                Log.LogErrorWithCodeFromResources(
                    "Vbc.EnumParameterHasInvalidValue",
                    "Verbosity",
                    Verbosity,
                    "Normal, Warning, Info, Verbose")

        commandLine.AppendFileNamesIfNotNull(Sources, " ")

        return commandLine.ToString()

    # Captures the file, line, column, code, and message from a BOO warning
    # in the form of: Program.boo(1,1): BCW0000: WARNING: This is a warning.
    private warningPattern = Regex(
            "^(?<file>.*?)(\\((?<line>\\d+),(?<column>\\d+)\\):)?" +
            "(\\s?)(?<code>BCW\\d{4}):(\\s)WARNING:(\\s)(?<message>.*)$",
            RegexOptions.Compiled
    )

    # Captures the file, line, column, code, error type, and message from a
    # BOO error of the form of:
    # 1. Program.boo(1,1): BCE0000: This is an error.
    # 2. Program.boo(1,1): BCE0000: Boo.Lang.Compiler.CompilerError:
    #            This is an error. ---> Program.boo:4:19: This is an error
    # 3. BCE0000: This is an error.
    # 4. Fatal error: This is an error.
    #
    #  The second line of the following error format is not cought because 
    # .NET does not support if|then|else in regular expressions,
    #  and the regex will be horrible complicated.  
    #  The second line is as worthless as the first line.
    #  Therefore, it is not worth implementing it.
    #
    #            Fatal error: This is an error.
    #            Parameter name: format.
    private errorPattern = Regex(
            "^(((?<file>.*?)\\((?<line>\\d+),(?<column>\\d+)\\): )?" +
            "(?<code>BCE\\d{4})|(?<errorType>Fatal) error):" +
            "( Boo.Lang.Compiler.CompilerError:)?" +
            " (?<message>.*?)($| --->)",
            RegexOptions.Compiled |
            RegexOptions.ExplicitCapture |
            RegexOptions.Multiline
        )

    protected override def LogEventsFromTextOutput(singleLine as string, messageImportance as MessageImportance) as void:
        if messageImportance == MessageImportance.Normal:
            wMatch = warningPattern.Match(singleLine)
            eMatch = errorPattern.Match(singleLine)
            line as int = 0
            column as int = 0

            if wMatch.Success:
                int.TryParse(wMatch.Groups["line"].Value, line)
                int.TryParse(wMatch.Groups["column"].Value, column)

                Log.LogWarning(
                    null,
                    wMatch.Groups["code"].Value,
                    null,
                    wMatch.Groups["file"].Value,
                    line,
                    column,
                    0,
                    0,
                    wMatch.Groups["message"].Value
                )
            elif eMatch.Success:
                code = eMatch.Groups["code"].Value
                if string.IsNullOrEmpty(code):
                    code = "BCE0000";
                file = eMatch.Groups["file"].Value
                if string.IsNullOrEmpty(file):
                    file = "BOOC"

                int.TryParse(eMatch.Groups["line"].Value, line)
                int.TryParse(eMatch.Groups["column"].Value, column)

                Log.LogError(
                    eMatch.Groups["errorType"].Value.ToLower(),
                    code,
                    null,
                    file,
                    line,
                    column,
                    0,
                    0,
                    eMatch.Groups["message"].Value
                )
        
        super(singleLine, messageImportance)
