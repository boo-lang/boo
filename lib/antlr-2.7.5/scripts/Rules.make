###############################################################################
# $Id:$
###############################################################################
#
# Rules.make :
#  General make rules, supporting
#   -compilation of .cpp to .o, .cpp to .s and .s to .o
#   -combining of multiple .o files into one single .o file
#   -creation of .a archives from multiple .o files
#   -recursive subdirectories
#   -recursive (dist)clean
#   -creation of binary executables
#   -doxygen generated docs
#

.EXPORT_ALL_VARIABLES:

#
# Depending on the definition of these variables in submakefiles different
# targets get build.
# NOTE: These should not be exported so we unexport them right here.
#
unexport SUBDIRS			# specifying all subdirs from $(TOPDIR)
unexport SUB_DIRS       # specifying all subdirs to make
unexport ALL_SUB_DIRS   # specifying all subdirs to process (dep/clean)

unexport O_TARGET			# name of combined .o file to generate
unexport O_OBJS			# name of .o files to combine into $(O_TARGET)

unexport L_TARGET			# name of .a archive to generate
unexport L_OBJS			# name of .o files to place in $(L_TARGET)

unexport B_NAME			# name of binary to generate
unexport B_OBJS			# name of .o files to combine into $(B_NAME)

unexport GCJ_B_NAME		# Rules for gcj (native java binaries)
unexport GCJ_B_OBJS

unexport DOXY_TARGET		# name of a doxygen config file to build docs from
unexport DOXY_GENDIR    # name of dir doxygen generates docs into

unexport JAR_TARGETS		# jar files..
unexport JAR_DEST			# jar files..

unexport G_FILES        # antlr .g files
unexport G_TAG_FILES		# empty file to record compiling .g files
unexport G_TARGETS		# per-Makefile list of ANTLR generated files

unexport C_TARGETS		# name of additional targets to "clean"
unexport DC_TARGETS		# name of additional targets to "distclean"

#
# Implicit rules
#
%.s:	%.cpp
	$(CXX) $(CXXFLAGS) $(EXTRA_CXXFLAGS) -S $< -o $@
#
# These rules support buiding the .o files into a obj_dir different than the
# current dir. This keeps the source dirs nice and clean..
# FIXME: VPATH?
#
ifdef obj_dir
OBJDIR_DEP := $(obj_dir)objects
else
OBJDIR_DEP :=
obj_dir :=
endif

$(obj_dir)%.o:	%.cpp $(OBJDIR_DEP)
	$(CXX) -c -o $@ $< $(CXXFLAGS) $(SHARED_FLAGS)

$(obj_dir)%.o: %.c
	$(CC) $(CFLAGS) $(SHARED_FLAGS) -c -o $@ $<

$(obj_dir)%.o: %.s
	$(AS) $(ASFLAGS) -o $@ $<

#
# Automatic dependency generation rules
#
$(obj_dir)%.d: %.c $(OBJDIR_DEP)
	@echo "Building dependencies for $<"
	@$(CC) -MM -MG $(CFLAGS) $< | $(SED) -e 's,\($*\)\.o[ :]*,$(obj_dir)\1.o $@ : ,g' > $@
$(obj_dir)%.d: %.cpp $(OBJDIR_DEP)
	@echo "Building dependencies for $<"
	@$(CXX) -MM -MG $(CXXFLAGS) $< | $(SED) -e 's,\($*\)\.o[ :]*,$(obj_dir)\1.o $@ : ,g' > $@
$(obj_dir)%.d: %.g $(OBJDIR_DEP) ;
	@echo "Building dependencies for $<"
	@echo "$($(addsuffix _FILES,$(subst .,_,$<))): $(obj_dir).$*.g ;" > $@

#
# How to make .x.g files from .g files. (ANTLR)
# A .x.g file is an empty target to record running ANTLR on x.g
# Customize flags per file 'x.g' by setting x_FLAGS
# The chmod is dirty but it makes life a lot easier with perforce
#
$(obj_dir).%.g: %.g $(OBJDIR_DEP);
	@ -$(CHMOD) -f u+w $($(addsuffix _FILES, $(subst .,_,$^))) 2> /dev/null
	$(ANTLR) $(ANTLR_FLAGS) $($*_FLAGS) $^
	@ $(TOUCH) $@

ifdef G_FILES
# make list of the sentinel files of the .g files
G_TARGETS := $(addprefix .,$(G_FILES))
endif

#
# How to build class files
#
# Take along existing CLASSPATH definition. Necessary for jikes.
ifdef JAVAC_CLASSPATH
 ifneq ($(JAVAC_CLASSPATH),)
  javac_paths= -classpath $(call fix_path,$(TOPDIR)$(PATH_SEPARATOR)$(JAVAC_CLASSPATH))
 else
  javac_paths	= -classpath $(call fix_path,$(TOPDIR))
 endif
endif

#
# For native binary java
#
%.o:  %.java
ifdef VERBOSE
	$(GCJ) -c -o $@ $< -classpath $(TOPDIR) $(GCJFLAGS) $(EXTRA_GCJFLAGS)
