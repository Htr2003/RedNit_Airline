using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Cloud;
using Google.Cloud.Firestore;

namespace RedNit_Airline.Models
{
    public class SanBay
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string TenSanBay { get; set; }
    }
}