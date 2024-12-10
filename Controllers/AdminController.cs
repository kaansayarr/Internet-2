using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Internet_1.Models;
using Microsoft.AspNetCore.Identity;

namespace Internet_1.Controllers
{
    [Route("Admin")]

    public class AdminController : Controller

    {

        private readonly string _connectionString = ConnectionHelper.GetConnectionString();


        // GET: Admin/Index
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";

            var model = new AdminViewModel();

            // Fetch users and videos
            using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                connection.Open();

                // Fetch Users
                var userQuery = "SELECT Id, UserName, Email, Rol FROM CustomUser";
                using (var command = new SqlCommand(userQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Users.Add(new CustomUser
                            {
                                Id = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Rol = reader.GetString(3)
                            });
                        }
                    }
                }

                // Fetch Videos
                var videoQuery = "SELECT Id, Title, Url, Description FROM Videos";
                using (var command = new SqlCommand(videoQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Videos.Add(new Video
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Url = reader.GetString(2),
                                Description = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"Users count: {model.Users.Count}");
            Console.WriteLine($"Videos count: {model.Videos.Count}");
            return View(model);
        }


        // POST: Admin/Create - Create a new user
        [HttpGet("Create")]
         
        public IActionResult Create(CustomUser model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            return View(model);
        }



        [HttpPost("Admin/Create")]
        public IActionResult CreateUser(CustomUser model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            // Check if ModelState is valid first
            if (true)
            {
                // Make sure the username is not empty
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError("", "Username is required.");
                    return View(model);
                }

                // Check if the username already exists in the database
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM CustomUser WHERE UserName = @UserName";

                    using (var command = new SqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", model.UserName);
                        var userCount = (int)command.ExecuteScalar();

                        // If the username already exists, show an error
                        if (userCount > 0)
                        {
                            ModelState.AddModelError("", "Username already exists.");
                            return View(model); // Return the view to correct the username
                        }
                    }
                }

                // Hash the password before inserting into the database
                var passwordHasher = new PasswordHasher<CustomUser>();
                string hashedPassword = passwordHasher.HashPassword(model, model.PasswordHash);

                // Create a new CustomUser object to insert
                var user = new CustomUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Rol = model.Rol
                };

                try
                {
                    // Insert the new user into the database
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO CustomUser (UserName, Email, PasswordHash, Rol) VALUES (@UserName, @Email, @PasswordHash, @Rol)";

                        using (var command = new SqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@UserName", user.UserName);
                            command.Parameters.AddWithValue("@Email", user.Email);
                            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                            command.Parameters.AddWithValue("@Rol", user.Rol);

                            // Execute the insertion
                            int rowsAffected = command.ExecuteNonQuery();

                            // Check if the insertion was successful
                            if (rowsAffected > 0)
                            {
                                TempData["SuccessMessage"] = "User created successfully!";
                                return RedirectToAction("Index"); // Redirect to the user list or another view
                            }
                            else
                            {
                                ModelState.AddModelError("", "Failed to create user.");
                            }
                        }
                    }
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

            // If ModelState is invalid or an error occurred, return the same view with the model
            return View(model);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            CustomUser user = null;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, UserName, Email, Rol FROM CustomUser WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new CustomUser
                                {
                                    Id = reader.GetInt32(0),
                                    UserName = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Rol = reader.GetString(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
            }

            if (user == null)
            {
                return NotFound();
            }

            // Set roles in ViewBag
            ViewBag.Roles = new List<string> { "user", "egitmen", "admin" };

            return View(user); // Return the CustomUser model directly to the view
        }

        // POST: Admin/Edit/{id}

        [HttpPost("Edit/{id}")]
        public IActionResult Edit(CustomUser model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            Console.WriteLine($"Editing user with ID: {model.Id}, UserName: {model.UserName}, Role: {model.Rol}");

            if (true)
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        // Log the SQL query and parameters
                        string query = "UPDATE CustomUser SET UserName = @UserName, Rol = @Rol WHERE Id = @Id";
                        Console.WriteLine($"Executing query: {query}");
                        Console.WriteLine($"Parameters: UserName = {model.UserName}, Rol = {model.Rol}, Id = {model.Id}");

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserName", model.UserName);
                            command.Parameters.AddWithValue("@Rol", model.Rol);
                            command.Parameters.AddWithValue("@Id", model.Id);

                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();
                            Console.WriteLine($"Rows affected: {rowsAffected}");

                            // Check if any rows were updated
                            if (rowsAffected > 0)
                            {
                                TempData["SuccessMessage"] = "User updated successfully!";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                Console.WriteLine("No rows were affected.");
                                ModelState.AddModelError("", "No rows were updated. Please try again.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", $"An error occurred while updating the user: {ex.Message}");
                }
            }

