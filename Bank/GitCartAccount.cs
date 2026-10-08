using System;
using System.Collections.Generic;
using System.Text;

namespace Bank;
public class GiftCardAccount : BankAccount
{
    private readonly decimal _monthlyDeposit = 0m;

    // monthlyDeposit - параметр по умолчанию, по умолчанию принимает 0,
    // при создании new GiftCardAccount("Ulyana", 1000); - monthlyDeposit = 0
    // new GiftCardAccount("Ulyana", 1000, 5000) => monthlyDeposit = 5000
    public GiftCardAccount(string name, decimal initialBalance, decimal monthlyDeposit = 0)
        : base(name, initialBalance)
        => _monthlyDeposit = monthlyDeposit;

    public override void PerformMonthAndTransactions()
    {
        if (_monthlyDeposit != 0)
        {
            MakeDeposit(_monthlyDeposit, DateTime.UtcNow, "Add monthly deposit");
        }
    }
    public override string ToString() => base.ToString() + $"monthly deposit: {_monthlyDeposit}";
}