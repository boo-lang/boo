dnl --*- sh -*--
##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##
## This file is part of ANTLR. See LICENSE.txt for licence  ##
## details. Written by W. Haefelinger - initial version by  ##
## R. Laren.                                                ##
## ...............Copyright (C) Wolfgang Haefelinger, 2004  ##
##                                                          ##
##xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx##

dnl
dnl ===============================================================
dnl A couple of Macros have been copied from the GNU Autoconf Macro 
dnl Archive: 
dnl            http://www.gnu.org/software/ac-archive
dnl ===============================================================
dnl

AC_DEFUN(
  [AC_CHECK_CLASSPATH],
  [
    test "x$CLASSPATH" = x && AC_MSG_ERROR(
      [CLASSPATH not set. Please set it to include the directory containing configure.])
    if test "x$CLASSPATH" = x; then
      :
    else
      echo "CLASSPATH set to:"
      echo "$CLASSPATH"
      echo "IMPORTANT: make sure the current directory containing configure"
      echo "is in the CLASSPATH"
    fi
  ]
)

#dnl /**
#dnl  * Test.java: used to test dynamicaly if a class exists.
#dnl  */
#dnl public class Test
#dnl {
#dnl
#dnl public static void
#dnl main( String[] argv )
#dnl {
#dnl     Class lib;
#dnl     if (argv.length < 1)
#dnl      {
#dnl             System.err.println ("Missing argument");
#dnl             System.exit (77);
#dnl      }
#dnl     try
#dnl      {
#dnl             lib = Class.forName (argv[0]);
#dnl      }
#dnl     catch (ClassNotFoundException e)
#dnl      {
#dnl             System.exit (1);
#dnl      }
#dnl     lib = null;
#dnl     System.exit (0);
#dnl }
#dnl
#dnl }

AC_DEFUN(
  [AC_CHECK_CLASS],[
    AC_REQUIRE([AC_PROG_JAVA])
    ac_var_name=`echo $1 | sed 's/\./_/g'`
    #dnl Normaly I'd use a AC_CACHE_CHECK here but since the variable name is
    #dnl dynamic I need an extra level of extraction
    AC_MSG_CHECKING([for $1 class])
    AC_CACHE_VAL(ac_cv_class_$ac_var_name,[
        if test x$ac_cv_prog_uudecode_base64 = xyes; then
          cat << \EOF > Test.uue
begin-base64 644 Test.class
yv66vgADAC0AKQcAAgEABFRlc3QHAAQBABBqYXZhL2xhbmcvT2JqZWN0AQAE
bWFpbgEAFihbTGphdmEvbGFuZy9TdHJpbmc7KVYBAARDb2RlAQAPTGluZU51
bWJlclRhYmxlDAAKAAsBAANlcnIBABVMamF2YS9pby9QcmludFN0cmVhbTsJ
AA0ACQcADgEAEGphdmEvbGFuZy9TeXN0ZW0IABABABBNaXNzaW5nIGFyZ3Vt
ZW50DAASABMBAAdwcmludGxuAQAVKExqYXZhL2xhbmcvU3RyaW5nOylWCgAV
ABEHABYBABNqYXZhL2lvL1ByaW50U3RyZWFtDAAYABkBAARleGl0AQAEKEkp
VgoADQAXDAAcAB0BAAdmb3JOYW1lAQAlKExqYXZhL2xhbmcvU3RyaW5nOylM
amF2YS9sYW5nL0NsYXNzOwoAHwAbBwAgAQAPamF2YS9sYW5nL0NsYXNzBwAi
AQAgamF2YS9sYW5nL0NsYXNzTm90Rm91bmRFeGNlcHRpb24BAAY8aW5pdD4B
AAMoKVYMACMAJAoAAwAlAQAKU291cmNlRmlsZQEACVRlc3QuamF2YQAhAAEA
AwAAAAAAAgAJAAUABgABAAcAAABtAAMAAwAAACkqvgSiABCyAAwSD7YAFBBN
uAAaKgMyuAAeTKcACE0EuAAaAUwDuAAasQABABMAGgAdACEAAQAIAAAAKgAK
AAAACgAAAAsABgANAA4ADgATABAAEwASAB4AFgAiABgAJAAZACgAGgABACMA
JAABAAcAAAAhAAEAAQAAAAUqtwAmsQAAAAEACAAAAAoAAgAAAAQABAAEAAEA
JwAAAAIAKA==
====
EOF
          if uudecode$EXEEXT Test.uue; then
            :
          else
            echo "configure: __oline__: uudecode had trouble decoding base 64 file 'Test.uue'" >&AC_FD_CC
            echo "configure: failed file was:" >&AC_FD_CC
            cat Test.uue >&AC_FD_CC
            ac_cv_prog_uudecode_base64=no
          fi
          rm -f Test.uue
          
          if AC_TRY_COMMAND($JAVA $JAVAFLAGS Test $1) >/dev/null 2>&1; then
            eval "ac_cv_class_$ac_var_name=yes"
          else
            eval "ac_cv_class_$ac_var_name=no"
          fi
          rm -f Test.class
        else
          AC_TRY_COMPILE_JAVA([$1], , 
            [eval "ac_cv_class_$ac_var_name=yes"],
            [eval "ac_cv_class_$ac_var_name=no"])
        fi
        eval "ac_var_val=$`eval echo ac_cv_class_$ac_var_name`"
        eval "HAVE_$ac_var_name=$`echo ac_cv_class_$ac_var_val`"
        HAVE_LAST_CLASS=$ac_var_val
        if test x$ac_var_val = xyes; then
          ifelse([$2], , :, [$2])
        else
          ifelse([$3], , :, [$3])
        fi
      ]
    )
    #dnl for some reason the above statment didn't fall though here?
    #dnl do scripts have variable scoping?
    eval "ac_var_val=$`eval echo ac_cv_class_$ac_var_name`"
    AC_MSG_RESULT($ac_var_val)
  ]
)

