using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
	//[Route("api/[controler]")]
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private readonly MagicVillaContext _context;
		private readonly IMapper _mapper;
		public VillaAPIController(MagicVillaContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
		{
			IEnumerable<Villa> villaList = await _context.Villas.ToListAsync();
			return Ok(_mapper.Map<List<VillaDTO>>(villaList));
		}

		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(200, Type=typeof(VillaDTO))]
		//[ProducesResponseType(404)]
		//[ProducesResponseType(400)]
		public async Task<ActionResult<VillaDTO>> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = await _context.Villas.FirstOrDefaultAsync(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<VillaDTO>(villa));
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
		{
			//if (!ModelState.IsValid)
			//{
			//	return BadRequest(ModelState);
			//}
			if (await _context.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa already Exists!");
				return BadRequest(ModelState);
			}

			if (createDTO == null)
			{
				return BadRequest(createDTO);
			}
			//if (villaDTO.Id > 0)
			//{
			//	return StatusCode(StatusCodes.Status500InternalServerError);
			//}
			Villa model = _mapper.Map<Villa>(createDTO);

			//Villa model = new()
			//{
			//	Amenity = createDTO.Amenity,
			//	Details = createDTO.Details,
			//	ImageUrl = createDTO.ImageUrl,
			//	Name = createDTO.Name,
			//	Occupancy = createDTO.Occupancy,
			//	Rate = createDTO.Rate,
			//	Sqft = createDTO.Sqft
			//};

			await _context.Villas.AddAsync(model);
			await _context.SaveChangesAsync();

			return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
		}

		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = await _context.Villas.FirstOrDefaultAsync(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}
			_context.Villas.Remove(villa);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
		{
			if (updateDTO == null || id != updateDTO.Id)
			{
				return BadRequest();
			}

			Villa model = _mapper.Map<Villa>(updateDTO);

			_context.Villas.Update(model);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

			VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

			if (villa == null)
			{
				return BadRequest();
			}
			patchDTO.ApplyTo(villaDTO, ModelState);

			Villa model = _mapper.Map<Villa>(villaDTO);

			_context.Villas.Update(model);
			await _context.SaveChangesAsync();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return NoContent();
		}
	}
}
