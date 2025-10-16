namespace APIClasses
{
    public class BankAccount
    {
        public int BankAccountId { get; set; } //PK
        public int BankAccountNumber { get; set; } //for searching
        public string BankAccountHolderName { get; set; } //additional field for assignment
        public string BankName { get; set; } //additional field for assignment
        public decimal balance { get; set; }
        public int UserProfileId { get; set; } //FK to UserProfile
    }
}