AC_DEFUN([AC_CHECK_JAVA_HOME],[
    AC_REQUIRE([AC_EXEEXT])dnl
    TRY_JAVA_HOME=`ls -dr /usr/java/* 2> /dev/null | head -n 1`
    if test x$TRY_JAVA_HOME != x; then
      PATH=$PATH:$TRY_JAVA_HOME/bin
    fi
    AC_PATH_PROG(JAVA_PATH_NAME, java$EXEEXT)
    if test x$JAVA_PATH_NAME != x; then
      JAVA_HOME=`echo $JAVA_PATH_NAME | sed "s/\(.*\)[[/]]bin[[/]]java$EXEEXT$/\1/"`
    fi;dnl
    ]
)


AC_DEFUN([AC_PROG_JAR],
  [
    AC_REQUIRE([AC_EXEEXT])dnl
    if test "x$JAVAPREFIX" = x; then
      test "x$JAR" = x && AC_CHECK_PROGS(JAR, jar$EXEEXT)
    else
      test "x$JAR" = x && AC_CHECK_PROGS(JAR, jar, $JAVAPREFIX)
    fi
    test "x$JAR" = x && AC_MSG_ERROR([no acceptable jar program found in \$PATH])
    AC_PROVIDE([$0])dnl
    ]
)

AC_DEFUN([AC_PROG_JAVA],[
    AC_REQUIRE([AC_EXEEXT])dnl
    if test x$JAVAPREFIX = x; then
      test x$JAVA = x && AC_CHECK_PROGS(JAVA, kaffe$EXEEXT java$EXEEXT)
    else
      test x$JAVA = x && AC_CHECK_PROGS(JAVA, kaffe$EXEEXT java$EXEEXT, $JAVAPREFIX)
    fi
    test x$JAVA = x && AC_MSG_ERROR([no acceptable Java virtual machine found in \$PATH])
    AC_PROG_JAVA_WORKS
    AC_PROVIDE([$0])dnl
    ]
)


#dnl /**
#dnl  * Test.java: used to test if java compiler works.
#dnl  */
#dnl public class Test
#dnl {
#dnl
#dnl public static void
#dnl main( String[] argv )
#dnl {
#dnl     System.exit (0);
#dnl }
#dnl
#dnl }

