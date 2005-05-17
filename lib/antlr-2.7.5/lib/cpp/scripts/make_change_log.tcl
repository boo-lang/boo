#!/bin/sh
# the next line restarts using tclsh \
exec tclsh8.3 $0 $*

# 
# Sort the (C++) changes recorded in the repository by change number and
# print them to stdout
#
set depots {//depot/code/org.antlr/dev/klaren.dev //depot/code/org.antlr/main/main }
set files { /lib/cpp/... /antlr/... }
set filespec ""
foreach depot $depots {
	foreach file $files {
		append filespec "$depot$file "
	}
}

puts stderr "Gettting changes from: $filespec"

catch {set file [open "|p4 changes -l $filespec" r]}

set cnt 0
set changes {}
set text ""
set change_nr -1

while {![eof $file]} {
	set line [gets $file]

	if { [regexp -- {^Change ([0-9]+).*$} $line dummy tmp] } {
		# append the number to the list of found changes
		lappend changes $tmp

		if { $change_nr != -1 } {
			# were already working on change..
			# so we have text to store..
			set description($change_nr) $text
		}

		# remember number...
		set change_nr $tmp
		# reinit text
		set text "[string trim $line]\n"
	} else {
		append text "   [string trim $line]\n"
	}
}

set description($change_nr) $text

catch {close $file}

set sorted_changes [lsort -unique -integer -decreasing $changes]

foreach change $sorted_changes {
	puts $description($change)
}
