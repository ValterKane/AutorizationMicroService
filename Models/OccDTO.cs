namespace AutorizationMicroService.Models
{
    public class OccDTO
    {
        public int OccId { get; set; }

        public string Country { get; set; } = null!;

        public string Region { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string Appartments { get; set; } = null!;
    }
}