AC_DEFUN([AC_PROG_JAVA_WORKS],
  [
    AC_CHECK_PROG(uudecode, uudecode$EXEEXT, yes)
    if test x$uudecode = xyes; then
      AC_CACHE_CHECK([if uudecode can decode base 64 file], ac_cv_prog_uudecode_base64, [
          cat << \EOF > Test.uue
begin-base64 644 Test.class
yv66vgADAC0AFQcAAgEABFRlc3QHAAQBABBqYXZhL2xhbmcvT2JqZWN0AQAE
bWFpbgEAFihbTGphdmEvbGFuZy9TdHJpbmc7KVYBAARDb2RlAQAPTGluZU51
bWJlclRhYmxlDAAKAAsBAARleGl0AQAEKEkpVgoADQAJBwAOAQAQamF2YS9s
YW5nL1N5c3RlbQEABjxpbml0PgEAAygpVgwADwAQCgADABEBAApTb3VyY2VG
aWxlAQAJVGVzdC5qYXZhACEAAQADAAAAAAACAAkABQAGAAEABwAAACEAAQAB
AAAABQO4AAyxAAAAAQAIAAAACgACAAAACgAEAAsAAQAPABAAAQAHAAAAIQAB
AAEAAAAFKrcAErEAAAABAAgAAAAKAAIAAAAEAAQABAABABMAAAACABQ=
====
EOF
          if uudecode$EXEEXT Test.uue; then
            ac_cv_prog_uudecode_base64=yes
          else
            echo "configure: __oline__: uudecode had trouble decoding base 64 file 'Test.uue'" >&AC_FD_CC
            echo "configure: failed file was:" >&AC_FD_CC
            cat Test.uue >&AC_FD_CC
            ac_cv_prog_uudecode_base64=no
          fi
          rm -f Test.uue])
    fi
    if test x$ac_cv_prog_uudecode_base64 != xyes; then
      rm -f Test.class
      AC_MSG_WARN([I have to compile Test.class from scratch])
      if test x$ac_cv_prog_javac_works = xno; then
        AC_MSG_ERROR([Cannot compile java source. $JAVAC does not work properly])
      fi
      if test x$ac_cv_prog_javac_works = x; then
        AC_PROG_JAVAC
      fi
    fi
    AC_CACHE_CHECK(if $JAVA works, ac_cv_prog_java_works, [
        JAVA_TEST=Test.java
        CLASS_TEST=Test.class
        TEST=Test
        changequote(, )dnl
        cat << \EOF > $JAVA_TEST
/* [#]line __oline__ "configure" */
public class Test {
public static void main (String args[]) {
        System.exit (0);
} }
EOF
        changequote([, ])dnl
        if test x$ac_cv_prog_uudecode_base64 != xyes; then
          if AC_TRY_COMMAND($JAVAC $JAVACFLAGS $JAVA_TEST) && test -s $CLASS_TEST; then
            :
          else
            echo "configure: failed program was:" >&AC_FD_CC
            cat $JAVA_TEST >&AC_FD_CC
            AC_MSG_ERROR(The Java compiler $JAVAC failed (see config.log, check the CLASSPATH?))
          fi
        fi
        if AC_TRY_COMMAND($JAVA $JAVAFLAGS $TEST) >/dev/null 2>&1; then
          ac_cv_prog_java_works=yes
        else
          echo "configure: failed program was:" >&AC_FD_CC
          cat $JAVA_TEST >&AC_FD_CC
          AC_MSG_ERROR(The Java VM $JAVA failed (see config.log, check the CLASSPATH?))
        fi
        rm -fr $JAVA_TEST $CLASS_TEST Test.uue
        ])
    AC_PROVIDE([$0])dnl
  ]
)

AC_DEFUN([AC_PROG_JAVAC],
  [
    AC_REQUIRE([AC_EXEEXT])dnl
    if test "x$JAVAPREFIX" = x; then
      test "x$JAVAC" = x && AC_CHECK_PROGS(JAVAC, javac$EXEEXT "gcj$EXEEXT -C" guavac$EXEEXT jikes$EXEEXT)
    else
      test "x$JAVAC" = x && AC_CHECK_PROGS(JAVAC, javac$EXEEXT "gcj$EXEEXT -C" guavac$EXEEXT jikes$EXEEXT, $JAVAPREFIX)
    fi
    test "x$JAVAC" = x && AC_MSG_ERROR([no acceptable Java compiler found in \$PATH])
    AC_PROG_JAVAC_WORKS
    AC_PROVIDE([$0])dnl
    ]
)

AC_DEFUN([AC_PROG_JAVAC_WORKS],[
    AC_CACHE_CHECK([if $JAVAC works], ac_cv_prog_javac_works, [
        JAVA_TEST=Test.java
        CLASS_TEST=Test.class
        cat << \EOF > $JAVA_TEST
/* [#]line __oline__ "configure" */
public class Test {
}
EOF
        if AC_TRY_COMMAND($JAVAC $JAVACFLAGS $JAVA_TEST) >/dev/null 2>&1; then
          ac_cv_prog_javac_works=yes
        else
          AC_MSG_ERROR([The Java compiler $JAVAC failed (see config.log, check the CLASSPATH?)])
          echo "configure: failed program was:" >&AC_FD_CC
          cat $JAVA_TEST >&AC_FD_CC
        fi
        rm -f $JAVA_TEST $CLASS_TEST
        ])
    AC_PROVIDE([$0])dnl
    ])

