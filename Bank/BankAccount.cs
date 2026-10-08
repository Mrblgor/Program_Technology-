using System.Text;

namespace Bank;


// потомок класса object => можно переопределить 
// виртуальные методы, находящиеся в object
public class BankAccount
{
    private readonly decimal _minimumBalance;
    static private int s_accountNuberSeed = 1000000000;
    public string Number { get; }
    public string Owner { get; private set; }
    public decimal Balance 
    {
        get 
        {
            decimal balance = 0;
            foreach (var item in _allTransactions)
            {
                balance += item.Amount;
            }

            return balance;
        } 
    }

    private List<Transaction> _allTransactions = new List<Transaction>();

    public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0)
    {

    }

    public BankAccount(string name, decimal initialBalance, decimal minimumBalance)
    {

        Owner = name; // this.Owner = name
        Number = s_accountNuberSeed.ToString();
        s_accountNuberSeed++;

        _minimumBalance = minimumBalance;

        if (initialBalance > 0)
            MakeDeposit(initialBalance, DateTime.UtcNow, "Inital balance");
    }
    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        { 
        throw new ArgumentOutOfRangeException(nameof(amount),"Amount of deposit must be positive");
        }

        var deposit = new Transaction(amount, date, note);
        _allTransactions.Add(deposit);
    }

    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Transaction? overdraftTransaction = CheckWithdrawalLimit(Balance - amount < _minimumBalance);
        Transaction? withdrawal = new(-amount, date, note);

        _allTransactions.Add(withdrawal);

        if (overdraftTransaction is not null)
            _allTransactions.Add(overdraftTransaction);
    }

    protected virtual Transaction? CheckWithdrawalLimit(bool isOvesdrawn)
    {
        if (isOvesdrawn)
        {
            throw new InvalidOperationException("Not sufficient rubls for this withdrawal");
        }
        else
        {
            return default;
        }
    }

    public string GetAccountHistory()
    {
        var report = new StringBuilder();
        decimal balance = 0;
        report.Append("Data\t\tAmount\tBalance\tNote");
        foreach (var item in _allTransactions)
        {
            balance += item.Amount;
            report.AppendLine($"" +
                $"{item.Date.ToShortDateString()}\t" +
                $"{item.Amount}\t{balance}\t{item.Note}");
        }
        return report.ToString();
    }
    // Ключевое слово virtual позволяет в дочернем классе
    // предоставить другую реализацию 
    // Метода PerformMonthAndTransactions
    public virtual void PerformMonthAndTransactions()
    {

    }

    // переопределяем метод базового класса - класса object 
    // toString возвращает строку с информацией об объекте
    //public override string ToString()
    //{
    //    return $"Type:{GetType().Name}\tOwner:{Owner}\tNumber of account:{Number}\tBalance:{Balance}";
    //}
    public override string ToString()
    => $"Type:{GetType().Name}\tOwner:{Owner}\tNumber of account:{Number}\tBalance:{Balance}";
}