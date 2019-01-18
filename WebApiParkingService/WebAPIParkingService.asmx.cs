using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Globalization;
using WebApiParkingService.Classes;
using WebApiParkingService.Models;

namespace WebApiParkingService {
  /// <summary>
  /// Summary description for WebService1
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/", Name = "Parking Servis", Description = "Web servis za evidenciju o parkingu")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]
  // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
  [System.Web.Script.Services.ScriptService]
  public class WebService1 : System.Web.Services.WebService {

    [WebMethod (MessageName = "Svi tiketi", Description = "Prikaz svih parking tiketa u JSon formatu")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetAllTickets() {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket[] getParkingTicket = null;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM parking_ticket", connection);
          sda.SelectCommand.CommandType = System.Data.CommandType.Text;
          DataTable dTable = new DataTable();
          sda.Fill(dTable);
          getParkingTicket = new ParkingTicket[dTable.Rows.Count];
          int brojac = 0;
          for (int i = 0; i < dTable.Rows.Count; ++i) {
            getParkingTicket[brojac] = new ParkingTicket() {
              parking_ticket_id = Convert.ToString(dTable.Rows[i]["parking_ticket_id"]),
              car_licence_plate = Convert.ToString(dTable.Rows[i]["car_licence_plate"]),
              ticket_valid = Convert.ToString(dTable.Rows[i]["ticket_valid"])
            };
            ++brojac;
          }
          dTable.Clear();
          connection.Close();
        }
      }
      catch { }
      var JSonData = new {
        getParkingTicket = getParkingTicket
      };
      HttpContext.Current.Response.Write(json.Serialize(JSonData));
    }

    [WebMethod (MessageName = "ID tiketa", Description = "Pretraga po ID tiketa")]
    [ScriptMethod (ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetByTicketId(int id) {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket getParkingTicket = null;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM parking_ticket WHERE parking_ticket_id =" + id, connection);
          sda.SelectCommand.CommandType = System.Data.CommandType.Text;
          DataTable dTable = new DataTable();
          sda.Fill(dTable);
          getParkingTicket = new ParkingTicket() {
            parking_ticket_id = Convert.ToString(dTable.Rows[0]["parking_ticket_id"]),
            car_licence_plate = Convert.ToString(dTable.Rows[0]["car_licence_plate"]),
            ticket_valid = Convert.ToString(dTable.Rows[0]["ticket_valid"])
          };
          dTable.Clear();
          connection.Close();
        }
      }
      catch { }
      var JSonData = new {
        getParkingTicket = getParkingTicket
      };
      HttpContext.Current.Response.Write(json.Serialize(JSonData));
    }

    [WebMethod (MessageName = "Tablice", Description = "Pretraga po tablicama vozila")]
    [ScriptMethod (ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetByLicencePlate(string licencePlate) {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket[] getParkingTicket = null;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          SqlDataAdapter sda = 
            new SqlDataAdapter("SELECT * FROM parking_ticket WHERE car_licence_plate = '" + licencePlate + "'", connection);
          sda.SelectCommand.CommandType = System.Data.CommandType.Text;
          DataTable dTable = new DataTable();
          sda.Fill(dTable);
          getParkingTicket = new ParkingTicket[dTable.Rows.Count];
          int brojac = 0;
          for (int i = 0; i < dTable.Rows.Count; ++i) {
            getParkingTicket[brojac] = new ParkingTicket() {
              parking_ticket_id = Convert.ToString(dTable.Rows[i]["parking_ticket_id"]),
              car_licence_plate = Convert.ToString(dTable.Rows[i]["car_licence_plate"]),
              ticket_valid = Convert.ToString(dTable.Rows[i]["ticket_valid"])
            };
            ++brojac;
          }
          dTable.Clear();
          connection.Close();
        }
      }
      catch { }
      var JSonData = new {
        getParkingTicket = getParkingTicket
      };
      HttpContext.Current.Response.Write(json.Serialize(JSonData));
    }

    [WebMethod (MessageName = "Novi Tiket", Description = "Kreiranje novog tiketa prema registarskim tablicama")]
    [ScriptMethod (ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void NewTicket(string licencePlate) {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket getParkingTicket = null;
      DateTime ticketValid = DateTime.Now.AddHours(1);
      int dataRows = 0;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          SqlDataAdapter sda = new SqlDataAdapter();
          SqlCommand command = new SqlCommand();
          connection.Open();
          command.Connection = connection;
          command.CommandText =
            "INSERT INTO parking_ticket (car_licence_plate, ticket_valid) " +
            "VALUES (@car_licence_plate, @ticket_valid)";
          command.Parameters.AddWithValue("@car_licence_plate", licencePlate);
          command.Parameters.AddWithValue("@ticket_valid", ticketValid);
          command.ExecuteNonQuery();
          connection.Close();

          connection.Open();
          SqlDataAdapter sda2 = new SqlDataAdapter();
          SqlCommand command2 = new SqlCommand();
          command2.Connection = connection;
          command2.CommandText =
            "SELECT * FROM parking_ticket " +
            "WHERE car_licence_plate = @car_licence_plate AND ticket_valid = @ticket_valid";
          command2.Parameters.AddWithValue("@car_licence_plate", licencePlate);
          command2.Parameters.AddWithValue("@ticket_valid", ticketValid);
          command2.CommandType = System.Data.CommandType.Text;
          sda2.SelectCommand = command2;
          DataTable dTable = new DataTable();
          dataRows = sda2.Fill(dTable);
          getParkingTicket = new ParkingTicket() {
            parking_ticket_id = Convert.ToString(dTable.Rows[0]["parking_ticket_id"]),
            car_licence_plate = Convert.ToString(dTable.Rows[0]["car_licence_plate"]),
            ticket_valid = Convert.ToString(dTable.Rows[0]["ticket_valid"])
          };
          dTable.Clear();
          connection.Close();
        }
      }
      catch { }
      var JSonData = new {
        getParkingTicket = getParkingTicket
      };
      HttpContext.Current.Response.Write(json.Serialize(JSonData));
    }
  }
}
