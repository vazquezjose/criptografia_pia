using System.Text;

namespace PIACriptografia;

public class Program
{
	static readonly string ENGLISH_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	static readonly string SPANISH_ALPHABET = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";

	static string FormatSpanishMessage(string message)
	{
		StringBuilder result = new(message);
		Dictionary<char, char> replacements = new()
		{
			{ 'Á', 'A' },
			{ 'É', 'E' },
			{ 'Í', 'I' },
			{ 'Ó', 'O' },
			{ 'Ú', 'U' }
		};

		for (int i = 0; i < message.Length; i++)
		{
			if (replacements.ContainsKey(result[i]))
			{
				char replacement;
				if (replacements.TryGetValue(result[i], out replacement))
				{
					result[i] = replacement;
				}
			}
		}
		return result.ToString();
	}

	static bool IsValidTextInAlphabet(string message, string alphabet)
	{
		if (message.Length == 0)
			return false;

		foreach (var c in message)
			if (!alphabet.Contains(c))
				return false;
		return true;
	}

	static string CaesarCipherMessage(string message, string alphabet, int key)
	{
		string result = string.Empty;
		int alphabetLength = alphabet.Length;

		foreach (var c in message)
		{
			int originalMessageCharIndex = alphabet.IndexOf(c);
			int encodedMessageCharIndex = originalMessageCharIndex + (key % alphabetLength);

			if (encodedMessageCharIndex >= alphabetLength)
				encodedMessageCharIndex -= alphabetLength;
			result += alphabet[encodedMessageCharIndex];
		}
		return result;
	}

	static string CaesarDecipherMessage(string message, string alphabet, int key)
	{
		string result = string.Empty;
		int alphabetLength = alphabet.Length;

		foreach (var c in message)
		{
			int encodedMessageCharIndex = alphabet.IndexOf(c);
			int originalMessageCharIndex = encodedMessageCharIndex - (key % alphabetLength);

			if (originalMessageCharIndex < 0)
				originalMessageCharIndex += alphabetLength;
			result += alphabet[originalMessageCharIndex];
		}
		return result;
	}

	static string AdjustVigenereKey(int messageLength, int keyLength, string key)
	{
		int difference = messageLength - keyLength;

		for (int i = 0; i < difference; i++)
			key += key[i];  // there's probably a better looking and more readable method to do this, but this one is safe
		return key;
	}

	static string VigenereCipherMessage(string message, string alphabet, string key)
	{
		string result = string.Empty;
		int alphabetLength = alphabet.Length;

		if (message.Length > key.Length)
			key = AdjustVigenereKey(message.Length, key.Length, key);

		for (int i = 0; i < message.Length; i++)
		{
			int originalMessageCharIndex = alphabet.IndexOf(message[i]);
			int keyCharIndex = alphabet.IndexOf(key[i]);
			int encodedMessageCharIndex = (originalMessageCharIndex + keyCharIndex) % alphabetLength;

			result += alphabet[encodedMessageCharIndex];
		}
		return result;
	}

	static string VigenereDecipherMessage(string message, string alphabet, string key)
	{
		StringBuilder result = new();
		int alphabetLength = alphabet.Length;

		if (message.Length > key.Length)
			key = AdjustVigenereKey(message.Length, key.Length, key);

		for (int i = 0; i < message.Length; i++)
		{
			int encodedMessageCharIndex = alphabet.IndexOf(message[i]);
			int keyCharIndex = alphabet.IndexOf(key[i]);
			int originalMessageCharIndex = (encodedMessageCharIndex - keyCharIndex) % alphabetLength;

			if (originalMessageCharIndex < 0)
				originalMessageCharIndex += alphabetLength;

			result.Append(alphabet[originalMessageCharIndex]);
		}

		return result.ToString();
	}

