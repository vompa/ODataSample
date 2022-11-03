namespace OData.Sample.WebApi.Infrastructure.Extensions;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.UriParser;

using OData.Sample.WebApi.Domain.Entities;

///<Summary>
///<see href="https://devblogs.microsoft.com/odata/compute-and-search-in-asp-net-core-odata-8/">devblogs.microsoft.com</see>
///</Summary>
public class CountrySearchBinder : QueryBinder, ISearchBinder
{
	private static readonly Dictionary<string, int> WorldRegionSearch = new()
	{
		{"LaenderInAfrika", 903 },
		{"LaenderInCaribic", 904 },
		{"LaenderInAmerika", 905 },
		{"LaenderInEuropa", 908 },
		{"LaenderInOzeanien", 909 },
		{"LaenderInAsien", 935 }
	};

	public Expression BindSearch(SearchClause searchClause, QueryBinderContext context)
	{
		if (searchClause is null)
		{
			throw new ArgumentNullException(nameof(searchClause));
		}

		if (context is null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		if (searchClause.Expression is SearchTermNode node)
		{
			// Lambda expression methods:
			if (WorldRegionSearch.ContainsKey(node.Text))
			{
				Expression<Func<Country, bool>> exp = p => p.WorldRegionId == WorldRegionSearch[node.Text];
				return exp;
			}
			throw new InvalidOperationException("unknown search");
		}
		else
		{
			// Linq expression tree methods:
			var exp = BindSingleValueNode(searchClause.Expression, context);

			var lambdaExp = Expression.Lambda(exp, context.CurrentParameter);

			return lambdaExp;
		}
	}
}

