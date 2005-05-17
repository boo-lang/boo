#!/bin/sh
test -z "${verbose}" && { 
  verbose=0
}

abs_top_build_dir=/home/rodrigob/java/antlr-2.7.5/scripts/../.
java_cmd="/usr/bin/java"
antlr_jar="/home/rodrigob/java/antlr-2.7.5/antlr/antlr.jar"
ARGV="$*"

case linux-gnu in
  cygwin)
    test -n "${antlr_jar}" && {
      antlr_jar="`cygpath -m ${antlr_jar}`"
    }
    sep=";"
    ;;
  macos*)
    sep=";"
    ;;
  *)
    sep=":"
    ;;
esac

if test -d "${abs_top_build_dir}"; then
  if test -f "${antlr_jar}" ; then
    
    if test -z "${CLASSPATH}"; then
      ## needs fine tuning - depends on os (wh:tbd)
      CLASSPATH=".${sep}${antlr_jar}"
      export CLASSPATH
    else
      ## needs fine tuning - depends on os (wh:tbd)
      CLASSPATH="${sep}${antlr_jar}${sep}${CLASSPATH}" 
    fi        
  fi
fi

## Translate all non option arguments 
case linux-gnu in
  cygwin)
    set x $ARGV ; shift
    ARGV=
    while test $# -gt 0 ; do
      case $1 in
        -*) 
          ARGV="$ARGV $1"
          ;;
        *)
          ARGV="$ARGV `echo $1`"
          ;;
      esac
      shift
    done
    ;;
  *)
    ;;
esac



## go ahead ..
cmd="${java_cmd} ${ARGV}"
case "${verbose}" in
  0)
    echo $cmd 
    ;;
  *)
    echo $cmd
    ;;
esac

$cmd || {
  rc=$?
  cat <<EOF

xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                      >> E R R O R <<
============================================================
CLASSPATH=$CLASSPATH

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
