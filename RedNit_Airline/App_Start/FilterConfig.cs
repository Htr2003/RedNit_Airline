﻿using System.Web;
using System.Web.Mvc;

namespace RedNit_Airline
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
