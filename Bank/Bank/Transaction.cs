namespace Bank;
// мы создали неизменяемый тип данных
internal record Transaction(decimal Amount, DateTime Date, string Note);
