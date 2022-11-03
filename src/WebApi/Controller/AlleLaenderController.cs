namespace OData.Sample.WebApi.Controller;

using System.Collections.Generic;
using System.Threading.Tasks;

using OData.Sample.WebApi.Domain.Entities;
using OData.Sample.WebApi.Infrastructure.Db;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class AlleLaenderController : ControllerBase
{
	private readonly ODataSampleContext _context;

	public AlleLaenderController(ODataSampleContext context)
		=> _context = context;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Country>>> GetAsync() =>
		await _context.Countries!.ToListAsync();

	[HttpGet("{key:int}")]
	public async Task<ActionResult<Country?>> GetAsync([FromODataUri] int key) =>
		await _context.Countries!.FirstOrDefaultAsync(e => e.Id == key);

}
