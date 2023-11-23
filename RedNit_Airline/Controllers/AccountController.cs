using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using System.Net.Http;
using Google.Cloud.Firestore;
using RedNit_Airline.Models;

namespace RedNit_Airline.Controllers
{
    public class AccountController : Controller
    {
        //private readonly string apiKey = "AIzaSyDtqKA4GQeRhQXaxGRr8rj4XFfyWoffngY";
        //private readonly string signUpUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp";
        //private readonly string signInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword";

        //public ActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Register(string email, string password)
        //{
        //    try
        //    {
        //        await SignUpAsync(email, password);
        //        return RedirectToAction("Login");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = $"User registration failed: {ex.Message}";
        //        return View();
        //    }
        //}

        //public ActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Login(string email, string password)
        //{
        //    try
        //    {
        //        await SignInAsync(email, password);
        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = $"Authentication failed: {ex.Message}";
        //        return View();
        //    }
        //}

        //private async Task SignUpAsync(string email, string password)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var content = new StringContent($"{{\"email\": \"{email}\", \"password\": \"{password}\", \"returnSecureToken\": true}}");
        //        var response = await client.PostAsync($"{signUpUrl}?key={apiKey}", content);
        //        response.EnsureSuccessStatusCode();
        //    }
        //}

        //private async Task SignInAsync(string email, string password)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var content = new StringContent($"{{\"email\": \"{email}\", \"password\": \"{password}\", \"returnSecureToken\": true}}");
        //        var response = await client.PostAsync($"{signInUrl}?key={apiKey}", content);
        //        response.EnsureSuccessStatusCode();
        //    }
        //}

        private FirestoreDb firestoreDb;

        public AccountController()
        {
            firestoreDb = FirestoreDb.Create("dvmb-16215");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string gmail, string matKhau)
        {
            var loggedInCustomer = AuthenticateAndGetCustomer(gmail, matKhau);

            if (loggedInCustomer != null)
            {
                // Lưu Document ID vào session
                Session["LoggedInCustomerId"] = loggedInCustomer.KhachHangID;
                Session["NameKH"] = loggedInCustomer.HoTen;
                // Redirect hoặc xử lý logic sau khi đăng nhập thành công
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Xử lý khi đăng nhập không thành công
                ViewBag.ErrorMessage = "Đăng nhập không thành công. Vui lòng kiểm tra lại Gmail và Mật khẩu.";
                return View("Login");
            }
        }

        private KhachHang AuthenticateAndGetCustomer(string gmail, string matKhau)
        {
            var khachHangCollection = firestoreDb.Collection("KhachHang");

            // Thực hiện truy vấn để kiểm tra xem tài khoản có tồn tại và mật khẩu đúng không
            var query = khachHangCollection
                .WhereEqualTo("Gmail", gmail)
                .WhereEqualTo("MatKhau", matKhau);

            var querySnapshot = query.GetSnapshotAsync().Result;

            if (querySnapshot.Count > 0)
            {
                // Lấy thông tin của tài khoản khách hàng
                var khachHangDocument = querySnapshot.Documents.First();
                var khachHang = ConvertToKhachHang(khachHangDocument);
                return khachHang;
            }

            return null;
        }

        private KhachHang ConvertToKhachHang(DocumentSnapshot document)
        {
            var khachHang = new KhachHang();

            if (document != null)
            {
                // Get the Firestore-generated Document ID
                khachHang.KhachHangID = document.Id;

                // Retrieve other fields from the document
                khachHang.HoTen = document.GetValue<string>("HoTen");
                khachHang.GioiTinh = document.GetValue<string>("GioiTinh");
                khachHang.Gmail = document.GetValue<string>("Gmail");
                khachHang.MatKhau = document.GetValue<string>("MatKhau");

                // Add similar lines for other fields
            }

            return khachHang;
        }
    }
}