using DCAssignment2A.Models;
using System.Runtime.InteropServices;

namespace DCAssignment2A.Data
{
    // Generates random data into the database
    public static class DataSeeder
    {
        private static readonly int numUsers = 10;     // Determines how many UserProfiles and BankAccounts to make
        public static void Seed(DBManager context)
        {
            // Only seed if empty
            if (context.UserProfiles.Any()) return;

            var random = new Random();

            // Generate random UserProfiles
            var streets = new[] { "Main Street", "High Street", "Willow Grove", "King Street", "Park Avenue", "Green Lane", "Maple Road", "South Grove", "William Street", "Birch Road" };
            var usedPhoneNumbers = new HashSet<string>();
            var usedUsernames = new HashSet<string>();
            var userProfiles = new List<UserProfile>();

            // Generate a random 8-character unique username
            string GenerateUniqueUsername()
            {
                string username;
                do
                {
                    // Random 8-character username
                    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
                    username = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
                } while (!usedUsernames.Add(username));

                return username;
            }

            for (int i = 1; i <= numUsers; i++)
            {
                var name = GenerateUniqueUsername();
                var email = $"{name.ToLower()}@gmail.com";
                var address = $"{random.Next(1, 999)} {streets[random.Next(streets.Length)]}, Perth WA";
                var password = "password{i}";

                // Ensure unique phone number using a hashset
                string phone;
                do
                {
                    phone = $"04{random.Next(10000000, 99999999)}";
                } while (!usedPhoneNumbers.Add(phone));

                var userProfile = new UserProfile
                {
                    UserProfileId = i,
                    name = name,
                    email = email,
                    address = address,
                    phoneNumber = phone,
                    password = password,
                    profilePicture = ProfilePicReader.GetDefaultPFP()
                };

                userProfiles.Add(userProfile);
            }

            context.UserProfiles.AddRange(userProfiles);
            context.SaveChanges();


            // Generate random BankAccounts
            var banks = new[] { "Commonwealth Bank", "ANZ", "Westpac", "NAB", "Bankwest" };
            var usedAccountNumbers = new HashSet<int>();
            var bankAccounts = new List<BankAccount>();

            for (int i = 1; i <= numUsers; i++)
            {
                var user = userProfiles[i - 1]; // zero-based index

                // Ensure unique bank account number using a hash set
                int accountNumber;
                do
                {
                    accountNumber = random.Next(100000, 999999);
                } while (!usedAccountNumbers.Add(accountNumber));

                var bankAccount = new BankAccount
                {
                    BankAccountId = i,
                    BankAccountNumber = accountNumber,
                    BankAccountHolderName = user.name,
                    BankName = banks[random.Next(banks.Length)],
                    balance = Math.Round((decimal)(random.NextDouble() * 10000), 2),   // Random balance 0-9999
                    UserProfileId = user.UserProfileId
                };

                bankAccounts.Add(bankAccount);
            }

            context.BankAccounts.AddRange(bankAccounts);
            context.SaveChanges();


            // Generate random Transactions
            var transactions = new List<Transaction>();
            for (int i = 1; i <= random.Next(numUsers, numUsers*3); i++)
            {
                var account = bankAccounts[random.Next(bankAccounts.Count)];   // Pick random account
                var isWithdrawal = random.Next(2) == 0;    // 50/50 change of withdrawal or deposit
                var amount = Math.Round((decimal)(random.NextDouble() * 1000), 2);

                var transaction = new Transaction
                {
                    TransactionId = i,
                    BankAccountId = account.BankAccountId,
                    Date = DateTime.Now.AddDays(-random.Next(1, 30)),
                    Amount = isWithdrawal ? -amount : amount,   // Negative if withdrawal
                    Description = isWithdrawal ? "Withdrawal" : "Deposit"
                };

                transactions.Add(transaction);
            }

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}
