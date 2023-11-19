using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using System.Net.Http;

namespace RedNit_Airline.Controllers
{
    public class AccountController : Controller
    {
        private readonly string apiKey = "AIzaSyDtqKA4GQeRhQXaxGRr8rj4XFfyWoffngY";
        private readonly string signUpUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp";
        private readonly string signInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword";

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(string email, string password)
        {
            try
            {
                await SignUpAsync(email, password);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"User registration failed: {ex.Message}";
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                await SignInAsync(email, password);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Authentication failed: {ex.Message}";
                return View();
            }
        }

        private async Task SignUpAsync(string email, string password)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent($"{{\"email\": \"{email}\", \"password\": \"{password}\", \"returnSecureToken\": true}}");
                var response = await client.PostAsync($"{signUpUrl}?key={apiKey}", content);
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task SignInAsync(string email, string password)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent($"{{\"email\": \"{email}\", \"password\": \"{password}\", \"returnSecureToken\": true}}");
                var response = await client.PostAsync($"{signInUrl}?key={apiKey}", content);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}