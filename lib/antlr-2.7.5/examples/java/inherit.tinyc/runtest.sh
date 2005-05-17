#
# Demonstrate how to subclass a grammar and actually get it to compile.
# Biggest issue is making sure ANTLR, compiler, and interpreter can see
# the supergrammar and classes needed by it.  In this case, the subgrammar
# uses the supergrammar's lexer; it must be visible for the compiler and
# interpreter.
#
# If you put a lexer here in this dir, you would not need all this
# classpath crap.
#
java antlr.Tool -glib "../tinyc/tinyc.g" subc.g
javac -classpath '/usr/local/src/jdk117_v3/lib/classes.zip:.:../tinyc:/home/parrt/depot/code/org.antlr/test/antlr-2.7.0a11' *.java
#
# have to change the next classpath to point to your 2.6.0 directory not mine.
#
java -classpath '/usr/local/src/jdk117_v3/lib/classes.zip:.:../tinyc:/home/parrt/depot/code/org.antlr/test/antlr-2.7.0a11' Main < input.c

