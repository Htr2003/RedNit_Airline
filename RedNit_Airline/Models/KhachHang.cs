using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedNit_Airline.Models
{
    public class KhachHang
    {
        [FirestoreProperty]
        public string HoTen { get; set; }
        [FirestoreProperty]
        public Timestamp NgaySinh { get; set; }

        [FirestoreProperty]
        public string GioiTinh { get; set; }

        [FirestoreProperty]
        public string QuocTich { get; set; }

        [FirestoreProperty]
        public string Sdt { get; set; }
    }
}