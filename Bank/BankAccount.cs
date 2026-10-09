using System.Text;

namespace Bank;

/// <summary>
/// Базовый банковский счёт: хранит историю операций и вычисляет баланс.
/// </summary>
public class BankAccount
{
    private readonly decimal _minimumBalance;
    static private int s_accountNuberSeed = 1000000000;

    /// <summary>
    /// Уникальный номер счёта, присвоенный при создании.
    /// </summary>
    public string Number { get; }

    /// <summary>
    /// Владелец счёта.
    /// </summary>
    public string Owner { get; private set; }

    /// <summary>
    /// Текущий баланс, вычисляемый как сумма всех операций.
    /// </summary>
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

    /// <summary>
    /// Статус клиента, определяющий условия обслуживания счёта.
    /// </summary>
    public ClientStatus Status { get; private set; } 

    /// <summary>
    /// Создаёт счёт с нулевым минимальным балансом.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный баланс счёта.</param>
    public BankAccount(string name, decimal initialBalance, ClientStatus status= ClientStatus.Regular) : this(name, initialBalance, 0, status)
    {
       
    }

    /// <summary>
    /// Создаёт счёт с заданным минимальным балансом.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный баланс счёта.</param>
    /// <param name="minimumBalance">Минимально допустимый баланс (может быть отрицательным — кредитный лимит).</param>
    public BankAccount(string name, decimal initialBalance, decimal minimumBalance, ClientStatus status = ClientStatus.Regular)
    {
        Status = status;
        Owner = name;
        Number = s_accountNuberSeed.ToString();
        s_accountNuberSeed++;

        _minimumBalance = minimumBalance;

        if (initialBalance > 0)
            MakeDeposit(initialBalance, DateTime.UtcNow, "Inital balance");
    }

    /// <summary>
    /// Начисляет проценты на остаток и списывает комиссию за обслуживание
    /// в соответствии со статусом клиента.
    /// </summary>
    public virtual void PerformMonthAndTransactions()
    {
        decimal interest = Status switch
        {
            ClientStatus.Premium when Balance > 10_000m => Balance * 0.01m,
            ClientStatus.Vip when Balance > 10_000m => Balance * 0.03m,
            _ => 0m
        };

        if (interest > 0m)
            MakeDeposit(interest, DateTime.UtcNow, "Apply month interest");

        decimal fee = Status switch
        {
            ClientStatus.Regular => 50m,
            _ => 0m
        };

        if (fee > 0m && Balance >= fee)
            MakeWithdrawal(fee, DateTime.UtcNow, "Monthly service fee");

        if (Status == ClientStatus.Vip)
        {
            decimal cashback = CalculateCashback();
            if (cashback > 0m)
                MakeDeposit(cashback, DateTime.UtcNow, "Cashback 5%");
        }
    }

    private decimal CalculateCashback()
    {
        decimal withdrawals = 0m;
        foreach (var t in _allTransactions)
        {
            if (t.Amount < 0m && t.Note != "Cashback 5%")
                withdrawals += -t.Amount;
        }
        return withdrawals * 0.05m;
    }

    /// <summary>
    /// Пополняет счёт на указанную сумму.
    /// </summary>
    /// <param name="amount">Сумма пополнения. Должна быть положительной.</param>
    /// <param name="date">Дата операции.</param>
    /// <param name="note">Комментарий к операции.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Бросается, если <paramref name="amount"/> меньше или равен нулю.
    /// </exception>
    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount of deposit must be positive");
        }

        var deposit = new Transaction(amount, date, note);
        _allTransactions.Add(deposit);
    }

    /// <summary>
    /// Списывает со счёта указанную сумму.
    /// </summary>
    /// <param name="amount">Сумма списания. Должна быть положительной.</param>
    /// <param name="date">Дата операции.</param>
    /// <param name="note">Комментарий к операции.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Бросается, если <paramref name="amount"/> меньше или равен нулю.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если после списания баланс выходит за допустимый минимум.
    /// </exception>
    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Transaction? overdraftTransaction = CheckWithdrawalLimit(Balance - amount < _minimumBalance);
        Transaction? withdrawal = new(-amount, date, note);

        _allTransactions.Add(withdrawal);

        if (overdraftTransaction is not null)
            _allTransactions.Add(overdraftTransaction);
    }

    /// <summary>
    /// Проверяет допустимость списания, приводящего к овердрафту.
    /// </summary>
    /// <param name="isOvesdrawn">Признак того, что баланс выйдет за допустимый минимум.</param>
    /// <returns>
    /// Дополнительная транзакция (например, комиссия) или <c>null</c>, если она не требуется.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если овердрафт недопустим для данного типа счёта.
    /// </exception>
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

    /// <summary>
    /// Формирует текстовый отчёт по всем операциям счёта.
    /// </summary>
    /// <returns>Многострочная строка с датой, суммой, балансом и комментарием каждой операции.</returns>
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

    /// <summary>
    /// Возвращает строковое представление счёта.
    /// </summary>
    /// <returns>Строка с типом, владельцем, номером и балансом счёта.</returns>
    public override string ToString()
    => $"Type:{GetType().Name}\tOwner:{Owner}\tNumber of account:{Number}\tBalance:{Balance}";
}