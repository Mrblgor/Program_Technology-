namespace Bank;

/// <summary>
/// Неизменяемая запись об одной банковской операции.
/// </summary>
/// <param name="Amount">Сумма операции: положительная — пополнение, отрицательная — списание.</param>
/// <param name="Date">Дата и время совершения операции.</param>
/// <param name="Note">Текстовый комментарий к операции.</param>
public record Transaction(decimal Amount, DateTime Date, string Note);