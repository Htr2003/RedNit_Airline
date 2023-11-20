using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Cloud;
using Google.Cloud.Firestore;

namespace RedNit_Airline.Models
{
    public class VeMayBay
    {
        [FirestoreProperty]
        public string VeID { get; set; }
        [FirestoreProperty]
        public string ChuyenBayID { get; set; }

        [FirestoreProperty]
        public string HangGhe { get; set; }

        [FirestoreProperty]
        public string KhachHangID { get; set; }

        [FirestoreProperty]
        public string LoaiVe { get; set; }

        [FirestoreProperty]
        public string TrangThaiBay { get; set; }

        [FirestoreProperty]
        public string GiaVe { get; set; }
       
    }
}