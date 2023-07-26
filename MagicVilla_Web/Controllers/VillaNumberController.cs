﻿using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace MagicVilla_Web.Controllers
{
	public class VillaNumberController : Controller
	{
		private readonly IVillaNumberService _villaNumberService;
		private readonly IVillaService _villaService;
		private readonly IMapper _mapper;
		public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService, IMapper mapper)
		{
			_villaNumberService = villaNumberService;
			_villaService = villaService;
			_mapper = mapper;
		}
		public async Task<IActionResult> IndexVillaNumber()
		{
			List<VillaNumberDTO> list = new();

			var response = await _villaNumberService.GetAllSync<APIResponse>();
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}

		[Authorize(Roles = "admin")]
		public async Task<IActionResult> CreateVillaNumber()
		{
			VillaNumberCreateVM villaNumberVM = new();
			var response = await _villaService.GetAllSync<APIResponse>();
			if (response != null && response.IsSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villaNumberVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Villa Number created successfully";
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					TempData["error"] = "Error encountered";
					if (response.ErrorMessages.Count > 0)
					{
						ModelState.AddModelError("ErrorMessage", response.ErrorMessages.FirstOrDefault());
					}
				}
			}

			var resp = await _villaService.GetAllSync<APIResponse>();
			if (resp != null && resp.IsSuccess)
			{
				model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(resp.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(model);
		}

		[Authorize(Roles = "admin")]
		public async Task<IActionResult> UpdateVillaNumber(int villaNo)
		{
			VillaNumberUpdateVM villaNumberVM = new();
			var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
			if (response != null && response.IsSuccess)
			{
				VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
				villaNumberVM.VillaNumber =_mapper.Map<VillaNumberUpdateDTO>(model);
			}

		    response = await _villaService.GetAllSync<APIResponse>();
			if (response != null && response.IsSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				return View(villaNumberVM);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Villa Number updated successfully";
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					TempData["error"] = "Error encountered";
					if (response.ErrorMessages.Count > 0)
					{
						ModelState.AddModelError("ErrorMessage", response.ErrorMessages.FirstOrDefault());
					}
				}
			}

			var resp = await _villaService.GetAllSync<APIResponse>();
			if (resp != null && resp.IsSuccess)
			{
				model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(resp.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(model);
		}

		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteVillaNumber(int villaNo)
		{
			VillaNumberDeleteVM villaNumberVM = new();
			var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
			if (response != null && response.IsSuccess)
			{
				VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
				villaNumberVM.VillaNumber = model;
			}

			response = await _villaService.GetAllSync<APIResponse>();
			if (response != null && response.IsSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				return View(villaNumberVM);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
		{
			var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Villa Number delete successfully";
				return RedirectToAction(nameof(IndexVillaNumber));
			}
			TempData["error"] = "Error encountered";
			return View(model);
		}
	}
}
