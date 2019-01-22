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

    [WebMethod(MessageName = "Svi tiketi", Description = "Prikaz svih parking tiketa u JSon formatu")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetAllTickets() {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket[] getParkingTicket = null;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          connection.Open();
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

    [WebMethod(MessageName = "ID tiketa", Description = "Pretraga po ID tiketa")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetByTicketId(int id) {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket getParkingTicket = null;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          connection.Open();
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

    [WebMethod(MessageName = "Tablice", Description = "Pretraga po tablicama vozila")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetByLicencePlate(string licencePlate) {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket[] getParkingTicket = null;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          connection.Open();
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

    [WebMethod(MessageName = "Novi Tiket", Description = 
      "Kreiranje novog tiketa prema registarskim tablicama. Ako vec postoji tiket sa zadatim tablicama, koji nije istekao, " +
      "taj tiket ce se produziti za jos 1h. Inace, kreirace se novi tiket koji istice za 1h od vremena kreiranja.")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void NewTicket(string licencePlate) {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket[] getParkingTicket = null;
      DateTime trenutnoVreme = DateTime.Now;
      DateTime ticketValid = new DateTime();
      int parkingTicketId = 0;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          // Pretraga da li vec postoji tiket sa zadatim tablicama i da li nije istekao
          connection.Open();
          SqlDataAdapter sda = new SqlDataAdapter();
          using (SqlCommand command = new SqlCommand()) {
            command.Connection = connection;
            command.CommandText =
              "SELECT * FROM parking_ticket WHERE car_licence_plate = @car_licence_plate " +
              //"AND ticket_valid = (SELECT MAX(ticket_valid) FROM parking_ticket) " +
              "AND ticket_valid > @ticket_valid";
            command.Parameters.AddWithValue("@car_licence_plate", licencePlate);
            command.Parameters.AddWithValue("@ticket_valid", trenutnoVreme);
            command.CommandType = System.Data.CommandType.Text;
            sda.SelectCommand = command;
          }
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
          
          // Tiket sa zadatim tablicama ne postoji ili je istekao
          if (getParkingTicket.Length == 0) {
            ticketValid = DateTime.Now.AddHours(1);
            connection.Open();
            using (SqlCommand command = new SqlCommand()) {
              command.Connection = connection;
              command.CommandText =
                "INSERT INTO parking_ticket (car_licence_plate, ticket_valid) " +
                "VALUES (@car_licence_plate, @ticket_valid)";
              command.Parameters.AddWithValue("@car_licence_plate", licencePlate);
              command.Parameters.AddWithValue("@ticket_valid", ticketValid);
              command.ExecuteNonQuery();
            }
            connection.Close();
          }
          // Tiket sa zadatim tablicama postoji i nije istekao
          else {
            parkingTicketId = Convert.ToInt32(getParkingTicket[getParkingTicket.Length - 1].parking_ticket_id);
            ticketValid = Convert.ToDateTime(getParkingTicket[getParkingTicket.Length - 1].ticket_valid).AddHours(1);
            connection.Open();
            using (SqlCommand command = new SqlCommand()) {
              command.Connection = connection;
              command.CommandText =
                "UPDATE parking_ticket SET ticket_valid = @ticket_valid WHERE parking_ticket_id = @parking_ticket_id";
              command.Parameters.AddWithValue("@ticket_valid", ticketValid);
              command.Parameters.AddWithValue("@parking_ticket_id", parkingTicketId);
              command.ExecuteNonQuery();
            }
            connection.Close();
          }
          
          // Pretraga novog tiketa radi provere da li je pravilno upisan u bazu
          connection.Open();
          using (SqlCommand command = new SqlCommand()) {
            command.Connection = connection;
            command.CommandText =
              "SELECT * FROM parking_ticket " +
              "WHERE car_licence_plate = @car_licence_plate AND ticket_valid = @ticket_valid";
            command.Parameters.AddWithValue("@car_licence_plate", licencePlate);
            command.Parameters.AddWithValue("@ticket_valid", ticketValid);
            command.CommandType = System.Data.CommandType.Text;
            sda.SelectCommand = command;
          }
          sda.Fill(dTable);
          getParkingTicket = new ParkingTicket[dTable.Rows.Count];
          brojac = 0;
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

    [WebMethod(MessageName = "Svi istekli tiketi", Description = "Prikaz isteklih tiketa")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public void GetAllExpired() {
      JavaScriptSerializer json = new JavaScriptSerializer();
      ParkingTicket[] getParkingTickets = null;
      DateTime trenutnoVreme = DateTime.Now;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          connection.Open();
          SqlDataAdapter sda = new SqlDataAdapter();
          SqlCommand command = new SqlCommand();
          command.Connection = connection;
          command.CommandText = "SELECT * FROM parking_ticket WHERE ticket_valid < @ticket_valid";
          command.Parameters.AddWithValue("@ticket_valid", trenutnoVreme);
          command.CommandType = System.Data.CommandType.Text;
          sda.SelectCommand = command;
          DataTable dTable = new DataTable();
          sda.Fill(dTable);
          getParkingTickets = new ParkingTicket[dTable.Rows.Count];
          int brojac = 0;
          for (int i = 0; i < dTable.Rows.Count; ++i) {
            getParkingTickets[brojac] = new ParkingTicket() {
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
        getParkingTickets = getParkingTickets
      };
      HttpContext.Current.Response.Write(json.Serialize(JSonData));
    }

    [WebMethod(MessageName = "Brisanje Isteklih Tiketa", Description = "Brisanje Svih Isteklih tiketa")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public int RemoveAllExpired() {
      int result = -1;
      try {
        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
          connection.Open();
          SqlCommand command = new SqlCommand();
          command.Connection = connection;
          command.CommandText = "DELETE FROM parking_ticket WHERE ticket_valid < @ticket_valid";
          command.Parameters.AddWithValue("@ticket_valid", DateTime.Now);
          result = command.ExecuteNonQuery();
          connection.Close();
        }
      }
      catch { }
      return result;
    }
  }
}
