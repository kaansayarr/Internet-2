using Microsoft.AspNetCore.Mvc;
using Internet_1.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Internet_1.Controllers
{


    public class VideoController : Controller
    {


        // GET: Video
        [HttpGet]
        public IActionResult Video()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            var videos = new List<Video>();

            try
            {
                using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    connection.Open();
                    string query = "SELECT Id, Title, Url, Description FROM Videos";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var video = new Video
                                {

                                    Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Url = reader.GetString(2),
                                    Description = reader.GetString(3)
                                };

                                Console.WriteLine($"Fetched Video: Id = {video.Id}, Title = {video.Title}");
                                videos.Add(video);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            // Pass the list of videos to the view
            return View(videos);
        }

        // Other actions (AddVideo, etc.)
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("Admin/DeleteVideo/{id}")]

        public IActionResult DeleteVideo(int id)
        {
            Console.WriteLine($"Deleting video with ID: {id}");
            try
            {
                using (var connection = new SqlConnection(ConnectionHelper.GetConnectionString()))

                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Videos WHERE Id = @Id";
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Video deleted successfully!";
                return RedirectToAction("Index", "Admin"); // Redirect to the list page after deletion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Admin"); // Redirect in case of error
            }
        }
    }
}
