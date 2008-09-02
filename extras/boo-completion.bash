# boo(c|i|ish) completion 
# put this file in /etc/bash_completion.d/ 

have booc &&
_booc()
{
	local cur

	COMP_WORDBREAKS=${COMP_WORDBREAKS//:}
	COMPREPLY=()
	cur=${COMP_WORDS[COMP_CWORD]}

	if [[ "$cur" == -* ]]; then
		COMPREPLY=( $( compgen -W '-help -v -vv -vvv \
		-debug- -define: -delaysign -ducky -checked- -embedres: \
		-lib: -noconfig -nostdlib -nologo -p:boo -p:ast -p:dump -p:verify \
		-target:exe -target:library -target:winexe -o: \
		-reference: -srcdir: -resource: -pkg: -utf8 -wsa' -- $cur ) ) 
	else
		_filedir '@(boo)'
	fi
}
[ "${have:-}" ] && complete -F _booc $filenames booc


have booi &&
_booi()
{
	local cur

	COMP_WORDBREAKS=${COMP_WORDBREAKS//:}
	COMPREPLY=()
	cur=${COMP_WORDS[COMP_CWORD]}

	if [[ "$cur" == -* ]]; then
		COMPREPLY=( $( compgen -W '\
		-debug- -define: -ducky -checked- \
		-noconfig -nostdlib -nologo \
		-reference: -utf8 -wsa' -- $cur ) ) 
	else
		_filedir '@(boo)'
	fi
}
[ "${have:-}" ] && complete -F _booi $filenames booi


have booish &&
_booish()
{
	local cur

	COMP_WORDBREAKS=${COMP_WORDBREAKS//:}
	COMPREPLY=()
	cur=${COMP_WORDS[COMP_CWORD]}

	if [[ "$cur" == -* ]]; then
		COMPREPLY=( $( compgen -W '\
		-debug- -define: -ducky -checked- \
		-noconfig -nostdlib -nologo \
		-reference: -utf8 -wsa' -- $cur ) ) 
	else
		_filedir '@(boo)'
	fi
}
[ "${have:-}" ] && complete -F _booish $filenames booish

