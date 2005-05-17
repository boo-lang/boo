echo off

rem

rem Demonstrate how to subclass a grammar and actually get it to compile.

rem Biggest issue is making sure ANTLR, compiler, and interpreter can see

rem the supergrammar and classes needed by it.  In this case, the subgrammar

rem uses the supergrammar's lexer; it must be visible for the compiler and

rem interpreter.

rem

rem If you put a lexer here in this dir, you would not need all this

rem classpath crap.

rem

echo on

java antlr.Tool -glib "..\tinyc\tinyc.g" subc.g

javac -classpath c:\jdk1.1.7A\lib\classes.zip;.;..\tinyc;d:\Projects\antlr-2.6.0 *.java

echo off

rem

rem have to change the next classpath to point to your 2.6.0 directory not mine.

rem

echo on

java -classpath c:\jdk1.1.7A\lib\classes.zip;.;..\tinyc;d:\Projects\antlr-2.6.0 Main < input.c