AC_DEFUN([AC_TRY_COMPILE_JAVA],[
    AC_REQUIRE([AC_PROG_JAVAC])dnl
    cat << \EOF > Test.java
/* [#]line __oline__ "configure" */
ifelse([$1], , , [import $1;])
public class Test {
[$2]
}
EOF
    if AC_TRY_COMMAND($JAVAC $JAVACFLAGS Test.java) && test -s Test.class ; then
      #dnl Don't remove the temporary files here, so they can be examined.
    ifelse([$3], , :, [$3])
    else
    echo "configure: failed program was:" >&AC_FD_CC
    cat Test.java >&AC_FD_CC
    ifelse([$4], , , [  rm -fr Test*
        $4
        ])dnl
    fi
    rm -fr Test*
    ]
)

AC_DEFUN([AC_TRY_RUN_JAVA],[
    AC_REQUIRE([AC_PROG_JAVAC])dnl
    AC_REQUIRE([AC_PROG_JAVA])dnl
    cat << \EOF > Test.java
/* [#]line __oline__ "configure" */
ifelse([$1], , , [include $1;])
public class Test {
[$2]
}
EOF
    if AC_TRY_COMMAND($JAVAC $JAVACFLAGS Test.java) && test -s Test.class && ($JAVA $JAVAFLAGS Test; exit) 2>/dev/null
      then
#dnl Don't remove the temporary files here, so they can be examined.
      ifelse([$3], , :, [$3])
    else
      echo "configure: failed program was:" >&AC_FD_CC
      cat Test.java >&AC_FD_CC
      ifelse([$4], , , [  rm -fr Test*
          $4
          ])dnl
    fi
    rm -fr Test*])

#dnl#xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
#dnl#                      AX_TRY_COMPILE_JAVA
#dnl#xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
#dnl $1 => import section
#dnl $2 => class body section
#dnl $3 => if_good_action
#dnl $4 => if_fails_action [implicit action: candidate is removed from
#dnl       list]. This cannot be overridden by providing a action.

AC_DEFUN([AX_TRY_COMPILE_JAVA],
[
  ## make sure that we do not use an existing file
  i=0;cn="Test\${i}"; eval "fj=${cn}.java"
  while test -f "${fj}"
  do
    i=`expr $i + 1`
    eval "fj=${cn}.java"
  done  
  eval "fc=${cn}.class"
  eval "cn=${cn}"

  cat << [_ACEOF] > ${fj}
  [$1]
  public class ${cn} {
  [$2]
  }
[_ACEOF]
  ## wh: how do I check that a file has a non-zero size (test -s)
  ## wh: is not portable.
  if AC_TRY_COMMAND($JAVAC $JAVACFLAGS ${fj}) && test -f "${fc}"
  then  
    $3
  else
    ifelse([$4], ,[
    echo ""
    echo "@configure:__oline__: failed to compile java input ...."
    echo "======================================================="
    cat ${fj}
    echo "======================================================="
    echo "exec $JAVAC $JAVACFLAGS ${fj}"
    echo ""
    rm -rf "${fc}" "${fj}"
    ],[$4])
  fi
  rm -rf "${fc}" "${fj}"
  ## eof [AX_TRY_COMPILE_JAVA]
])dnl

#dnl AX_GNU_MAKE
#dnl $1->var that contains list of suitable candidates [not empty]
#dnl $2->action_if_not_found || empty
#dnl $3->action_if_found || empty
#dnl => $MAKE

AC_DEFUN(
  [AX_GNU_MAKE], 
  [
    #Search all the common names for GNU make
    ax_gnu_make_list="${[$1]}"
    [$1]=
    for a in . ${ax_gnu_make_list} ; do
      if test "$a" == "." ; then 
        continue
      fi
      AC_MSG_CHECKING([whether ${a} is GNU make])
      if (/bin/sh -c "$a --version" 2> /dev/null | grep GNU  2>&1 > /dev/null );  then
        [$1]="$a"
        AC_MSG_RESULT(yes)
        break
      else
        AC_MSG_RESULT(no)
      fi 
    done 
    ## handle search result 
    if test  "x${[$1]}" == "x"  ; then
      :
      $2
    else
      :
      $3
    fi
  ]
)dnl


