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
                TrangThaiBay = "Đang chờ",
               
            };

            var veMayBayData = new Dictionary<string, object>
            {
                { "ChuyenBayID", veMayBayDto.ChuyenBayID },
                { "KhachHangID", veMayBayDto.KhachHangID },
                { "GiaVe", veMayBayDto.GiaVe },
                { "HangGhe", veMayBayDto.HangGhe },
                { "LoaiVe", veMayBayDto.LoaiVe },
                { "TrangThaiBay", veMayBayDto.TrangThaiBay },
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
    }
}