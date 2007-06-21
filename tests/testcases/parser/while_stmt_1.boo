random = System.Random()
number = random.Next(100)
guess = 100 # trocar por -1 e fazer o parser funcionar

while true:
	guess = int.Parse(prompt("adivinhe o número: "))
	print("não, é maior") if number > guess
	print("não, é menor") if number < guess
	break if guess == number
	
print("o número era ${number}!")