            return View(model);  // Return the model back to the view in case of failure
        }




        // POST: Admin/Delete/{id} - Delete user
        [HttpPost("Admin/DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                // Deletion logic here
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM CustomUser WHERE Id = @Id";
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "User deleted successfully!";
                return RedirectToAction("Index"); // Redirect to the list page after deletion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index"); // Redirect in case of error
            }
        }





        // POST: Admin/ChangeUserRole - Change the role of a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeUserRole(int userId, string role)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE CustomUser SET Rol = @Role WHERE Id = @UserId";
                    using (var command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Role", role);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Role updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }



        [HttpGet("UploadVideo")]
        public IActionResult UploadVideo()
        {

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            return View(); // Ensure the corresponding UploadVideo.cshtml exists in Views/Admin.
        }

        [HttpGet("EditVideo/{id}")]
        public IActionResult EditVideo(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {


                connection.Open();
                var query = "SELECT * FROM Videos WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Create a Video model with the fetched data
                            var video = new Video
                            {
                                Id = (int)reader["Id"],
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Url = reader["Url"].ToString()
                            };

                            return View(video);
                        }
                        else
                        {
                            // Handle case when video is not found
                            Console.WriteLine("Video not found.");
                            return NotFound(); // You can return a custom error view if preferred
                        }
                    }
                }
            }
        }

        [HttpPost("EditVideo/{id}")]
        public IActionResult EditVideo(int id, string title, string description, IFormFile VideoFile)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                ModelState.AddModelError("", "Title and Description are required.");
                return View(); // Hata durumunda formu tekrar gösteriyoruz
            }

            try
            {
                string videoUrl = null;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");

                // Eğer yeni bir video dosyası yüklenmişse, dosyayı yükleyip yeni URL oluşturuyoruz
                if (VideoFile != null)
                {
                    var filePath = Path.Combine(uploadsFolder, VideoFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        VideoFile.CopyTo(stream);
                    }
                    videoUrl = $"/uploads/videos/{VideoFile.FileName}";
                }
                else
                {
                    // Eğer yeni dosya yüklenmemişse, mevcut video URL'sini alıyoruz
                    using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                    {
                        connection.Open();
                        var query = "SELECT Url FROM Videos WHERE Id = @Id";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            videoUrl = (string)command.ExecuteScalar();
                        }
                    }
                }

                // Video başlığı ve açıklamasını güncelliyoruz
                using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    connection.Open();
                    var updateQuery = "UPDATE Videos SET Title = @Title, Description = @Description, Url = @Url WHERE Id = @Id";
                    using (var command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Url", videoUrl); // Eğer yeni video yüklenmediyse eski URL'yi kullanıyoruz
                        command.Parameters.AddWithValue("@Id", id);

                        var rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Video updated successfully!";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to update video.");
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(); // Hata durumunda formu tekrar göster
            }
        }
        [HttpPost("UploadVideo")]
        public IActionResult UploadVideo(IFormFile VideoFile, string Title, string Description)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            Console.WriteLine("=== Debug Log: UploadVideo Action Triggered ===");
            try
            {
                // Log input values
                Console.WriteLine($"Input Data: Title = {Title}, Description = {Description}");
                if (VideoFile != null)
                {
                    Console.WriteLine($"Video File: Name = {VideoFile.FileName}, Size = {VideoFile.Length} bytes");
                }
                else
                {
                    Console.WriteLine("Video File is null.");
                    ModelState.AddModelError("", "Please select a video file to upload.");
                    return View();
                }

                // Define upload folder path
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
                Console.WriteLine($"Uploads Folder Path: {uploadsFolder}");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Console.WriteLine("Uploads folder does not exist. Creating directory...");
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique file name
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(VideoFile.FileName);
                Console.WriteLine($"Generated Unique File Name: {uniqueFileName}");

                // Full file path
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                Console.WriteLine($"Full File Path: {filePath}");

                // Save the file locally
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Console.WriteLine("Saving file to disk...");
                    VideoFile.CopyTo(stream);
                }

                // Store relative file path in the database
                var relativePath = $"/uploads/videos/{uniqueFileName}";
                Console.WriteLine($"Relative Path for Database: {relativePath}");

                using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    Console.WriteLine("Opening database connection...");
                    connection.Open();

                    var insertQuery = "INSERT INTO Videos (Title, Url, Description) VALUES (@Title, @Url, @Description)";
                    Console.WriteLine($"Executing SQL Query: {insertQuery}");
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Url", relativePath);
                        command.Parameters.AddWithValue("@Description", Description);

                        var rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Rows Affected: {rowsAffected}");

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Video uploaded and saved to database successfully.");
                            TempData["SuccessMessage"] = "Video uploaded successfully!";
                        }
                        else
                        {
                            Console.WriteLine("Failed to upload video. No rows affected.");
                            ModelState.AddModelError("", "Failed to upload video.");
                        }
                    }
                }

                Console.WriteLine("Redirecting to Video page...");
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                // Log error details
                Console.WriteLine("=== Error Occurred ===");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View();
            }
            finally
            {
                Console.WriteLine("=== Debug Log: UploadVideo Action Completed ===");
            }
        }
    }
}



  