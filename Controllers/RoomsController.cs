using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebApplication7.Models;

public class RoomsController : Controller
{
    // GET: Rooms
    public ActionResult Index()
    {
        List<Room> rooms = new List<Room>();
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Rooms";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(new Room
                {
                    RoomId = (int)reader["RoomId"],
                    RoomType = reader["RoomType"].ToString(),
                    IsAvailable = (bool)reader["IsAvailable"],
                    PricePerNight = (decimal)reader["PricePerNight"]
                });
            }
            conn.Close();
        }
        return View(rooms);
    }

    // GET: Rooms/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Rooms/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Room model)
    {
        if (ModelState.IsValid)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Rooms (RoomType, IsAvailable, PricePerNight) VALUES (@RoomType, @IsAvailable, @PricePerNight)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomType", model.RoomType);
                cmd.Parameters.AddWithValue("@IsAvailable", model.IsAvailable);
                cmd.Parameters.AddWithValue("@PricePerNight", model.PricePerNight);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            return RedirectToAction("Index");
        }
        return View(model);
    }

    // GET: Rooms/Edit/5
    public ActionResult Edit(int id)
    {
        Room room = null;
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Rooms WHERE RoomId = @RoomId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RoomId", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                room = new Room
                {
                    RoomId = (int)reader["RoomId"],
                    RoomType = reader["RoomType"].ToString(),
                    IsAvailable = (bool)reader["IsAvailable"],
                    PricePerNight = (decimal)reader["PricePerNight"]
                };
            }
            conn.Close();
        }
        if (room == null) return HttpNotFound();
        return View(room);
    }

    // POST: Rooms/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Room model)
    {
        if (ModelState.IsValid)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Rooms SET RoomType = @RoomType, IsAvailable = @IsAvailable, PricePerNight = @PricePerNight WHERE RoomId = @RoomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomType", model.RoomType);
                cmd.Parameters.AddWithValue("@IsAvailable", model.IsAvailable);
                cmd.Parameters.AddWithValue("@PricePerNight", model.PricePerNight);
                cmd.Parameters.AddWithValue("@RoomId", model.RoomId);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            return RedirectToAction("Index");
        }
        return View(model);
    }

    // GET: Rooms/Delete/5
    public ActionResult Delete(int id)
    {
        Room room = null;
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Rooms WHERE RoomId = @RoomId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RoomId", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                room = new Room
                {
                    RoomId = (int)reader["RoomId"],
                    RoomType = reader["RoomType"].ToString(),
                    IsAvailable = (bool)reader["IsAvailable"],
                    PricePerNight = (decimal)reader["PricePerNight"]
                };
            }
            conn.Close();
        }
        if (room == null) return HttpNotFound();
        return View(room);
    }

    // POST: Rooms/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Rooms WHERE RoomId = @RoomId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RoomId", id);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        return RedirectToAction("Index");
    }
}
