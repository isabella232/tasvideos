﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using TASVideos.Core;
using TASVideos.Data;
using TASVideos.Data.Entity;
using TASVideos.Data.Entity.Game;
using TASVideos.Extensions;
using TASVideos.Pages.Games.Models;

namespace TASVideos.Pages.Games
{
	[RequirePermission(PermissionTo.CatalogMovies)]
	public class ListModel : BasePageModel
	{
		private readonly ApplicationDbContext _db;

		public ListModel(
			ApplicationDbContext db)
		{
			_db = db;
		}

		[FromQuery]
		public GameListRequest Search { get; set; } = new ();

		public SystemPageOf<GameListModel> Games { get; set; } = SystemPageOf<GameListModel>.Empty();

		public List<SelectListItem> SystemList { get; set; } = new ();

		public async Task OnGet()
		{
			Games = await GetPageOfGames(Search);
			SystemList = await _db.GameSystems
				.ToDropdown()
				.ToListAsync();

			SystemList.Insert(0, new SelectListItem { Text = "All", Value = "" });
		}

		public async Task<IActionResult> OnGetFrameRateDropDownForSystem(int systemId, bool includeEmpty)
		{
			var items = await _db.GameSystemFrameRates
				.ForSystem(systemId)
				.ToDropDown()
				.ToListAsync();

			if (includeEmpty)
			{
				items = UiDefaults.DefaultEntry.Concat(items).ToList();
			}

			return new PartialViewResult
			{
				ViewName = "_DropdownItems",
				ViewData = new ViewDataDictionary<IEnumerable<SelectListItem>>(ViewData, items)
			};
		}

		public async Task<IActionResult> OnGetGameDropDownForSystem(int systemId, bool includeEmpty)
		{
			var items = await _db.Games
				.ForSystem(systemId)
				.OrderBy(g => g.DisplayName)
				.Select(g => new SelectListItem
				{
					Value = g.Id.ToString(),
					Text = g.DisplayName
				})
				.ToListAsync();

			if (includeEmpty)
			{
				items = UiDefaults.DefaultEntry.Concat(items).ToList();
			}

			return new PartialViewResult
			{
				ViewName = "_DropdownItems",
				ViewData = new ViewDataDictionary<IEnumerable<SelectListItem>>(ViewData, items)
			};
		}

		public async Task<IActionResult> OnGetRomDropDownForGame(int gameId, bool includeEmpty)
		{
			var items = await _db.GameRoms
				.Where(r => r.GameId == gameId)
				.OrderBy(r => r.Name)
				.Select(r => new SelectListItem
				{
					Value = r.Id.ToString(),
					Text = r.Name
				})
				.ToListAsync();

			if (includeEmpty)
			{
				items = UiDefaults.DefaultEntry.Concat(items).ToList();
			}

			return new PartialViewResult
			{
				ViewName = "_DropdownItems",
				ViewData = new ViewDataDictionary<IEnumerable<SelectListItem>>(ViewData, items)
			};
		}

		private async Task<SystemPageOf<GameListModel>> GetPageOfGames(GameListRequest paging)
		{
			var data = await _db.Games
				.ForSystemCode(paging.SystemCode)
				.Select(g => new GameListModel
				{
					Id = g.Id,
					DisplayName = g.DisplayName,
					SystemCode = g.System!.Code
				})
				.SortedPageOf(paging);

			return new SystemPageOf<GameListModel>(data)
			{
				SystemCode = paging.SystemCode,
				PageSize = data.PageSize,
				CurrentPage = data.CurrentPage,
				RowCount = data.RowCount,
				Sort = data.Sort
			};
		}
	}
}
