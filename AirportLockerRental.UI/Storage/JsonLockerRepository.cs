using AirportLockerRental.UI.DTOs;
using System.Security.Cryptography;
using System.Text.Json;

namespace AirportLockerRental.UI.Storage
{
    public class JsonLockerRepository : DictionaryLockerRepository
    {
        private const string _FILE = "lockers.json";
        private const int _PBKDF2_ITERATIONS = 100000;
        private const int _SALT_SIZE = 16;
        private const int _HASH_SIZE = 32;

        public JsonLockerRepository(int capacity) : base(capacity)
        {
            Load();
        }

        public override bool Add(LockerContents contents)
        {
            var result = base.Add(contents);

            if(result)
            {
                Save();
            }

            return result;
        }

        public override LockerContents? Remove(int number)
        {
            var item = base.Remove(number);

            if (item != null) 
            {
                Save();
            }

            return item;
        }

        public byte[] CreateSalt()
        {
            return RandomNumberGenerator.GetBytes(_SALT_SIZE);
        }

        private string HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                _PBKDF2_ITERATIONS,
                HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(_HASH_SIZE);
            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(int lockerNumber, string password)
        {
            var locker = base._storage[lockerNumber];
            byte[] salt = Convert.FromHexString(locker.Salt);
            string hash = HashPassword(password, salt);

            return locker.PasswordHash == hash;
        }

        public void Load()
        {
            if(File.Exists(_FILE))
            {
                string fileJson = File.ReadAllText(_FILE);
                _storage = JsonSerializer.Deserialize<Dictionary<int, LockerContents>>(fileJson) ?? new Dictionary<int, LockerContents>();
            }            
        }

        public void Save()
        {
            string fileContents = JsonSerializer.Serialize(_storage);

            File.WriteAllText(_FILE, fileContents);
        }
    }
}
