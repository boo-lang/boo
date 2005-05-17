#!/bin/bash
##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##
## This file is part of ANTLR. See LICENSE.txt for licence  ##
## details. Written by W. Haefelinger.                      ##
##                                                          ##
##       Copyright (C) Wolfgang Haefelinger, 2004           ##
##                                                          ##
##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##
test -z "${verbose}" && { 
  verbose=0
}

cmd=/usr/bin/python
PYTHONPATH=/home/rodrigob/java/antlr-2.7.5/scripts/../lib/python
export PYTHONPATH
ARGV="$*"

##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##
##    This shall be the command to be excuted below       ##
##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##

cmd="/usr/bin/python  ${ARGV}"

##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##
##        standard template to execute a command          ##
##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##
case "${verbose}" in
  0|no|nein|non)
    ;;
  *)
    echo PYTHONPATH=${PYTHONPATH}
    echo $cmd
    ;;
esac

$cmd || {
  rc=$?
  cat <<EOF

xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                      >> E R R O R <<
============================================================

$cmd

============================================================
Got an error while trying to execute  command  above.  Error
messages (if any) must have shown before. The exit code was:
exit($rc)
xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
EOF
  exit $rc
}
exit 0
