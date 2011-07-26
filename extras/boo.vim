" Vim syntax file
" Language: boo
" Maintainer: nsf <no.smile.face@gmail.com>
" Latest Revision: 22 July 2011
"
" Based on boo.vim by Rodrigo B. de Oliveira which is based on python syntax
" file by Neil Schemenauer.
"
" Options to control syntax highlighting:
"
" For highlighted numbers:
"
"    let boo_highlight_numbers = 1
"
" For highlighted builtin functions:
"
"    let boo_highlight_builtins = 1
"
" For highlighted standard exceptions:
"
"    let boo_highlight_exceptions = 1
"
" Highlight erroneous whitespace:
"
"    let boo_highlight_space_errors = 1
"
" If you want all possible highlighting (the same as setting the
" preceding options):
"

let boo_highlight_all = 1

"
" For version 5.x: Clear all syntax items
" For version 6.x: Quit when a syntax file was already loaded
if version < 600
  syntax clear
elseif exists("b:current_syntax")
  finish
endif

syn keyword booConstant         true false null

syn keyword booAccess           public protected private

syn keyword booModifier         abstract final internal override ref new
syn keyword booModifier         partial static transient virtual event

syn region  booImportRegion     start="^import" end="$" contains=booImport
syn keyword booImport           import as from contained

syn keyword booRepeat           for while then

syn keyword booConditional      if elif else unless

syn keyword booStatement        break continue return pass yield goto
syn keyword booStatement        get set
syn keyword booStatement        constructor destructor typeof super

syn keyword booOperator         and in is isa in not or of cast as

syn keyword booExceptionKWs     try except raise ensure failure

syn keyword booStorage          callable class def enum do
syn keyword booStorage          interface namespace struct

syn keyword booTodo             WARNING TODO FIXME XXX contained
syn match   booComment          "#.*$" contains=booTodo
syn match   booComment2         "//.*$" contains=booTodo
syn region  booRegionComment    start="/\*"  end="\*/" contains=booTodo

