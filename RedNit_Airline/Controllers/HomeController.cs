using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Cloud.Firestore;
using RedNit_Airline.Models;
using Google.Type;

namespace RedNit_Airline.Controllers
{
    public class HomeController : Controller
    {
        private FirestoreDb firestoreDb;

        public HomeController()
        {
            firestoreDb = FirestoreDb.Create("dvmb-16215");
        }
        public ActionResult Index()
        {
            var sanBayCollection = firestoreDb.Collection("SanBay");
            var sanBayDocuments = sanBayCollection.GetSnapshotAsync().Result;

            List<SanBay> sanBays = new List<SanBay>();

            foreach (var document in sanBayDocuments)
            {
                if (document.Exists)
                {
                    var sanBay = new SanBay
                    {
                        TenSanBay = document.GetValue<string>("TenSanBay")
                    };
                    sanBays.Add(sanBay);
                }
            }

            ViewBag.DiemDi = new SelectList(sanBays, "TenSanBay", "TenSanBay");
            ViewBag.DiemDen = new SelectList(sanBays, "TenSanBay", "TenSanBay");

            return View();
        }

        /*public async Task<ActionResult> KhachHangList()
        {

            CollectionReference collection = firestoreDb.Collection("KhachHang"); // Replace with your collection name

            QuerySnapshot querySnapshot = await collection.GetSnapshotAsync();

            List<KhachHang> khachHangList = new List<KhachHang>();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> firestoreData = documentSnapshot.ToDictionary();

                    // Convert the dictionary to a JSON string
                    string firestoreJson = JsonConvert.SerializeObject(firestoreData);

                    // Deserialize the JSON string into your KhachHang object
                    KhachHang khachHang = JsonConvert.DeserializeObject<KhachHang>(firestoreJson);

                    if (firestoreData.ContainsKey("NgaySinh"))
                    {
                        Timestamp timestamp = (Timestamp)firestoreData["NgaySinh"];
                        khachHang.NgaySinh = timestamp;

                    }

                    khachHangList.Add(khachHang);
                }
            }

            return View(khachHangList);
        }*/

        public async Task<ActionResult> TimKiemChuyenBay(string diemDi, string diemDen)
        {
            CollectionReference chuyenBayCollection = firestoreDb.Collection("ChuyenBay");

            // Query ChuyenBay where DiemDiId and DiemDenId match the input
            Query query = chuyenBayCollection
                .WhereEqualTo("DiemDi", diemDi)
                .WhereEqualTo("DiemDen", diemDen);

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            List<ChuyenBay> ketQua = new List<ChuyenBay>();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                var chuyenBay = new ChuyenBay
                {
                    DiemDi = documentSnapshot.GetValue<string>("DiemDi"),
                    DiemDen = documentSnapshot.GetValue<string>("DiemDen"),
                    GioKhoiHanh = documentSnapshot.GetValue<string>("GioBatDau"),
                    NgayDi = documentSnapshot.GetValue<string>("NgayDi"),
                    NgayVe = documentSnapshot.GetValue<string>("NgayVe"),
                    ChuyenBayID = documentSnapshot.GetValue<string>("ChuyenBayID"),
                    Gia = documentSnapshot.GetValue<string>("Gia"),
                };
                
                ketQua.Add(chuyenBay);
            }

            return View("TimKiemChuyenBay", ketQua);
        }

        public async Task<ChuyenBay> GetChuyenBayByIdAsync(string chuyenBayId)
        {
            CollectionReference chuyenBayCollection = firestoreDb.Collection("ChuyenBay");
            Query query = chuyenBayCollection.WhereEqualTo("ChuyenBayID", chuyenBayId);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            // Check if any documents match the query
            if (querySnapshot.Documents.Count > 0)
            {
                // Assuming you want the first matching document
                DocumentSnapshot documentSnapshot = querySnapshot.Documents[0];

                // Serialize Firestore document to JSON
                IDictionary<string, object> documentData = documentSnapshot.ToDictionary();

                // Serialize dictionary to JSON
                string json = JsonConvert.SerializeObject(documentData);

                // Deserialize JSON to ChuyenBay
                ChuyenBay chuyenBay = JsonConvert.DeserializeObject<ChuyenBay>(json);


                return chuyenBay;
            }
            else
            {
                // No matching document found
                return null;
            }
        }

