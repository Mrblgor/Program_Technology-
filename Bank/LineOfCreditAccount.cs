using System;
using System.Collections.Generic;
using System.Text;

namespace Bank;

public class LineOfCreditAccount : BankAccount
{
    public LineOfCreditAccount(string name, decimal initialBalance, decimal creditLimit)
        : base(name, initialBalance, -creditLimit)
    {

    }

}