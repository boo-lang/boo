"""
Passo 1: resolver using

Passo 2: resolver atributos e mixins

Passo 3: resolver nomes e métodos
	* um visitor navega cada sentença
	* BinaryExpression
		* navega nó à direita
			* MethodInvocationExpression
				* prompt é resolvido para Boo.Lang.Builtins.prompt(string)
					* Target:ReferenceExpression => ComputedAST.StaticMethodReference
						* MethodInfo:
								* DeclaringType: Boo.Lang.Builtins
								* 
		* navega nó à esquerda
			* ReferenceExpression
				* nome não existe, nada a fazer
		* nó à esquerda não foi resolvido
			* declara a nova variável no escopo atual (módulo) com o tipo da expressão à direita
			* name:ReferenceExpression => ComputedAST.GlobalVariableReference
	* MethodInvocationExpression
		* print é resolvido para Boo.Lang.Builtins.print(string)
			* Target:ReferenceExpression => ComputedAST.StaticMethodReference
		* Argumentos:
			* StringFormattingExpression
				* Argumentos:
					* name:ReferenceExpression => ComputedAST.GlobalVariableReference
					
Passo 4: normalizar/otimizar a ast
					
Passo 5: geração de IL
	* módulo é gerado como uma classe private abstract sealed helloModule;
	* variáveis globais são campos;
	* sentenças globais são colocadas em um entry point public static void Main();
"""
name = prompt("Seu nome? ")
print("Olá, ${name}!")
