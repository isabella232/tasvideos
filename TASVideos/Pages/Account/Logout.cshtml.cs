﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TASVideos.Core.Services;

namespace TASVideos.Pages.Account
{
	[Authorize]
	public class LogoutModel : BasePageModel
	{
		private readonly SignInManager _signInManager;
		private readonly UserManager _userManager;

		public LogoutModel(
			SignInManager signInManager,
			UserManager userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public async Task<IActionResult> OnPost()
		{
			var user = await _userManager.GetUserAsync(User);
			await _userManager.RemoveClaimsAsync(user, User.Claims);
			await _signInManager.SignOutAsync();
			return Login();
		}
	}
}
