namespace OData.Sample.Client.Commands.Abstract;

using System;

internal interface IOdataCommand
{
	string CommandText { get; }

	void Execute(Uri serviceRoot);
}