###dnl Like AC_PATH_PROGS. However, each argument in $2 will be checked.
###dnl The result will be added to $1. There's no caching etc.
###dnl

AC_DEFUN(
  [AX_TYPE_DASHA],
  [
    for ac_prog in [$2] ; do
      set dummy $ac_prog; ac_word=${2}
      ## if argument is absolute we check whether such a file exists,
      ## otherwise we lookup PATH. Each hit will be added to main
      ## variable.
      case $ac_word in
        @<:@\\/@:>@* | ?:@<:@\\/@:>@*)      
          AC_MSG_CHECKING([for $ac_word])
          if test -f $ac_word ; then
            [$1]="${[$1]} ${ac_word}"
            AC_MSG_RESULT(yes)
          else
            AC_MSG_RESULT(no)
          fi
          ;;
        *)
          as_found=
          as_save_IFS=$IFS; IFS=$PATH_SEPARATOR
          for as_dir in $PATH
          do
           IFS=$as_save_IFS
           test -z "$as_dir" && as_dir=.
           for ac_exec_ext in '' $ac_executable_extensions; do
             if $as_executable_p "$as_dir/$ac_word$ac_exec_ext"; then
               [$1]="${[$1]} $as_dir/$ac_word$ac_exec_ext"
               AC_MSG_CHECKING([for $ac_word])
               AC_MSG_RESULT([$as_dir/$ac_word$ac_exec_ext])
               as_found=1
             fi
           done
          done
          test "x$as_found" == "x" && {
            AC_MSG_CHECKING([for $ac_word])
            AC_MSG_RESULT([no])
          }
          ;;
      esac
    done
  ]
)dnl

###dnl Like AC_PATH_PROGS but if <variable> is given, then it's argument
###dnl is taken unconditionally(?).
AC_DEFUN(
  [AX_PATH_PROGS],
  [
    ax_arg_list="[$2]"
    if test "x${[$1]}" != "x" ; then
      ax_arg_list="${[$1]}"
    fi
    [$1]=""
    AX_TYPE_DASHA([$1],[${ax_arg_list}])
    if test "x${[$1]}" != "x" ; then
      ifelse([$3], ,[:],$3)
    else
      ifelse([$4], ,[
          AC_MSG_ERROR([no suitable value has been found for [$1]])
          ],$4)
    fi
  ]
)


AC_DEFUN([AX_JAVAC],
[
  ## make sure that we do not use an existing file
  i=0;cn="Test\${i}"; eval "fj=${cn}.java"
  while test -f "${fj}"
  do
    i=`expr $i + 1`
    eval "fj=${cn}.java"
  done  
  eval "fc=${cn}.class"
  eval "cn=${cn}"

  cat << [_ACEOF] > ${fj}
  [$1]
  public class ${cn} {
  [$2]
  }
[_ACEOF]
  ## wh: how do I check that a file has a non-zero size (test -s)
  ## wh: is not portable.
  if AC_TRY_COMMAND($JAVAC $JAVACFLAGS ${fj}) && test -f "${fc}"
  then  
    $3
  else
    ifelse([$4], ,[
    echo ""
    echo "@configure:__oline__: failed to compile java input ...."
    echo "======================================================="
    cat ${fj}
    echo "======================================================="
    echo "exec $JAVAC $JAVACFLAGS ${fj}"
    echo ""
    rm -rf "${fc}" "${fj}"
    ],[$4])
  fi
  rm -rf "${fc}" "${fj}"
  ## eof [AX_TRY_COMPILE_JAVA]
])dnl

AC_DEFUN([AX_WHICH_JAVAC],[
    AC_SUBST([$1])
    if (/bin/sh -c "$JAVAC --version" 2>&1 | grep -i 'GCC' 2>&1 > /dev/null ) ; then
      [$1]=gcj
    elif (/bin/sh -c "$JAVAC --version" 2>&1 | grep -i 'jikes' 2>&1 > /dev/null ) ; then
      [$1]=jikes
    else
      [$1]=javac
    fi
  ]
)

AC_DEFUN([AX_VAR_HEAD],[
    set x ${[$1]}
    [$1]="${2}"
  ]
)

AC_DEFUN([AX_VAR_ADD],[
    ifelse([$3], ,,[$1=$3])
    $1="${[$1]} $2"
  ]
)


AC_DEFUN([AX_JAVA_PROGS],[
    case $LANG_JAVA in
      1)
        AX_PATH_PROGS([$1],[$2],[$3],[
            LANG_JAVA=0
            cat <<EOF

============================================================
Warning:
Support for JAVA has been disabled as I have not been able
locate to locate a mandatory program. Please change \$PATH or run 
with option --help on how to overcome this problem.
============================================================

EOF
          ]
        )
        ;;
    esac
  ]
)

