using System.Linq.Expressions;
using System.Text;

namespace Bank;

internal class Bank_Account
{
    private List<Transaction> _alltransactions = new List<Transaction>();
    public string Owner { get; private set; }
    public decimal Balance 
    {
        get
        {
            decimal balance = 0;
            foreach (var transaction  in _alltransactions)
            {
                balance += transaction.Amount;
            }
            return balance;
        }
    }
    public string Number { get; private set; }
    private static int s_accountNumberSeed = 1000000000;
    public Bank_Account(string name, decimal initialbalance)
    {
        Owner = name; // this.Owner = name;
        MakeDeposit(initialbalance, DateTime.UtcNow, "initial balance");
        Number = s_accountNumberSeed.ToString();
        s_accountNumberSeed++;
    }
    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amout of deposit must be positive");
        }
        var deposite = new Transaction(amount, date, note);
        _alltransactions.Add(deposite);
    }
    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amout of withdawal deposit must be positive");
        }

        if (Balance < amount)
        {
            throw new InvalidOperationException("Not sufficient rubls for this withdrawal");
        }
        var withdrawal = new Transaction(-amount, date, note); 
        _alltransactions.Add(withdrawal);
    }
    public string GetAccountHistory()
    {
        var report = new StringBuilder();

        decimal balance = 0;
        report.AppendLine("Data\t\tAmount\tBalance\tNote");
        foreach(var item in  _alltransactions)
        {
            balance += item.Amount;
            report.AppendLine($"" + $"{item.Date.ToShortDateString()}" + $"{item.Amount}\t{balance}\t{item.Note}");
        }
        return report.ToString();
    }
}
