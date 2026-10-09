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
    /// Создаёт счёт с нулевым минимальным балансом.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный баланс счёта.</param>
    public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0)
    {
    }

    /// <summary>
    /// Создаёт счёт с заданным минимальным балансом.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный баланс счёта.</param>
    /// <param name="minimumBalance">Минимально допустимый баланс (может быть отрицательным — кредитный лимит).</param>
    public BankAccount(string name, decimal initialBalance, decimal minimumBalance)
    {
        Owner = name;
        Number = s_accountNuberSeed.ToString();
        s_accountNuberSeed++;

        _minimumBalance = minimumBalance;

        if (initialBalance > 0)
            MakeDeposit(initialBalance, DateTime.UtcNow, "Inital balance");
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
    /// Выполняет операции, начисляемые раз в месяц.
    /// </summary>
    /// <remarks>Базовая реализация ничего не делает; переопределяется в наследниках.</remarks>
    public virtual void PerformMonthAndTransactions()
    {
    }

    /// <summary>
    /// Возвращает строковое представление счёта.
    /// </summary>
    /// <returns>Строка с типом, владельцем, номером и балансом счёта.</returns>
    public override string ToString()
    => $"Type:{GetType().Name}\tOwner:{Owner}\tNumber of account:{Number}\tBalance:{Balance}";
}