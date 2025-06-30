using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApplication7.Models;
using System.Data.SqlClient;
using static System.Collections.Specialized.BitVector32;

namespace WebApplication7.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    conn.Open();
                    var role = cmd.ExecuteScalar() as string;
                    conn.Close();

                    if (!string.IsNullOrEmpty(role))
                    {
                        Session["Username"] = model.Username;
                        Session["Role"] = role;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            // If login fails, redisplay the form with errors
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Booking model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Check for available room
                    string query = "SELECT TOP 1 RoomId, PricePerNight FROM Rooms WHERE RoomType = @RoomType AND IsAvailable = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoomType", model.RoomType);
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int roomId = (int)reader["RoomId"];
                        decimal price = (decimal)reader["PricePerNight"];
                        reader.Close();

                        int days = (model.CheckOut - model.CheckIn).Days;
                        if (days < 1) days = 1;
                        model.Bill = price * days;
                        model.RoomId = roomId;
                        model.BookingId = Guid.NewGuid();
                        model.Username = Session["Username"]?.ToString();

                        // Insert booking
                        string insertQuery = "INSERT INTO Bookings (BookingId, RoomId, RoomType, CheckIn, CheckOut, Bill, Username) VALUES (@BookingId, @RoomId, @RoomType, @CheckIn, @CheckOut, @Bill, @Username)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@BookingId", model.BookingId);
                        insertCmd.Parameters.AddWithValue("@RoomId", model.RoomId);
                        insertCmd.Parameters.AddWithValue("@RoomType", model.RoomType);
                        insertCmd.Parameters.AddWithValue("@CheckIn", model.CheckIn);
                        insertCmd.Parameters.AddWithValue("@CheckOut", model.CheckOut);
                        insertCmd.Parameters.AddWithValue("@Bill", model.Bill);
                        insertCmd.Parameters.AddWithValue("@Username", model.Username);
                        insertCmd.ExecuteNonQuery();

                        // Mark room as unavailable
                        string updateRoom = "UPDATE Rooms SET IsAvailable = 0 WHERE RoomId = @RoomId";
                        SqlCommand updateCmd = new SqlCommand(updateRoom, conn);
                        updateCmd.Parameters.AddWithValue("@RoomId", model.RoomId);
                        updateCmd.ExecuteNonQuery();

                        conn.Close();
                        return RedirectToAction("Bill", model);
                    }
                    else
                    {
                        conn.Close();
                        ModelState.AddModelError("", "No available rooms of selected type.");
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        public ActionResult Bill(Booking model)
        {
            return View(model);
        }

        public ActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUser(User model, string ConfirmPassword, string[] LanguagesKnown)
        {
            if (LanguagesKnown != null)
                model.LanguagesKnown = string.Join(",", LanguagesKnown);

            if (ModelState.IsValid)
            {
                if (model.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Users 
                        (FirstName, LastName, Username, Gender, Password, Email, Phone, Address, Age, LanguagesKnown, Country, Role)
                        VALUES (@FirstName, @LastName, @Username, @Gender, @Password, @Email, @Phone, @Address, @Age, @LanguagesKnown, @Country, @Role)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@Password", model.Password); // Hash in production!
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Phone", model.Phone);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@Age", model.Age);
                    cmd.Parameters.AddWithValue("@LanguagesKnown", model.LanguagesKnown ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", model.Country);
                    cmd.Parameters.AddWithValue("@Role", "Customer");
                    model.LanguagesKnown = LanguagesKnown != null ? string.Join(",", LanguagesKnown) : string.Empty;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return RedirectToAction("UserList");
            }
            return View(model);
        }

        public ActionResult UserList()
        {
            List<User> users = new List<User>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Username = reader["Username"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Password = reader["Password"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Address = reader["Address"].ToString(),
                        Age = (int)reader["Age"],
                        LanguagesKnown = reader["LanguagesKnown"].ToString(),
                        Country = reader["Country"].ToString(),
                        Role = reader["Role"].ToString()
                    });
                }
                conn.Close();
            }
            return View(users);
        }

        public ActionResult AdminDashboard()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            // Example: Query total bookings, occupancy, revenue
            var dashboard = new AdminDashboardViewModel();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Total bookings
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Bookings", conn))
                    dashboard.TotalBookings = (int)cmd.ExecuteScalar();

                // Occupied rooms
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Rooms WHERE IsAvailable = 0", conn))
                    dashboard.OccupiedRooms = (int)cmd.ExecuteScalar();

                // Revenue
                using (var cmd = new SqlCommand("SELECT ISNULL(SUM(Bill),0) FROM Bookings", conn))
                    dashboard.TotalRevenue = (decimal)cmd.ExecuteScalar();

                conn.Close();
            }

            return View(dashboard);
        }

        public ActionResult CustomerDashboard()
        {
            if (Session["Role"]?.ToString() != "Customer")
                return RedirectToAction("Login", "Home");

            string username = Session["Username"].ToString();
            List<Booking> bookings = new List<Booking>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Bookings WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookings.Add(new Booking
                    {
                        BookingId = reader["BookingId"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("BookingId")) : Guid.Empty,
                        RoomId = reader["RoomId"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("RoomId")) : 0,
                        RoomType = reader["RoomType"]?.ToString(),
                        CheckIn = reader["CheckIn"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("CheckIn")) : DateTime.MinValue,
                        CheckOut = reader["CheckOut"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("CheckOut")) : DateTime.MinValue,
                        Bill = reader["Bill"] != DBNull.Value ? reader.GetDecimal(reader.GetOrdinal("Bill")) : 0m,
                        Username = reader["Username"]?.ToString()
                    });
                }
            
            conn.Close();
            }
            return View(bookings);
        }
    }


    // Example ViewModel for Admin Dashboard
    public class AdminDashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int OccupiedRooms { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
