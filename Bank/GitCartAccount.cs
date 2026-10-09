using System;
using System.Collections.Generic;
using System.Text;

namespace Bank;

/// <summary>
/// Подарочная карта с возможностью ежемесячного автоматического пополнения.
/// </summary>
public class GiftCardAccount : BankAccount
{
    private readonly decimal _monthlyDeposit = 0m;

    /// <summary>
    /// Создаёт подарочную карту.
    /// </summary>
    /// <param name="name">Имя владельца карты.</param>
    /// <param name="initialBalance">Начальный баланс карты.</param>
    /// <param name="monthlyDeposit">Сумма ежемесячного автоматического пополнения; по умолчанию 0.</param>
    public GiftCardAccount(string name, decimal initialBalance, decimal monthlyDeposit = 0)
        : base(name, initialBalance)
        => _monthlyDeposit = monthlyDeposit;

    /// <summary>
    /// Вносит ежемесячное пополнение, если оно задано.
    /// </summary>
    public override void PerformMonthAndTransactions()
    {
        if (_monthlyDeposit != 0)
        {
            MakeDeposit(_monthlyDeposit, DateTime.UtcNow, "Add monthly deposit");
        }
    }

    /// <summary>
    /// Возвращает строковое представление карты.
    /// </summary>
    /// <returns>Строка с данными базового счёта и размером ежемесячного пополнения.</returns>
    public override string ToString() => base.ToString() + $"monthly deposit: {_monthlyDeposit}";
}