AC_DEFUN([AX_PYTHON_PROGS],[
    case $LANG_PY in
      1)
        AX_PATH_PROGS([$1],[$2],[$3],[
            LANG_PY=0
            cat <<EOF

============================================================
Warning:
Support for Python has been disabled as I have not been able
to locate a mandatory program. Please change \$PATH or run 
with option --help on how to overcome this problem.
============================================================

EOF
          ]
        )
        ;;
    esac
  ]
)

AC_DEFUN([AX_CSHARP_PROGS],[
    case $LANG_CS in
      1)
        AX_PATH_PROGS([$1],[$2],[$3],[
            LANG_CS=0
            cat <<EOF

============================================================
Warning:
Support for C# has been disabled as I have not been able to
locate a mandatory program. Please change \$PATH or run 
with option --help on how to overcome this problem.
============================================================

EOF
          ]
        )
        ;;
    esac
  ]
)

AC_DEFUN([AX_CXX_PROGS],[
    case $LANG_CXX in
      1)
        AX_PATH_PROGS([$1],[$2],[$3],[
            LANG_CXX=0
            cat <<EOF

============================================================
Warning:
Support for C++ has been disabled as I have not been able to
locate a mandatory program. Please change \$PATH or run 
with option --help on how to overcome this problem.
============================================================

EOF
          ]
        )
        ;;
    esac
  ]
)

AC_DEFUN([AX_LANG_JAVA],[
    case $LANG_JAVA in
      1)
        ifelse([$1], ,[:],$1)
        ;;
    esac
  ]
)
AC_DEFUN([AX_LANG_CXX],[
    case $LANG_CXX in
      1)
        ifelse([$1], ,[:],$1)
        ;;
    esac
  ]
)
AC_DEFUN([AX_LANG_PYTHON],[
    case $LANG_PY in
      1)
        ifelse([$1], ,[:],$1)
        ;;
    esac
  ]
)
AC_DEFUN([AX_LANG_CSHARP],[
    case $LANG_CS in
      1)
        ifelse([$1], ,[:],$1)
        ;;
    esac
  ]
)

AC_DEFUN([AX_MSG_UNKOWN_CXX],[
    AC_MSG_WARN([
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
          U N K N O W N  C++ COMPILER:  ${cxx}  
============================================================
Compilation  is  very  likely to fail as we are not aware of
this compiler yet. In case of problems  please  try  to  set
additional flags by using environment variable CXXFLAGS.

If CXXFLAGS does not help you, please edit either

 ${srcdir}/scripts/cxx.sh.in   ; or
 ${srcdir}/scripts/link.sh.in

Those scripts are getting called for compilation of all C++
source code (cxx.sh.in) or for linking binaries (link.sh.in).

In very obscure cases, building the library may also fail.If
so, please try variable ARFLAGS or edit 

 ${srcdir}/scripts/lib.sh.in
  
============================================================

  *** PLEASE PROVIDE FEEDBACK TO antlr.org - THANK YOU ***

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
])
    ]
)


AC_DEFUN([AX_ARG_WITH],[
    AC_ARG_WITH(
      [$1],
      [AC_HELP_STRING(
          [--with-$1=ARG],
          [given argument will override variable $$2. For a detailed
            description of $$2 see below.])
        ],
      [[$2]="${withval}"]
    )
    AC_ARG_WITH(
      [$1flags],
      [AC_HELP_STRING(
          [--with-$1flags=ARG],
          [given argument will override variable $$2FLAGS. For a detailed
            description of $$2FLAGS see below.])
        ],
      [[$2FLAGS]="${withval}"]
    )
  ]
)


AC_DEFUN([AX_ARG_ENABLE],[
    $2=$4
    AC_ARG_ENABLE(
      [$1],
      [AC_HELP_STRING(
          [--enable-$1],
          [$3])
        ],[
        $2="${enableval}"
        case "${$2}" in
          no|0|false) $2=0;;
          * )         $2=1;;
        esac
      ]
    )
  ]
)


AC_DEFUN([AX_MAKEVARS_ADD],[
    cat >> "$[$1]" <<EOF
$2
EOF
  ]
)

AC_DEFUN([AX_BASENAME],[
    test -n "$1" && {
      $1=`basename ${$1}`
    }
  ]
)