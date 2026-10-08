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

public class CountriesController : ODataController
{
	private readonly ODataSampleContext _context;

	public CountriesController(ODataSampleContext context)
		=> _context = context;

	[EnableQuery]
	public IQueryable<Country> Get() =>
		_context.Countries!.AsNoTracking();

	[EnableQuery]
	public SingleResult<Country> Get([FromODataUri] int key)
	{
		var result = _context.Countries!
			.Where(m => m.Id == key)
			.Select(m => m)
			.AsNoTracking();
		return SingleResult.Create(result);
	}

	[AcceptVerbs("POST", "PUT")]
	public async Task<IActionResult> PostAsync([FromBody] Country entity)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		_context.Countries!.Add(entity);

		await _context.SaveChangesAsync();
		return Created(entity);
	}

	[AcceptVerbs("PATCH", "PUT")]
	public async Task<IActionResult> PatchAsync([FromODataUri] int key, [FromBody] Delta<Country> entity)
	{
		if (entity is null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var existingEntity = await _context.Countries!
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
		var existingEntity = await _context.Countries!
			.FirstOrDefaultAsync(e => e.Id == key);

		if (existingEntity == null)
		{
			return NotFound();
		}

		_context.Countries!
			.Remove(existingEntity);

		await _context.SaveChangesAsync();
		return StatusCode(StatusCodes.Status204NoContent);
	}

	private bool EntityExists(int id)
	{
		return _context.Countries!.Any(p => p.Id == id);
	}
}
