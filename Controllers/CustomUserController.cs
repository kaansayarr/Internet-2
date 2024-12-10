using Microsoft.AspNetCore.Mvc;
using Internet_1.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Internet_1.Controllers
{
    public class UserController : Controller
    {


        private readonly PasswordHasher<CustomUser> _passwordHasher = new PasswordHasher<CustomUser>();


        // GET: User/Login
        [HttpGet]
        public IActionResult Login()

        {

            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                CustomUser user = null;

                // Retrieve the user from the database
                using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    connection.Open();
                    string query = "SELECT Id, UserName, Email, PasswordHash, Rol FROM CustomUser WHERE UserName = @UserName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", model.UserName);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new CustomUser
                                {
                                    Id = reader.GetInt32(0),
                                    UserName = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    PasswordHash = reader.GetString(3),
                                    Rol = reader.GetString(4)  // Ensure you are getting the role

                                };
                            }
                        }
                        using (var session = command.ExecuteReader())
                        {
                            if (session.Read())
                            {
                                var userId = session["Id"].ToString();
                                HttpContext.Session.SetString("Rol", user.Rol ?? "User");
                                HttpContext.Session.SetString("UserId", userId);
                                HttpContext.Session.SetString("UserName", user.UserName);
                                return RedirectToAction("Index", "Home");
                            }

                        }
                    }
                }

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                // Verify the password hash
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Successful login, redirect to Dashboard or another secured page
                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
            }

            return View(model);
        }

        // Secure page for logged-in users (after successful login)
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists in the database
                using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    connection.Open();
                    Console.WriteLine($"Connection: {connection.State}");
                    string checkQuery = "SELECT COUNT(*) FROM CustomUser WHERE UserName = @UserName";

                    using (var command = new SqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", model.UserName);

                        var userCount = (int)command.ExecuteScalar();

                        if (userCount > 0)
                        {
                            ModelState.AddModelError("", "Username already exists.");
                            return View(model);
                        }
                    }
                }

                // Hash the password
                var user = new CustomUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = _passwordHasher.HashPassword(null, model.PasswordHash), // Hash the password
                    Rol = model.Rol ?? "User"              };

                // Insert the new user into the database
                try
                {
                    using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO CustomUser (UserName, Email, PasswordHash, Rol) VALUES (@UserName, @Email, @PasswordHash, @Rol)";

                        using (var command = new SqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@UserName", user.UserName);
                            command.Parameters.AddWithValue("@Email", user.Email);
                            command.Parameters.AddWithValue("@Rol",user.Rol);

                            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);


                            command.ExecuteNonQuery();
                        }
                    }

                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login","User");
                }
                catch (SqlException sqlEx)
                {
                    // Handle SQL-specific exceptions
                    ModelState.AddModelError("", $"Database error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    // Handle general exceptions
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            return View(model);
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        // Secure Dashboard page
        public IActionResult Index()

        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }
    }
}