        public async Task<bool> DatVeAsync(ChuyenBay chuyenBay, string khachHangId)
        {
            // Thực hiện logic đặt vé tại đây, ví dụ: cập nhật trạng thái của chuyến bay
            // DocumentReference docRef = firestoreDb.Collection("ChuyenBay").Document(chuyenBayId);

            CollectionReference veCollectionRef = firestoreDb.Collection("VeMayBay");

            // Create a reference to the ChuyenBay document
            DocumentReference chuyenBayRef = firestoreDb.Collection("ChuyenBay").Document(chuyenBay.ChuyenBayID);

            // Add the ChuyenBay reference to the VeMayBay document
            VeMayBay veMayBayDto = new VeMayBay
            {
                ChuyenBayID = chuyenBayRef.Id,
                KhachHangID = khachHangId,
                GiaVe = "",
                HangGhe = "",
                LoaiVe = "",
                TrangThaiVe = false,
               
            };

            var veMayBayData = new Dictionary<string, object>
            {
                { "ChuyenBayID", veMayBayDto.ChuyenBayID },
                { "KhachHangID", veMayBayDto.KhachHangID },
                { "GiaVe", veMayBayDto.GiaVe },
                { "HangGhe", veMayBayDto.HangGhe },
                { "LoaiVe", veMayBayDto.LoaiVe },
                { "TrangThaiBay", veMayBayDto.TrangThaiVe },
            };

            // Add the ChuyenBay reference to the VeMayBay document
            DocumentReference veDocRef = await veCollectionRef.AddAsync(veMayBayData);

            string veId = veDocRef.Id;

            return true; 
        }

        [HttpGet]
        public async Task<ActionResult> DatVe(string chuyenBayId)
        {
            if (string.IsNullOrEmpty(chuyenBayId))
            {
                ViewBag.ErrorMessage = "ChuyenBayID is null or empty.";
                return View("Error");
            }
            ViewBag.ChuyenBayId = chuyenBayId;

            ChuyenBay chuyenBay = await GetChuyenBayByIdAsync(chuyenBayId);

            return View(chuyenBay);
        }

        [HttpPost]
        public async Task<ActionResult> DatVe(ChuyenBay model, string khachHangId)
        {
            ChuyenBay chuyenBay = await GetChuyenBayByIdAsync(model.ChuyenBayID);

            if (chuyenBay != null)
            {
                bool datVeSuccess = await DatVeAsync(chuyenBay, khachHangId);

                if (datVeSuccess)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View("Error");
        }

        public ActionResult GetTickets()
        {
            // Lấy KhachHangID từ session
            string loggedInCustomerId = Session["LoggedInCustomerId"] as string;

            if (!string.IsNullOrEmpty(loggedInCustomerId))
            {
                // Gọi hàm để lấy danh sách vé cho khách hàng
                var tickets = GetTicketsForCustomer(loggedInCustomerId);

                // Sử dụng danh sách vé trong view hoặc thực hiện các xử lý khác
                return View(tickets);
            }
            else
            {
                // Redirect hoặc xử lý khi chưa đăng nhập
                return RedirectToAction("Login", "Account");
            }
        }

        private List<VeMayBay> GetTicketsForCustomer(string khachHangId)
        {
            var veMayBayCollection = firestoreDb.Collection("VeMayBay");

            // Thực hiện truy vấn để lấy danh sách vé cho khách hàng có KhachHangID tương ứng
            var query = veMayBayCollection.WhereEqualTo("KhachHangID", khachHangId);

            var querySnapshot = query.GetSnapshotAsync().Result;

            var tickets = new List<VeMayBay>();

            foreach (var document in querySnapshot.Documents)
            {
                var ticket = ConvertToVeMayBay(document);
                tickets.Add(ticket);
            }

            return tickets;
        }

        private VeMayBay ConvertToVeMayBay(DocumentSnapshot document)
        {
            // Tạo đối tượng VeMayBay từ dữ liệu của Firestore
            var veMayBay = new VeMayBay
            {
                VeMayBayID = document.Id,
                ChuyenBayID = document.GetValue<string>("ChuyenBayID"),
                KhachHangID = document.GetValue<string>("KhachHangID"),
                TrangThaiVe = document.GetValue<bool>("TrangThaiVe"),
                GiaVe = document.GetValue<string>("giaVe"),
            };

            // Lấy dữ liệu từ ChuyenBay dựa trên ChuyenBayID
            var chuyenBay = GetChuyenBay(veMayBay.ChuyenBayID);

            if (chuyenBay != null)
            {
                // Gán dữ liệu từ ChuyenBay vào veMayBay
                veMayBay.NgayDi = chuyenBay.NgayDi;
                veMayBay.GioDi = chuyenBay.GioKhoiHanh;
            }

            return veMayBay;
        }

        private ChuyenBay GetChuyenBay(string chuyenBayId)
        {
            var chuyenBayCollection = firestoreDb.Collection("ChuyenBay");

            var chuyenBayDocument = chuyenBayCollection.Document(chuyenBayId).GetSnapshotAsync().Result;

            if (chuyenBayDocument.Exists)
            {
                var chuyenBayData = chuyenBayDocument.ToDictionary();

                // Manually map fields to ChuyenBay model
                var chuyenBay = new ChuyenBay
                {
                    ChuyenBayID = chuyenBayDocument.Id,
                    NgayDi = chuyenBayData.ContainsKey("GioBatDau") ? chuyenBayData["GioBatDau"].ToString() : null,
                    GioKhoiHanh = chuyenBayData.ContainsKey("NgayDi") ? chuyenBayData["NgayDi"].ToString() : null,
                    // Map other fields as needed
                };

                return chuyenBay;
            }

            return null;
        }
    }
}