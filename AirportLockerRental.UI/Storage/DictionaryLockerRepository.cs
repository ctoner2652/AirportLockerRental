using System.Security.Cryptography;
using System.Text;
using AirportLockerRental.UI.Actions;
using AirportLockerRental.UI.DTOs;

namespace AirportLockerRental.UI.Storage
{
    public class DictionaryLockerRepository : ILockerRepository
    {
        private const int _PBKDF2_ITERATIONS = 100000;
        private const int _SALT_SIZE = 16;
        private const int _HASH_SIZE = 32;

        protected Dictionary<int, LockerContents> _storage = new Dictionary<int, LockerContents>();

        public int Capacity { get; set; }

        public DictionaryLockerRepository(int capacity)
        {
            Capacity = capacity;
        }

        public virtual LockerContents? Remove(int number)
        {
            if (_storage.ContainsKey(number))
            {
                var item = _storage[number];
                _storage.Remove(number);
                return item;
            }

            return null;
        }

        public LockerContents? Get(int number)
        {
            if(_storage.ContainsKey(number))
            {
                return _storage[number];
            }

            return null;
        }

        public bool IsAvailable(int number)
        {
            return !_storage.ContainsKey(number);
        }

        public void List()
        {
            foreach(var key in _storage.Keys)
            {
                ConsoleIO.DisplayAllLockerContents(_storage[key]);
            }
        }

        public virtual bool Add(LockerContents contents)
        {
            if(_storage.ContainsKey(contents.LockerNumber) || _storage.Count >= Capacity)
            {
                return false;
            }

            _storage.Add(contents.LockerNumber, contents);

            return true;
        }

        public byte[] CreateSalt()
        {
            return RandomNumberGenerator.GetBytes(_SALT_SIZE);
        }

        public string HashPassword(string password, byte[] salt)
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
            var locker = _storage[lockerNumber];
            byte[] salt = Convert.FromHexString(locker.Salt);
            string hash = HashPassword(password, salt);

            return locker.PasswordHash == hash;
        }

    }
}
