"""
Passo 1: resolver using
	* using System:
		* NamespaceNameResolver: todos os tipos com ns igual a System em
			todos os assemblies referenciados;
	* using System.IO:
		* NamespaceNameResolver: idem para System.IO;
	* using Boo.IO
		* NamespaceNameResolver: idem para Boo.IO;
		
Passo 2: atributos e mixins
	* nada a fazer	
	
Passo 3: resolver referências
	* começando pelos métodos
	* def ScanFile:
		* for
			* TextFile é resolvido para o tipo Boo.IO.TextFile
				* MethodInvocationExpression => ComputedAST.ConstructorInvocation
			* enumerate => Boo.Lang.Builtins.enumerate(IEnumerable)
				* MethodInvocationExpression => ComputedAST.StaticMethodInvocation
			* index, line
				* declara as variáveis locais ao for com tipo object, object
			* print => Boo.Lang.Builtins.print(string)
	* UnpackStatement (linha 9)
		* resolve expressão à direita:
			* MethodInvocationExpression => ComputedAST.StaticMethodInvocation:
				* MemberReferenceExpression 
					* Environment:ReferenceExpression => ComputedAST.TypeReference
		* resolve cada uma das declarações à esquerda
			* declara variáveis no escopo atual (módulo)
	* o resto é muito semelhante ao que aconteceu no outro for
			
Passo 4: normalizar/otimizar a ast
	* o for na linha 6 se transforma em um while no iterator
		* o equivalente a um UnpackStatement é colocado no início do bloco
		* try/finally é colocado em volta do for para dar Dispose no iterator
		* a sentença na linha 7 se transforma em bloco if
		* o resultado final é algo assim:
			__iterator0 = Boo.Lang.Builtins.enumerate(Boo.IO.TextFile(fname))
			while __iterator0.MoveNext():
				// begin UnpackStatement
				__iterator1 = Boo.Lang.Builtins.iterator(__iterator0.Current)
				index = Boo.Lang.Runtime.Unpacking.UnpackNext(__iterator1);
				line = Boo.Lang.Runtime.Unpacking.UnpackNext(__iterator1);
				// end UnpackStatement
				if Boo.Lang.Runtime.Regex.IsMatch(line, expression):
					__temp0 = string.Format("{0}({1}): {2}", fname, index, line)
					print(__temp0)
			
	* o UnpackStatement na linha 9 é traduzido para algo como (já que é baseado em array):
		__GetCommandLineArgsResult = Environment.GetCommandLineArgs()
		Boo.Lang.Runtime.Unpacking.CheckLength(__GetCommandLineArgsResult, 2);
		fspec = __GetCommandLineArgsResult[0]
		expression = __GetCommandLineArgsResult[1]		
	* a sentença na linha 11 se transforma em bloco if	
	* o for na linha 7 se transforma em um while indexado (já que é baseado em array):
			__ifname = 0
			__GetFilesResult = Directory.GetFiles(".")
			while __ifname < __GetFilesResult.Length:
				fname = __GetFilesResult[__ifname]
				if Boo.Lang.Runtime.Regex.IsMatch(fname, fspec):
					ScanFile(fname, expression)
				++__ifname
				
Passo 5: geração de IL
	* módulo é gerado como uma classe private abstract sealed grepModule;
	* variáveis globais são campos;
	* sentenças globais são colocadas em um entry point public static void Main();
"""
using System // Environment
using System.IO // Directory
using Boo.IO // TextFile

def ScanFile(fname as string, expression as string):	
	for index, line in enumerate(TextFile(fname)):
		print("${fname}(${index}): ${line}") if line =~ expression

_, glob, expression = Environment.GetCommandLineArgs()
for fname in Directory.GetFiles(".", glob):
	ScanFile(fname, expression)