" strings
syn region  booString           matchgroup=Normal start=+[uU]\='+ end=+'+ skip=+\\\\\|\\'+ contains=booEscape
syn region  booString           matchgroup=Normal start=+[uU]\="+ end=+"+ skip=+\\\\\|\\"+ contains=booEscape
syn region  booString           matchgroup=Normal start=+[uU]\="""+ end=+"""+ contains=booEscape
syn region  booString           matchgroup=Normal start=+[uU]\='''+ end=+'''+ contains=booEscape
syn region  booRawString        matchgroup=Normal start=+[uU]\=[rR]'+ end=+'+ skip=+\\\\\|\\'+
syn region  booRawString        matchgroup=Normal start=+[uU]\=[rR]"+ end=+"+ skip=+\\\\\|\\"+
syn region  booRawString        matchgroup=Normal start=+[uU]\=[rR]"""+ end=+"""+
syn region  booRawString        matchgroup=Normal start=+[uU]\=[rR]'''+ end=+'''+
syn match   booEscape           +\\[abfnrtv'"\\]+ contained
syn match   booEscape           "\\\o\{1,3}" contained
syn match   booEscape           "\\x\x\{2}" contained
syn match   booEscape           "\(\\u\x\{4}\|\\U\x\{8}\)" contained
syn match   booEscape           "\\$"
" TODO: regexp?

if exists("boo_highlight_all")
        let boo_highlight_numbers = 1
        let boo_highlight_builtins = 1
        let boo_highlight_exceptions = 1
        let boo_highlight_space_errors = 1
endif

"------------------------------------------------------------------------------
" Built-ins
"------------------------------------------------------------------------------

if exists("boo_highlight_builtins")
        " built-in macros from:
        " grep "^macro" Boo.Lang.Extensions/Macros/*.boo
        syn keyword booBuiltin assert unchecked checked debug lock preserving print
        syn keyword booBuiltin property normalArrayIndexing rawArrayIndexing using
        syn keyword booBuiltin yieldAll

        " built-in functions from booish:
        " dir(Boo.Lang.Builtins)
        syn keyword booBuiltin print gets prompt join map array matrix iterator
        syn keyword booBuiltin shellp shell shellm enumerate range reversed zip cat

        " built-in types from:
        " Boo.Lang.Compiler/TypeSystem/Services/TypeSystemServices.cs:997
        syn keyword booType    duck void object callable decimal date
        syn keyword booType    bool sbyte byte short ushort int uint long ulong
        syn keyword booType    single double char string regex timespan

        " self
        syn keyword booBuiltin self
endif

"------------------------------------------------------------------------------
" Numbers from:
" Boo.Lang.Parser/booel.g:67
"------------------------------------------------------------------------------

" holy shit, if anyone wants to edit this, good luck :D
"                                     digits                ([eE][+-]? digits)?                       ([lL]   |  [fF]   |    (('.' digits               ([eE][+-]? digits)?                    [fF]?   )?     ([smhd] | ms)?))
"syn match   booNumber           "\<  \d\%(\%(_\d\)\|\d\)*  \%([eE][+-]\=\d\%(\%(_\d\)\|\d\)*\)\=   \%([lL]  \|  [fF]  \|  \%(\%(\.\d\%(\%(_\d\)\|\d\)* \%([eE][+-]\=\d\%(\%(_\d\)\|\d\)*\)\=  [fF]\=  \)\=   \%([smhd]\|ms\)\=\) \)\>"
"
if exists("boo_highlight_numbers")
        syn match booNumber "\<0x\x\+[lL]\=\>"
        syn match booNumber "\<\d\%(\%(_\d\)\|\d\)*\%([eE][+-]\=\d\%(\%(_\d\)\|\d\)*\)\=\%([lL]\|[fF]\|\%(\%(\.\d\%(\%(_\d\)\|\d\)*\%([eE][+-]\=\d\%(\%(_\d\)\|\d\)*\)\=[fF]\=\)\=\%([smhd]\|ms\)\=\)\)\>"
        syn match booNumber "\.\d\%(\%(_\d\)\|\d\)*\%([eE][+-]\=\d\%(\%(_\d\)\|\d\)*\)\=\%([fF]\|\%([smhd]\|ms\)\)\=\>"
endif

if exists("boo_highlight_exceptions")
        " common .NET exceptions
        syn keyword booException Exception SystemException ArgumentException
        syn keyword booException ArgumentNullException ArgumentOutOfRangeException
        syn keyword booException DuplicateWaitObjectException ArithmeticException
        syn keyword booException DivideByZeroException OverflowException
        syn keyword booException NotFiniteNumberException ArrayTypeMismatchException
        syn keyword booException ExecutionEngineException FormatException
        syn keyword booException IndexOutOfRangeException InvalidCastException
        syn keyword booException InvalidOperationException ObjectDisposedException
        syn keyword booException InvalidProgramException IOException
        syn keyword booException DirectoryNotFoundException EndOfStreamException
        syn keyword booException FileLoadException FileNotFoundException
        syn keyword booException PathTooLongException NotImplementedException
        syn keyword booException NotSupportedException NullReferenceException
        syn keyword booException OutOfMemoryException RankException
        syn keyword booException SecurityException VerificationException
        syn keyword booException StackOverflowException SynchronizationLockException
        syn keyword booException ThreadAbortException ThreadStateException
        syn keyword booException TypeInitializationException UnauthorizedAccessException
endif

if exists("boo_highlight_space_errors")
        " trailing whitespace
        syn match booSpaceError display excludenl "\S\s\+$"ms=s+1
        " mixed tabs and spaces
        syn match booSpaceError display " \+\t"
        syn match booSpaceError display "\t\+ "
endif

" This is fast but code inside triple quoted strings screws it up. It
" is impossible to fix because the only way to know if you are inside a
" triple quoted string is to start from the beginning of the file. If
" you have a fast machine you can try uncommenting the "sync minlines"
" and commenting out the rest.
syn sync match booSync grouphere NONE "):$"
syn sync maxlines=200
"syn sync minlines=2000

if version >= 508 || !exists("did_boo_syn_inits")
        if version <= 508
                let did_boo_syn_inits = 1
                command -nargs=+ HiLink hi link <args>
        else
                command -nargs=+ HiLink hi def link <args>
        endif

        " The default methods for highlighting.  Can be overridden later
        HiLink booConstant            Constant
        HiLink booAccess              StorageClass
        HiLink booModifier            StorageClass
        HiLink booImport              Include
        HiLink booRepeat              Repeat
        HiLink booConditional         Conditional
        HiLink booStatement           Statement
        HiLink booOperator            Operator
        HiLink booExceptionKWs        Exception
        if exists("boo_highlight_exceptions")
                HiLink booException   Exception
        endif
        HiLink booStorage             StorageClass
        HiLink booTodo                Todo
        HiLink booComment             Comment
        HiLink booComment2            Comment
        HiLink booRegionComment       Comment
        HiLink booString              String
        HiLink booRawString           String
        HiLink booRegex               String
        HiLink booEscape              Special
        if exists("boo_highlight_builtins")
                HiLink booBuiltin     Function
                HiLink booType        Type
        endif
        if exists("boo_highlight_numbers")
                HiLink booNumber      Number
        endif
        if exists("boo_highlight_space_errors")
                HiLink booSpaceError  Error
        endif

        delcommand HiLink
endif

let b:current_syntax = "boo"
" vim: ts=8
