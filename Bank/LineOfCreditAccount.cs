using System;
using System.Collections.Generic;
using System.Text;

namespace Bank;

/// <summary>
/// Кредитный счёт с разрешённым отрицательным балансом в пределах лимита.
/// </summary>
public class LineOfCreditAccount : BankAccount
{
    /// <summary>
    /// Создаёт кредитный счёт с заданным кредитным лимитом.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный баланс счёта.</param>
    /// <param name="creditLimit">Максимально допустимая сумма овердрафта.</param>
    public LineOfCreditAccount(string name, decimal initialBalance, decimal creditLimit)
        : base(name, initialBalance, -creditLimit)
    {

    }
    /// <summary>
    /// Проверяет допустимость списания, приводящего к овердрафту,
    /// и возвращает транзакцию с комиссией за использование кредитной линии.
    /// </summary>
    /// <param name="isOvesdrawn">Признак того, что баланс выйдет за допустимый минимум.</param>
    /// <returns>Транзакция с комиссией или <c>null</c>, если овердрафт не возникает.</returns>
    protected override Transaction? CheckWithdrawalLimit(bool isOvesdrawn)
        => isOvesdrawn
            ? new Transaction(-20m, DateTime.UtcNow, "Overdraft fee")
            : default;
}