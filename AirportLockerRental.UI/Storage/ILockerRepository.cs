using AirportLockerRental.UI.DTOs;

namespace AirportLockerRental.UI.Storage
{
    public interface ILockerRepository
    {
        void List();
        LockerContents? Get(int number);
        bool IsAvailable(int number);
        bool Add(LockerContents contents);
        LockerContents? Remove(int number);
        int Capacity { get; set; }
        byte[] CreateSalt();
        string HashPassword(string password, byte[] salt);
        bool VerifyPassword(int lockerNumber, string password);
    }
}
