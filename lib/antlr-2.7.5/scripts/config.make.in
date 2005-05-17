##xxxxxxxxxxxxxxxxxxxxxx --*- Makefile -*-- xxxxxxxxxxxxxxxx>>> config.make
## By default, make will jump into any sub directory that contains a   file 
## named "Makefile". This is done  in the order implied by "/bin/ls"  which
## is in almost  all  cases  correct  (note  that  you  should  not  design 
## Makefiles which depend on a specific invocation order). You can override
## specific  behaviour  by  using  variable SUBDIRS. If given and not empty,
## "/bin/ls" is not used. Also,  if you want to disable jumping subdirs you
## may  use  either ".", ".."  as  value for SUBDIRS. Note that SUBDIRS may 
## contain any directory (except "." and "..).

## This is very much  GNU specific, sigh.  Variable SUBDIRS is used to tell 
## make which  subdirectory to jump. It's  value is normally preset or just
## empty, in which case /bin/ls is used as discussed above. However, I also
## want that a user can say
##
##  make SUBDIRS="d1 d2 .. dn"
##
## That means, ignore defaults and go ahead and make exactly this director-
## ies mentioned. Of course, this should only have  an  impact  on Makefile
## being used  by  "make"  but not for any makefils in d1 .. dn, right? For
## example, if di  needs  to  make directories a,b and c, then they need to
## be made of course. So all burns down to the question how to prevent a
## variable from being passed to submakes. Below you can see the answer. If
## you believe that there's a simpler answer to the problem don't hesistate
## to try it out. If  it  works, send me an email: ora dot et dot labora at
## web dot de. But be warned - you need to try all variations.
##
## Here is in short what  I  found  and how  it  works.  Variables given on 
## command line  are  saved  in  variable  MAKEOVERRIDES. This  variable is 
## exported  and  passed down. On  invocation  of a submake file, make will
## have a  look  into MAKEOVERRIDES  and unpack each variable found therein.
## Therefore I'm just going to  remove  every (?) occurance of SUBDIRS from
## this variable. 
MAKEOVERRIDES := $(patsubst SUBDIRS=%,,$(MAKEOVERRIDES))


## The actuall rule on how to make a recursive target.
all clean distclean test install force-target clean-target :: 
	@dirs="$(SUBDIRS)" ; \
	test -z "$${dirs}" && { \
		dirs=`/bin/ls` ; \
  } ; \
	for d in . $${dirs} ; do \
		case $${d} in \
		. | .. ) ;; \
	  $(SUBDIRS_NOT) ) ;; \
		*) \
		if test -f "$${d}/Makefile" ; then \
		  echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" ; \
		  echo ">> $(MAKE) -C $(subdir)/$${d} $@                              " ; \
			echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" ; \
			$(MAKE) -C "$$d" $@ || exit 1 ;\
		fi ; \
		;; \
		esac ; \
	done

## For historical  reasons only you can make local targets as "this-*" or 
## "*-this" rules. The  default is to do nothing. Although this targets
## exists, it is recommended to define further "all", "clean" etc. double
## colon rules.
 
all       :: this-all all-this
clean     :: this-clean clean-this
distclean :: this-distclean distclean-this
test      :: this-test test-this
install   :: this-install install-this

this-all ::
this-clean ::
this-distclean ::
this-test ::
this-install ::

all-this ::
clean-this ::
distclean-this ::
test-this ::
install-this ::

force-target :: clean-target all

## xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx<< config.make 
