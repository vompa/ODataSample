namespace OData.Sample.Client;

using System;

using OData.Sample.Client.Commands;
using OData.Sample.Client.Commands.Abstract;

using Spectre.Console;

internal class Program
{
	private const string ServiceRoot = "https://localhost:7239/odata/v1";

	private static void Main(string[] args)
	{
		var commands = new IOdataCommand[]
		{
			new GetCountriesCommand(),
			new GetCountriesExpandedCommand(),
			new GetCountryCountByRegionCommand(),
			new AddCountryCommand(),
			new UpdateCountryCommand(),
			new DeleteCountryCommand()
		};

		AnsiConsole.WriteLine("Odata.Sample.Client");

		while (true)
		{
			AnsiConsole.Write(new Rule().RuleStyle("red dim"));
			AnsiConsole.WriteLine();

			// Menu
			for (var i = 0; i < commands.Length; i++)
			{
				AnsiConsole.WriteLine("{0}. {1}", i + 1, commands[i].CommandText);
			}

			IOdataCommand? command;
			do
			{
				command = Console.ReadKey().Key switch
				{
					ConsoleKey.D1 or ConsoleKey.NumPad1 => commands[0],
					ConsoleKey.D2 or ConsoleKey.NumPad2 => commands[1],
					ConsoleKey.D3 or ConsoleKey.NumPad3 => commands[2],
					ConsoleKey.D4 or ConsoleKey.NumPad4 => commands[3],
					ConsoleKey.D5 or ConsoleKey.NumPad5 => commands[4],
					ConsoleKey.D6 or ConsoleKey.NumPad6 => commands[5],
					_ => default,
				};
				ClearCurrentConsoleLine();
			}
			while (command is null);

			AnsiConsole.WriteLine();
			AnsiConsole.Write(new Rule().RuleStyle("red dim"));

			command.Execute(new Uri(ServiceRoot));
		}
	}

	private static void ClearCurrentConsoleLine()
	{
		var currentLineCursor = Console.CursorTop;
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write(new string(' ', Console.WindowWidth));
		Console.SetCursorPosition(0, currentLineCursor);
	}
}
