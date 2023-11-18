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
                    NgayVe = documentSnapshot.GetValue<string>("NgayVe")
                    // Other property assignments...
                };
                
                ketQua.Add(chuyenBay);
            }

            return View("TimKiemChuyenBay", ketQua);
        }

        [HttpGet]
        public ActionResult BookFlight(string flightId)
        {
            // Lấy thông tin chuyến bay từ Firestore
            ChuyenBay flight = GetFlightFromFirestore(flightId);

            // Truyền thông tin chuyến bay và form đặt vé đến view
            return View(new DatVeViewModel { ChuyenBayInf = flight, KhachHangInf = new KhachHang() });
        }

        [HttpPost]
        public ActionResult BookFlight(DatVeViewModel viewModel)
        {

            // Xử lý thông tin đặt vé và lưu vào Firestore
            firestoreDb.Collection("VeMayBay").AddAsync(viewModel);

            return RedirectToAction("ThankYou");
        }

        private ChuyenBay GetFlightFromFirestore(string flightId)
        {
            string collectionPath = "ChuyenBay";

            // Truy vấn Firestore để lấy thông tin chuyến bay dựa trên flightId
            DocumentReference docRef = firestoreDb.Collection(collectionPath).Document(flightId);
            DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;

            if (snapshot.Exists)
            {
                // Chuyển đổi dữ liệu từ snapshot thành đối tượng Flight
                ChuyenBay flight = snapshot.ConvertTo<ChuyenBay>();
                return flight;
            }
            else
            {
                // Xử lý trường hợp không tìm thấy chuyến bay
                return null;
            }
        }
    }
}