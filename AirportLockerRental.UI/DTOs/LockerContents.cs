namespace AirportLockerRental.UI.DTOs
{
    public class LockerContents
    {
        public int LockerNumber { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; } 
    }
}
