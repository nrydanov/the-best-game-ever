﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BL;
using JustADude.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JustADude.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public GameController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GameIndex()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Create()
        {
            GameBL.Create(User.Identity.Name);
            return RedirectToAction("Start", "Game");
        }

        [HttpGet]
        public IActionResult Start()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Join()
        {
            return View();
        }

        [HttpGet]
        public void SubmitJoin(int gameId)
        {
            GameBL.Join(User.Identity.Name, gameId);
        }


        [HttpGet]
        public string List()
        {
            var result = JsonConvert.SerializeObject(GameBL.GetGames());
            return result;
        }

        [HttpGet]
        public string Update(string json)
        {
            if (!User.Identity.IsAuthenticated) return "";

            if (json == null) return "";

            IList<int> keys = JsonConvert.DeserializeObject<List<int>>(json);
            var objs = GameBL.Update(User.Identity.Name, keys);

            var response_json = JsonConvert.SerializeObject(objs);

            return response_json;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}