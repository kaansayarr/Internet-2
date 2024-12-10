namespace Internet_1.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }

}


