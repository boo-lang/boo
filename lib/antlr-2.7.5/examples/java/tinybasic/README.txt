
 This is a simple Basic Parser/Interpreter I put together in 3 days.
 Has NOT been tested, only for instructional value.
 Do whatever you do with it, I don't care....
 
 Sinan Karasu <sinan.karasu@boeing.com>

% make
% javac -classpath $(ANTLR_HOME)/antlr.jar:. *.java
% java -classpath $(ANTLR_HOME)/antlr.jar:../:. tinybasic.Main try.bas
% java -classpath $(ANTLR_HOME)/antlr.jar:../:. tinybasic.Main try1.bas
% java -classpath $(ANTLR_HOME)/antlr.jar:../:. tinybasic.Main try2.bas

Terence Parr notes:
	Needs swing 1.1 to run. (the javax one)
	I converted package to be tinybasic instead of tb
	I updated this to work with 2.7.0 (changed a few exception type names)
	I did the following:

	$ cd ~/antlr-2.7.0/examples/tinybasic
	$ java antlr.Tool TinyBasic.g
	$ java antlr.Tool TinyBasicTreeWalker.g
	$ javac -classpath "$CLASSPATH:.." *.java
	$ java -classpath "$CLASSPATH:.." tinybasic.Main try.bas
	Parsing...
	   /home/parrt/projects/antlr.private/resources/tinybasic.try.bas



	6	6	To Java Programmer	hello
	7	7	To Java Programmer	hello
	24
	1	3
	2	4
	3	5
	Yes it works!tinybasic.DTExitModuleException: Done folks
	$ 

	And so on...

	Terence says: Pretty cool!  Great work Sinan!
