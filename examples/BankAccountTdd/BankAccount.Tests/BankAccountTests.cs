using BankAccount;

namespace BankAccount.Tests;

[TestClass]
public class BankAccountTests
{
    [TestMethod]
    public void Deposit_100_IncreasesBalanceBy100()
    {
        // Arrange
        BankAccount bankAccount = new BankAccount();
        bankAccount.Balance = 0;

        // Act
        bankAccount.Deposit(100);

        // Assert
        Assert.AreEqual(100, bankAccount.Balance);
    }
}
