using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Cloud.Firestore;

namespace RedNit_Airline.Models
{
    public class ChuyenBay
    {
        [FirestoreProperty]
        public string ChuyenBayID { get; set; }
        [FirestoreProperty]
        public string Gia { get; set; }
        [FirestoreProperty]
        public string DiemDi { get; set; }

        [FirestoreProperty]
        public string DiemDen { get; set; }

        [FirestoreProperty]
        public string GioKhoiHanh { get; set; }

        [FirestoreProperty]
        public string HinhAnh { get; set; }

        [FirestoreProperty]
        public string NgayDi { get; set; }

        [FirestoreProperty]
        public string NgayVe { get; set; }

        [FirestoreProperty]
        public int SoGheTrong { get; set; }

        [FirestoreProperty]
        public int SoGheVipTrong { get; set; }

        [FirestoreProperty]
        public string TrangThai { get; set; }
    }
}