" Vim syntax file
" Language:	boo
" Maintainer:	Rodrigo B. de Oliveira
" Based on python syntax file by Neil Schemenauer
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
"    let boo_highlight_all = 1
"

" For version 5.x: Clear all syntax items
" For version 6.x: Quit when a syntax file was already loaded
if version < 600
  syntax clear
elseif exists("b:current_syntax")
  finish
endif


syn keyword booStatement	break continue 
syn keyword booStatement	except exec ensure
syn keyword booStatement	pass print raise
syn keyword booStatement	return try
syn keyword booStatement	assert
syn keyword booStatement	yield
syn keyword booStatement	def class nextgroup=booFunction skipwhite
syn match   booFunction	"[a-zA-Z_][a-zA-Z0-9_]*" contained
syn keyword booRepeat	for while
syn keyword booConditional	if unless else
syn keyword booOperator	and in is not or
syn keyword booPreCondit	import from as
syn match   booComment	"#.*$" contains=booTodo
syn keyword booTodo		TODO FIXME XXX contained

" strings
syn region booString		matchgroup=Normal start=+[uU]\='+ end=+'+ skip=+\\\\\|\\'+ contains=booEscape
syn region booString		matchgroup=Normal start=+[uU]\="+ end=+"+ skip=+\\\\\|\\"+ contains=booEscape
syn region booString		matchgroup=Normal start=+[uU]\="""+ end=+"""+ contains=booEscape
syn region booString		matchgroup=Normal start=+[uU]\='''+ end=+'''+ contains=booEscape
syn region booRawString	matchgroup=Normal start=+[uU]\=[rR]'+ end=+'+ skip=+\\\\\|\\'+
syn region booRawString	matchgroup=Normal start=+[uU]\=[rR]"+ end=+"+ skip=+\\\\\|\\"+
syn region booRawString	matchgroup=Normal start=+[uU]\=[rR]"""+ end=+"""+
syn region booRawString	matchgroup=Normal start=+[uU]\=[rR]'''+ end=+'''+
syn match  booEscape		+\\[abfnrtv'"\\]+ contained
syn match  booEscape		"\\\o\{1,3}" contained
syn match  booEscape		"\\x\x\{2}" contained
syn match  booEscape		"\(\\u\x\{4}\|\\U\x\{8}\)" contained
syn match  booEscape		"\\$"

if exists("boo_highlight_all")
  let boo_highlight_numbers = 1
  let boo_highlight_builtins = 1
  let boo_highlight_exceptions = 1
  let boo_highlight_space_errors = 1
endif

if exists("boo_highlight_numbers")
  " numbers (including longs and complex)
  syn match   booNumber	"\<0x\x\+[Ll]\=\>"
  syn match   booNumber	"\<\d\+[LljJ]\=\>"
  syn match   booNumber	"\.\d\+\([eE][+-]\=\d\+\)\=[jJ]\=\>"
  syn match   booNumber	"\<\d\+\.\([eE][+-]\=\d\+\)\=[jJ]\=\>"
  syn match   booNumber	"\<\d\+\.\d\+\([eE][+-]\=\d\+\)\=[jJ]\=\>"
endif

if exists("boo_highlight_builtins")
  " builtin functions, types and objects, not really part of the syntax
  syn keyword booBuiltin	Ellipsis None NotImplemented __import__ abs
  syn keyword booBuiltin	apply buffer callable chr classmethod cmp
  syn keyword booBuiltin	coerce compile complex delattr dict dir divmod
  syn keyword booBuiltin	eval execfile file filter float getattr globals
  syn keyword booBuiltin	hasattr hash hex id input int intern isinstance
  syn keyword booBuiltin	issubclass iter len list locals long map max
  syn keyword booBuiltin	min object oct open ord pow property range
  syn keyword booBuiltin	raw_input reduce reload repr round setattr
  syn keyword booBuiltin	slice staticmethod str super tuple type unichr
  syn keyword booBuiltin	unicode vars xrange zip
endif

if exists("boo_highlight_exceptions")
  " builtin exceptions and warnings
  syn keyword booException	ArithmeticError AssertionError AttributeError
  syn keyword booException	DeprecationWarning EOFError EnvironmentError
  syn keyword booException	Exception FloatingPointError IOError
  syn keyword booException	ImportError IndentationError IndexError
  syn keyword booException	KeyError KeyboardInterrupt LookupError
  syn keyword booException	MemoryError NameError NotImplementedError
  syn keyword booException	OSError OverflowError OverflowWarning
  syn keyword booException	ReferenceError RuntimeError RuntimeWarning
  syn keyword booException	StandardError StopIteration SyntaxError
  syn keyword booException	SyntaxWarning SystemError SystemExit TabError
  syn keyword booException	TypeError UnboundLocalError UnicodeError
  syn keyword booException	UserWarning ValueError Warning WindowsError
  syn keyword booException	ZeroDivisionError
endif

if exists("boo_highlight_space_errors")
  " trailing whitespace
  syn match   booSpaceError   display excludenl "\S\s\+$"ms=s+1
  " mixed tabs and spaces
  syn match   booSpaceError   display " \+\t"
  syn match   booSpaceError   display "\t\+ "
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
  HiLink booStatement	Statement
  HiLink booFunction		Function
  HiLink booConditional	Conditional
  HiLink booRepeat		Repeat
  HiLink booString		String
  HiLink booRawString	String
  HiLink booEscape		Special
  HiLink booOperator		Operator
  HiLink booPreCondit	PreCondit
  HiLink booComment		Comment
  HiLink booTodo		Todo
  if exists("boo_highlight_numbers")
    HiLink booNumber	Number
  endif
  if exists("boo_highlight_builtins")
    HiLink booBuiltin	Function
  endif
  if exists("boo_highlight_exceptions")
    HiLink booException	Exception
  endif
  if exists("boo_highlight_space_errors")
    HiLink booSpaceError	Error
  endif

  delcommand HiLink
endif

let b:current_syntax = "boo"

" vim: ts=8