else
	@ echo "Building native binary for $<"
	@ $(GCJ) -c -o $@ $< -classpath $(TOPDIR) $(GCJFLAGS) $(EXTRA_GCJFLAGS)
endif

#
# For class files
#
%.class: %.java
ifdef VERBOSE
	$(JAVAC) $(JAVAC_FLAGS) $(javac_paths) $(obj_dir_arg) $<
else
	@ echo "Building java bytecode for $<"
	@ $(JAVAC) $(JAVAC_FLAGS) $(javac_paths) $(obj_dir_arg) $<
endif
unexport obj_dir_arg javac_paths

#
# Note: this stuff is just too annoying to use.... Now do it in the
# submakefile itself
#
#  How to build a Java jar file.
#
#  Note: The jar contents are taken from the rule dependency
#  list; thus the user must explicitly define dependencies per
#  jar target.  E.g.
#     x.jar : $(x_jar_FILES)
#  Make performs variable expansion before implicit rule search,
#  hence, the desired implicit rule dependency
#	$($(subst .,_,$%)_jar_FILES)
#  is an undefined variable, resulting in an empty dependency
#  list.
#
#%.jar:
#	@ echo "==========================================="
#	@ echo "Making $(JAR_DEST)/$(@)..."
#	@ rm -f $@
#	@ (cd $(JAR_DEST) $(JAR) cf $(JAR_DEST)/$@ $(subst $$,\$$,$^)
#	@ echo "==========================================="

#
# Get things started:
# 1. Build ANTLR generated files, subdirectories.
# 2. Remaining TARGETS
#
first_rule: sub_dirs
	$(MAKE) all_targets

unexport TARGETS
#
# Build default targets
#
all_targets: $(G_TARGETS) $(O_TARGET) $(L_TARGET) $(GCJ_B_NAME) $(B_NAME) $(JAR_TARGETS) $(GCJ_B_NAME)

#
# Compile a set of .o files into one .o file
#
ifdef O_TARGET
$(O_TARGET): $(O_OBJS)
	-$(RM) -f $@
ifneq "$(strip $(O_OBJS))" ""
	$(LD) $(EXTRA_LDFLAGS) -r -o $@ $(O_OBJS)
else
	$(AR) rus $@
endif
endif

#
# Compile a set of .o files into one .a file
#
ifdef L_TARGET
$(L_TARGET): $(L_OBJS)
	-$(RM) -f $@
	$(AR) $(EXTRA_ARFLAGS) $(ARFLAGS) $@ $(L_OBJS)
	$(RANLIB) $@
endif

#
# This takes care of creating obj_dirs's
#
ifdef obj_dir
$(OBJDIR_DEP): ;
	$(MKDIR) -p $(obj_dir)
	$(TOUCH) $(OBJDIR_DEP)
endif

ifeq ($(ANTLR_WIN32),"yes")
ifdef DLL_TARGET
$(DLL_TARGET): $(DLL_OBJS)
	-$(RM) -f $@
	$(CXX) -o $@ -Wl,-mdll $(L_OBJS)
endif
endif

