namespace BankAccount;

public class BankAccount
{
    public int Balance { get; set; }

    public void Deposit(int cash)
    {
        Balance += cash;
    }
}
