namespace Internet_1.Models;

public class AdminViewModel
{
    public List<CustomUser> Users { get; set; } = new List<CustomUser>(); // Initialize the Users collection
    public List<Video> Videos { get; set; } = new List<Video>();           

    public AdminViewModel()
    {
        // Alternatively, you can initialize in the constructor like so
        // Users = new List<CustomUser>();
        // Videos = new List<Video>();
    }
}
