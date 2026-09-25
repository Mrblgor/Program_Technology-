namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bank_Account account1 = new Bank_Account("Igor", 100000000000);
            Bank_Account account2 = new Bank_Account("Sasha", 809);
            Console.WriteLine($"account: {account1.Owner} {account1.Balance} {account1.Number}");
            Console.WriteLine($"account: {account2.Owner} {account2.Balance} {account2.Number}");
            account1.MakeDeposit(10000, DateTime.UtcNow, ":)");
            Console.WriteLine(account1.Balance);
            account1.MakeWithdrawal(100, DateTime.UtcNow, ":(");
            Console.WriteLine(account1.Balance);
             
            try


            {
                account2.MakeWithdrawal(1000, DateTime.UtcNow, ":(");
                Console.WriteLine(account2.Balance);
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
