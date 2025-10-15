namespace DCAssignment2A.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; } //PK
        public int BankAccountId { get; set; } // Foreign key to BankAccount
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