ifdef DOXY_TARGET
gen_doc: $(DOXY_TARGET) ;
ifdef DOXY_GENDIR
ifneq ($(DOXY_GENDIR),)
	-$(RM) -f $(DOXY_GENDIR)/*
endif
endif
ifneq ($(DOXYGEN),"")
	$(DOXYGEN) -f $(DOXY_TARGET)
else
	echo "Doxygen not installed skipping $(DOXY_TARGET)"
endif
endif

# Rule to make subdirs
#
.PHONY: $(SUB_DIRS) sub_dirs
sub_dirs: $(SUB_DIRS)
ifdef SUB_DIRS
ifneq ($(strip $(SUB_DIRS)),)
$(SUB_DIRS):
	@echo ">>>>>>>>>>>>>>>>>>>> Entering $@ ..."
	$(MAKE) -C $@
	@echo "<<<<<<<<<<<<<<<<<<<< Leaving $@"
endif
endif

#
# Rule to make binaries
#

ifdef B_NAME
$(B_NAME): $(B_OBJS)
	-$(RM) -f $@
	$(CXX) -o $@ $(EXTRA_LDFLAGS) $(B_OBJS) $(EXTRA_LIBS)
endif

ifdef GCJ_B_NAME
$(GCJ_B_NAME): $(GCJ_B_OBJS)
	@ -$(RM) -f $@
	$(GCJ) $(GCJFLAGS) $(EXTRA_GCJFLAGS) -o $@ --main=antlr.Tool $(GCJ_B_OBJS)
endif

#
# Include dependency files if they exist (and we're not cleaning)
#
ifneq (clean,$(findstring clean,$(MAKECMDGOALS)))
-include .depend
ifneq ($(SOURCE),)
-include $(addprefix $(obj_dir),$(SOURCE:.cpp=.d))
endif
ifneq ($(SOURCES),)
-include $(addprefix $(obj_dir),$(SOURCES:.cpp=.d))
endif
ifneq ($(CXXSOURCE),)
-include $(addprefix $(obj_dir),$(CXXSOURCE:.cpp=.d))
endif
ifneq ($(GCJSOURCES),)
-include $(GCJSOURCES:.java=.d)
endif
ifneq ($(CSOURCE),)
-include $(addprefix $(obj_dir),$(CSOURCE:.c=.d))
endif
ifneq ($(G_FILES),)
-include $(addprefix $(obj_dir),$(G_FILES:.g=.d))
endif
endif

#
#  Rule to bootstrap from external ANTLR jar.
#
.PHONY: bootstrap_g
bootstrap_g: ANTLR := $(ANTLR_BOOTSTRAP)
bootstrap_g: ANTLR_FLAGS := $(ANTLR_BOOTSTRAP_FLAGS)
bootstrap_g: $(G_TAG_FILES)
ifdef ALL_SUB_DIRS
	@set -e; for i in $(ALL_SUB_DIRS); do $(MAKE) -C $$i bootstrap_g; done
endif

#
#  Rule to clean ANTLR generated files (corresponding
#  to bootstrap_g targets).
#
.PHONY: clean_g
clean_g:
ifdef ALL_SUB_DIRS
	@set -e; for i in $(ALL_SUB_DIRS); do $(MAKE) -C $$i clean_g; done
endif
	-$(RM) -f $(G_TAG_FILES) $(G_TARGETS)

#
# Rule to remove all objects, cores, etc.; leaving
# ANTLR generated and config files.
#
.PHONY: mostlyclean
mostlyclean:
ifdef obj_dir
ifneq ($(obj_dir),)
	-$(RM) -f $(obj_dir)/*
endif
endif
ifdef DOXY_GENDIR
ifneq ($(DOXY_GENDIR),)
	-$(RM) -f $(DOXY_GENDIR)/*
endif
endif
ifdef ALL_SUB_DIRS
	set -e; for i in $(ALL_SUB_DIRS); do $(MAKE) -C $$i mostlyclean; done
endif
	-$(RM) -f *.o *.class *.a *.so core *.s $(B_NAME) $(C_TARGETS) $(JAR_TARGETS)

#
# Rule to remove all objects, cores, ANTLR generated, etc.;
# leaving configure generated files.
#
.PHONY: clean
clean: mostlyclean clean_g
ifdef obj_dir
# make sure to do nothing if obj_dir empty...
ifneq ($(obj_dir),)
	@-test -d $(obj_dir) && $(RM) -f $(obj_dir)/* $(obj_dir)/.*.g
	@-test -d $(obj_dir) && $(RMDIR) $(obj_dir)
endif
endif
ifdef ALL_SUB_DIRS
	set -e; for i in $(ALL_SUB_DIRS); do $(MAKE) -C $$i clean; done
endif

#
# Rule to remove all objects, cores, ANTLR generated,
# configure generated, etc.; leaving files requiring
# additional programs to generate (e.g., autoconf).
#
# FIXME: can not be called more than once successively because
# FIXME: it removes files unconditionally included by subdirectory
# FIXME: Makefiles (e.g., Config.make).
#
.PHONY: distclean
distclean: clean
ifdef ALL_SUB_DIRS
	set -e; for i in $(ALL_SUB_DIRS); do $(MAKE) -C $$i distclean; done
endif
	-$(RM) -f .depend $(DC_TARGETS)

#
#  Install rule
#
ifndef OVERRULE_INSTALL
.PHONY: install
install:
ifdef B_NAME
	@echo "Installing $(B_NAME) into $(bindir)"
	@test -d $(DESTDIR)$(bindir) || $(MKDIR) -p $(DESTDIR)$(bindir)
	@$(INSTALL) -m 755 $(B_NAME) $(DESTDIR)$(bindir)
endif
ifdef L_TARGET
	@echo "Installing $(L_TARGET) into $(libdir)"
	@test -d $(DESTDIR)$(libdir) || $(MKDIR) -p $(DESTDIR)$(libdir)
	@$(INSTALL) -m 644 $(L_TARGET) $(DESTDIR)$(libdir)
endif
ifdef JAR_TARGETS
	@test -d $(DESTDIR)$(datadir)/$(versioneddir) || $(MKDIR) -p $(DESTDIR)$(datadir)/$(versioneddir)
	@for i in $(JAR_TARGETS); do \
		echo "Installing $i into $(datadir)/$(versioneddir)" \
		$(INSTALL) -m 644 $$i $(DESTDIR)$(datadir)/$(versioneddir) ;\
	done
endif
ifdef INSTALL_TARGETS
	@test -d $(DESTDIR)$(INSTALL_DIR) || $(MKDIR) -p $(DESTDIR)$(INSTALL_DIR)
	@for i in $(INSTALL_TARGETS); do \
		echo "Installing $$i into $(INSTALL_DIR)" ; \
		$(INSTALL) -m $(INSTALL_MODE) $$i $(DESTDIR)$(INSTALL_DIR) > /dev/null ;\
	done
endif
ifdef ALL_SUB_DIRS
	@set -e; for i in $(ALL_SUB_DIRS); do $(MAKE) -C $$i install; done
endif
endif
