namespace OData.Sample.WebApi.Controller;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

using OData.Sample.WebApi.Domain.Entities;
using OData.Sample.WebApi.Infrastructure.Db;

public class CountryRegionsController : ODataController
{
	private readonly ODataSampleContext _context;

	public CountryRegionsController(ODataSampleContext context)
		=> _context = context;

	[EnableQuery(PageSize = 10)]
	public IQueryable<CountryRegion> Get() =>
		_context.CountryRegions!.AsNoTracking();

	[EnableQuery]
	public SingleResult<CountryRegion> Get([FromODataUri] int key)
	{
		var result = _context.CountryRegions!
			.Where(m => m.Id == key)
			.Select(m => m)
			.AsNoTracking();
		return SingleResult.Create(result);
	}

	[AcceptVerbs("POST", "PUT")]
	public async Task<IActionResult> PostAsync([FromBody] CountryRegion entity)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		_context.CountryRegions!.Add(entity);

		await _context.SaveChangesAsync();
		return Created(entity);
	}

	[AcceptVerbs("PATCH", "PUT")]
	public async Task<IActionResult> PatchAsync([FromODataUri] int key, [FromBody] Delta<CountryRegion> entity)
	{
		if (entity is null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var existingEntity = await _context.CountryRegions!
			.FirstOrDefaultAsync(e => e.Id == key);

		if (existingEntity == null)
		{
			return NotFound();
		}

		entity.Patch(existingEntity);
		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!EntityExists(key))
			{
				return NotFound();
			}
			else
			{
				throw;
			}
		}
		return Updated(existingEntity);
	}

	[AcceptVerbs("DELETE")]
	public async Task<IActionResult> DeleteAsync([FromODataUri] int key)
	{
		var existingEntity = await _context.CountryRegions!
			.FirstOrDefaultAsync(e => e.Id == key);

		if (existingEntity == null)
		{
			return NotFound();
		}

		_context.CountryRegions!
			.Remove(existingEntity);

		await _context.SaveChangesAsync();
		return StatusCode(StatusCodes.Status204NoContent);
	}

	private bool EntityExists(int id)
	{
		return _context.CountryRegions!.Any(p => p.Id == id);
	}
}
