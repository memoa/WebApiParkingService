using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace WebApiParkingService.Classes {
  public class DBConnection {
    public static string ConnectionString {
      get {
        return WebConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString.ToString();
      }
    }
  }
}