	static void Print(string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
	{
		Console.ForegroundColor = color;
		if (newLine)
			Console.WriteLine(message);
		else
			Console.Write(message);
		Console.ResetColor();
	}

	static void Main()
	{
		bool choseToExitMenu = false;
		char menuOption;
		do
		{
			Console.Clear();
			Print("PIA | CRIPTOGRAFIA", ConsoleColor.Cyan);
			Print("MENÚ PRINCIPAL", ConsoleColor.Yellow);
			Print("Opciones:");
			Print("A. Cifrado de César");
			Print("B. Cifrado de Vigènere");
			Print("C. Ver información");
			Print("Z. Salir");

			menuOption = Console.ReadKey(true).KeyChar;

			string input;
			switch (menuOption)
			{
				case 'A':
					Console.Clear();
					Print("CIFRADO DE CÉSAR", ConsoleColor.Cyan);
					Print("MENÚ", ConsoleColor.Yellow);
					Print("Opciones:");
					Print("A. Cifrar");
					Print("B. Descifrar");
					Print("Otra. Salir");

					int caesarCipherMenuOption = Console.ReadKey(true).KeyChar;

					switch (caesarCipherMenuOption)
					{
						case 'A':
						case 'B':
							Console.Clear();
							if (caesarCipherMenuOption == 'A')
								Print("CIFRAR MENSAJE USANDO EL MÉTODO DE CÉSAR", ConsoleColor.Cyan);
							else
								Print("DESCIFRAR MENSAJE USANDO EL MÉTODO DE CÉSAR", ConsoleColor.Cyan);

							char chosenAlphabet;
							bool validAlphabet;
							do
							{
								Print("Escoja un alfabeto", ConsoleColor.Yellow);
								Print("Opciones:");
								Print("A. Inglés (26 letras)");
								Print("B. Español (27 letras)");
								chosenAlphabet = Console.ReadKey().KeyChar;

								validAlphabet = chosenAlphabet == 'A' || chosenAlphabet == 'B';

								if (!validAlphabet)
								{
									Print("\nLa opción ingresada es inválida. Ingrese cualquier tecla para intentar de nuevo.", ConsoleColor.Red);
									Console.ReadKey(true);
								}
							} while (!validAlphabet);
							string alphabet = chosenAlphabet == 'A' ? ENGLISH_ALPHABET : SPANISH_ALPHABET;

							int key;
							bool validKey;
							do
							{
								Print("\nIntroduzca la clave: ", ConsoleColor.White, false);

								input = Console.ReadLine();

								validKey = int.TryParse(input, out key) && key != 0 && key < alphabet.Length;
								if (!validKey)
								{
									Print("La clave ingresada es inválida. Ingrese cualquier tecla para intentar de nuevo.", ConsoleColor.Red);
									Console.ReadKey(true);
								}
							} while (!validKey);

							bool validMessage;
							do
							{
								Print("\nIntroduzca el mensaje: ", ConsoleColor.White, false);
								input = FormatSpanishMessage(Console.ReadLine().ToUpper());
								
								validMessage = IsValidTextInAlphabet(input, alphabet) && input.Length > 0;
								if (validMessage)
								{
									if (caesarCipherMenuOption == 'A')
									{
										Print("Mensaje cifrado: ", ConsoleColor.Green, false);
										Print(CaesarCipherMessage(input, alphabet, key));
									}
									else
									{
										Print("Mensaje descifrado: ", ConsoleColor.Green, false);
										Print(CaesarDecipherMessage(input, alphabet, key));
									}
									Print("Presione cualquier tecla para regresar al menú principal.", ConsoleColor.Yellow);
								}
								else
								{
									Print("El mensaje introducido es inválida. Ingrese cualquier tecla para intentar de nuevo.", ConsoleColor.Red);
								}
								Console.ReadKey(true);
							} while (!validMessage);
							break;
					}
					break;
				case 'B':
					Console.Clear();
					Print("CIFRADO DE VIGENÈRE", ConsoleColor.Cyan);
					Print("MENÚ", ConsoleColor.Yellow);
					Print("Opciones:");
					Print("A. Cifrar");
					Print("B. Descifrar");
					Print("Otra. Salir");

					int vigenereCipherMenuOption = Console.ReadKey(true).KeyChar;

					switch (vigenereCipherMenuOption)
					{
						case 'A':
						case 'B':
							Console.Clear();
							if (vigenereCipherMenuOption == 'A')
								Print("CIFRAR MENSAJE USANDO EL MÉTODO DE VIGENÈRE", ConsoleColor.Cyan);
							else
								Print("DESCIFRAR MENSAJE USANDO EL MÉTODO DE VIGENÈRE", ConsoleColor.Cyan);

							char chosenAlphabet;
							bool validAlphabet;
							do
							{
								Print("Escoja un alfabeto", ConsoleColor.Yellow);
								Print("Opciones:");
								Print("A. Inglés (26 letras)");
								Print("B. Español (27 letras)");
								chosenAlphabet = Console.ReadKey().KeyChar;

								validAlphabet = chosenAlphabet == 'A' || chosenAlphabet == 'B';

								if (!validAlphabet)
								{
									Print("\nLa opción ingresada es inválida. Ingrese cualquier tecla para intentar de nuevo.", ConsoleColor.Red);
									Console.ReadKey(true);
								}
							} while (!validAlphabet);
							string alphabet = chosenAlphabet == 'A' ? ENGLISH_ALPHABET : SPANISH_ALPHABET;

							string key = string.Empty;
							bool validKey;
							do
							{
								Print("\nIntroduzca la clave: ", ConsoleColor.White, false);

								input = FormatSpanishMessage(Console.ReadLine().ToUpper());

								validKey = IsValidTextInAlphabet(input, alphabet) && input.Length > 0;
								if (!validKey)
								{
									Print("La clave ingresada es inválida. Ingrese cualquier tecla para intentar de nuevo.", ConsoleColor.Red);
									Console.ReadKey(true);
								}
								else
									key = input;
							} while (!validKey);

							bool validMessage;
							do
							{
								Print("\nIntroduzca el mensaje: ", ConsoleColor.White, false);
								input = FormatSpanishMessage(Console.ReadLine().ToUpper());

								validMessage = IsValidTextInAlphabet(input, alphabet) && input.Length > 0;
								if (validMessage)
								{
									if (vigenereCipherMenuOption == 'A')
									{
										Print("Mensaje cifrado: ", ConsoleColor.Green, false);
										Print(VigenereCipherMessage(input, alphabet, key));
									}
									else
									{
										Print("Mensaje descifrado: ", ConsoleColor.Green, false);
										Print(VigenereDecipherMessage(input, alphabet, key));
									}
									Print("Presione cualquier tecla para regresar al menú principal.", ConsoleColor.Yellow);
								}
								else
								{
									Print("El mensaje introducido es inválida. Ingrese cualquier tecla para intentar de nuevo.", ConsoleColor.Red);
								}
								Console.ReadKey(true);
							} while (!validMessage);
							break;
					}
					break;
				case 'C':
					Console.Clear();
					Console.WriteLine("El cifrado César es un cifrado de sustitución donde cada letra es reemplazada");
					Console.WriteLine("por otra ubicada más delante en el alfabeto. Este método debe su nombre a Julio");
					Console.WriteLine("César, que lo usaba para comunicarse con sus generales.");
					Console.WriteLine("=");
					Console.WriteLine("El cifrado Vigenère es un cifrado de sustitución que usa varios cifrados César");
					Console.WriteLine("en secuencia con diferentes valores de desplazamiento para esconder con éxito");
					Console.WriteLine("el contenido. Fue propuesto por Blaise de Vigenère en el siglo XVI.");
					Console.ReadKey(true);
					break;
				case 'Z':
					choseToExitMenu = true;
					break;
			}
		} while (!choseToExitMenu);
	}
}