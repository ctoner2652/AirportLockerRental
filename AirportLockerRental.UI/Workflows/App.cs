using AirportLockerRental.UI.Actions;
using AirportLockerRental.UI.DTOs;
using AirportLockerRental.UI.Storage;
using Microsoft.Extensions.Configuration;

namespace AirportLockerRental.UI.Workflows
{
    // we only need one App object, so making it static is appropriate
    public static class App
    {

        public static void Run(string key)
        {
            
            // instantiate a locker manager to do the work
            ILockerRepository lockerStorage = ConsoleIO.GetStorageType();
            Encryption encrypt = new Encryption(key);
            while (true)
            {
                int choice = ConsoleIO.GetMenuOption();

                if(choice == 5)
                {
                    return;
                }
                else if(choice == 4)
                {
                    lockerStorage.List();
                }
                else
                {
                    // we need a locker number for these three choices
                    int lockerNumber = ConsoleIO.GetLockerNumber(lockerStorage.Capacity);

                    if(choice == 1)
                    {
                        LockerContents? contents = lockerStorage.Get(lockerNumber);
                        if(contents != null)
                        {
                            var username = ConsoleIO.GetUserName();
                            var password = ConsoleIO.GetPassword();
                            if (contents.UserName == username && lockerStorage.VerifyPassword(lockerNumber, password))
                            {
                                ConsoleIO.DisplayLockerContents(contents, encrypt.Decrypt(contents.Description));
                            }
                            else
                            {
                                Console.WriteLine("Sorry incorrect password/username, please try another time");
                            }
                        }
                        else
                        {
                            Console.WriteLine("That locker is currently empty!");
                        }
                       
                        
                    }
                    else if(choice == 2)
                    {
                        LockerContents contents = ConsoleIO.GetLockerContentsFromUser();
                        
                        if (lockerStorage.IsAvailable(lockerNumber))
                        {
                            contents.LockerNumber = lockerNumber;
                            contents.Description = encrypt.Encrypt(contents.Description);
                            contents.Salt = Convert.ToHexString(lockerStorage.CreateSalt());
                            string password = ConsoleIO.GetPassword();
                            contents.PasswordHash = lockerStorage.HashPassword(password, Convert.FromHexString(contents.Salt));
                            lockerStorage.Add(contents);
                            Console.WriteLine($"Locker {lockerNumber} is rented, stop by later to pick up your stuff!");
                        }
                        else
                        {
                            Console.WriteLine($"Sorry, but locker {lockerNumber} has already been rented!");
                        }                       
                    }
                    else
                    {
                        LockerContents? contents = lockerStorage.Get(lockerNumber);
                        if (contents == null)
                        {
                            Console.WriteLine($"Locker {lockerNumber} is not currently rented.");
                            ConsoleIO.AnyKey();
                            continue;
                        }
                        var username = ConsoleIO.GetUserName();
                        var password = ConsoleIO.GetPassword();
                        if (contents.UserName == username && lockerStorage.VerifyPassword(lockerNumber, password))
                        {
                            contents = lockerStorage.Remove(lockerNumber);
                            Console.WriteLine($"Locker {lockerNumber} rental has ended, please take your {encrypt.Decrypt(contents.Description)}.");
                        }
                        else
                        {
                            Console.WriteLine("Sorry incorrect password/username, please try another time");
                            ConsoleIO.AnyKey();
                            continue;
                        }
                    }
                }

                ConsoleIO.AnyKey();
            }
        }
    }
}
