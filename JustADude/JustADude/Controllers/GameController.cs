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
        public async void SubmitJoin(int gameId)
        {
            await GameBL.Join(User.Identity.Name, gameId);
        }


        [HttpGet]
        public async Task<string> List()
        {
            var result = JsonConvert.SerializeObject(await GameBL.GetGames());
            return result;
        }

        private bool convertBitToFlag(string mask, int index)
        {
            return mask[index] != '0';
        }
        
        [HttpPost]
        public void Update(string mask)
        {
            // TODO: Return to error page
            if (!User.Identity.IsAuthenticated)
                return;
            
            var keys = new Dictionary<string, bool>();
            
            keys["ArrowUp"] = convertBitToFlag(mask, 0);
            keys["ArrowDown"] = convertBitToFlag(mask, 1);
            keys["ArrowRight"] = convertBitToFlag(mask, 2);
            keys["ArrowLeft"] = convertBitToFlag(mask, 3);
            
            var name = User.Identity.Name;
            
            
            GameBL.Update(name, keys);
        }

        [HttpGet]
        public async Task<string> Update()
        {
            // TODO: Return to error page
            if (!User.Identity.IsAuthenticated) 
                return "";

            var name = User.Identity.Name;

            var objs = await GameBL.GetObjects(name);

            return JsonConvert.SerializeObject(objs); 
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