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
}