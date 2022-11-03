namespace OData.Sample.Client.Extensions;

using System;
using System.Text.Json;

using Microsoft.OData.Client;

using Spectre.Console;

internal static class AnsiConsoleExtensions
{
	public enum SaveOperation
	{
		AddEntity = 0,
		UpdateEntity = 1,
		DeleteEntity = 2,
	}

	internal static IAnsiConsole WriteLogMessage(this IAnsiConsole console, string message, string helplink = "")
	{
		AnsiConsole.MarkupLine($"[grey]{DateTime.Now:HH:mm:ss}:[/] {message}[grey][/]");

		AnsiConsole.WriteLine();

		if (!string.IsNullOrWhiteSpace(helplink))
		{
			AnsiConsole.Markup($"[link={helplink}]Microsoft Help[/]");
			AnsiConsole.WriteLine();
		}

		return console;
	}

	internal static IAnsiConsole WriteDataServiceResponse<T>(this IAnsiConsole console, DataServiceResponse responses, SaveOperation opt) where T : class
	{
		foreach (var response in responses)
		{
			var changeResponse = (ChangeOperationResponse)response;
			var entityDescriptor = (EntityDescriptor)changeResponse.Descriptor;
			var countryUpdated = (T)entityDescriptor.Entity; // the created entity

			AnsiConsole.WriteLine($"{opt} - Response");
			_ = AnsiConsole.Console.WriteJson(changeResponse);

			AnsiConsole.WriteLine();
			AnsiConsole.WriteLine($"{opt} - Context");
			_ = AnsiConsole.Console.WriteJson(countryUpdated);
		}

		var helplink = "https://learn.microsoft.com/en-us/odata/client/get-response-content";
		AnsiConsole.WriteLine();
		AnsiConsole.Markup($"[link={helplink}]Microsoft Help[/]");
		AnsiConsole.WriteLine();

		return console;
	}

	internal static IAnsiConsole WriteJson<TValue>(this IAnsiConsole console, TValue value, JsonSerializerOptions? options = null, JsonStyle? jsonStyle = null)
	{
		ArgumentNullException.ThrowIfNull(console);
		ArgumentNullException.ThrowIfNull(value);

		var node = JsonSerializer.SerializeToElement(value, options);
		_ = console.WriteJson(node, jsonStyle ?? JsonStyle.Default, 0);
		return console;
	}

	internal static IAnsiConsole WriteJson(this IAnsiConsole console, JsonElement node, JsonStyle? jsonStyle = null)
	{
		ArgumentNullException.ThrowIfNull(console);
		ArgumentNullException.ThrowIfNull(node);

		_ = console.WriteJson(node, jsonStyle ?? JsonStyle.Default, 0);
		return console;
	}

	internal static IAnsiConsole WriteJson(this IAnsiConsole console, JsonElement node, JsonStyle jsonStyle, int indentionLevel)
	{
		switch (node.ValueKind)
		{
			case JsonValueKind.Undefined:
				break;

			case JsonValueKind.Object:
				var indent = new string(' ', indentionLevel * jsonStyle.IndentSize);
				var firstPropertyWritten = false;
				using (var properties = node.EnumerateObject())
				{
					foreach (var property in properties)
					{
						if (!firstPropertyWritten)
						{
							console.Write(new Text("{", jsonStyle.CurlyBracketStyle));
							console.WriteLine();
						}
						else
						{
							console.Write(new Text(",", jsonStyle.ValueSeparatorStyle));
							console.WriteLine();
						}

						console.Write(indent + "  \"");
						console.Write(new Text(property.Name, jsonStyle.NameStyle));
						console.Write("\"");
						console.Write(new Text(": ", jsonStyle.NameSeparatorStyle));
						_ = console.WriteJson(property.Value, jsonStyle, indentionLevel + 1);
						firstPropertyWritten = true;
					}
				}
				if (firstPropertyWritten)
				{
					console.WriteLine();
					console.Write(indent);
					console.Write(new Text("}", jsonStyle.CurlyBracketStyle));
				}
				else
				{
					console.Write(new Text("{}", jsonStyle.CurlyBracketStyle));
				}

				break;

			case JsonValueKind.Array:
				var indentArray = new string(' ', (indentionLevel + 1) * jsonStyle.IndentSize);
				var firstPropertyWritten2 = false;
				using (var child_properties = node.EnumerateArray())
				{
					foreach (var child in child_properties)
					{
						if (!firstPropertyWritten2)
						{
							console.Write(new Text("[", jsonStyle.SquareBracketStyle));
							console.WriteLine();
						}
						else
						{
							console.Write(new Text(",", jsonStyle.ValueSeparatorStyle));
							console.WriteLine();
						}

						console.Write(indentArray);
						_ = console.WriteJson(child, jsonStyle, indentionLevel + 1);
						firstPropertyWritten2 = true;
					}
				}
				if (firstPropertyWritten2)
				{
					console.WriteLine();
					console.Write(indentArray[..^2]);
					console.Write(new Text("]", jsonStyle.SquareBracketStyle));
				}
				else
				{
					console.Write(new Text("[]", jsonStyle.SquareBracketStyle));
				}
				break;

			case JsonValueKind.String:
				console.Write(new Text("\"" + node.GetString() + "\"", jsonStyle.StringStyle));
				break;

			case JsonValueKind.Number:
				console.Write(new Text(node.GetRawText(), jsonStyle.NumberStyle));
				break;

			case JsonValueKind.True:
				console.Write(new Text("true", jsonStyle.BooleanStyle));
				break;

			case JsonValueKind.False:
				console.Write(new Text("false", jsonStyle.BooleanStyle));
				break;

			case JsonValueKind.Null:
				console.Write(new Text("null", jsonStyle.NullStyle));
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(node.ValueKind), "undefined node value");
		}

		if (indentionLevel == 0)
		{
			console.WriteLine();
		}

		return console;
	}

	internal record JsonStyle
	{
		public static readonly JsonStyle Default = new();

		public int IndentSize { get; init; } = 2;

		public Style NameStyle { get; init; } = new(Color.LightSkyBlue1);
		public Style StringStyle { get; init; } = new(Color.LightPink3);
		public Style NumberStyle { get; init; } = new(Color.DarkSeaGreen2);
		public Style NullStyle { get; init; } = new(Color.SkyBlue3);
		public Style BooleanStyle { get; init; } = new(Color.SkyBlue3);
		public Style CurlyBracketStyle { get; init; } = new(Color.Grey82);
		public Style SquareBracketStyle { get; init; } = new(Color.Grey82);
		public Style NameSeparatorStyle { get; init; } = new(Color.Grey82);
		public Style ValueSeparatorStyle { get; init; } = new(Color.Grey82);
	}
}
