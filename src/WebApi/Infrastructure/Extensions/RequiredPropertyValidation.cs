namespace OData.Sample.WebApi.Infrastructure.Extensions;

using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

/// <summary>
/// Übersetzt die im EF-Modell als Pflicht markierten Eigenschaften in Modellfehler. Ohne diese Prüfung scheitert ein POST
/// mit fehlendem Pflichtfeld erst in der Datenbank (NOT NULL) und endet als 500 statt als 400.
/// </summary>
public static class RequiredPropertyValidation
{
	public static void AddMissingRequiredProperties<TEntity>(this ModelStateDictionary modelState, DbContext context, TEntity entity)
		where TEntity : class
	{
		// Bei kaputtem JSON ist der Body null und ModelState bereits ungültig; dann gibt es nichts zu prüfen.
		var entityType = context.Model.FindEntityType(typeof(TEntity));
		if (entity is null || entityType is null)
		{
			return;
		}

		var missing = entityType.GetProperties()
			.Where(p => !p.IsNullable && !p.IsPrimaryKey() && p.ValueGenerated == ValueGenerated.Never)
			.Where(p => p.PropertyInfo is not null && p.ClrType == typeof(string))
			.Where(p => string.IsNullOrWhiteSpace((string?)p.PropertyInfo!.GetValue(entity)));

		foreach (var property in missing)
		{
			modelState.AddModelError(property.Name, $"{property.Name} ist ein Pflichtfeld.");
		}
	}
}
