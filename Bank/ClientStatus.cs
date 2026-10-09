namespace Bank;

/// <summary>
/// Статус клиента банка, определяющий условия обслуживания.
/// </summary>
public enum ClientStatus
{
    /// <summary>Обычный клиент: фиксированная комиссия за обслуживание.</summary>
    Regular,

    /// <summary>Премиум-клиент: без комиссии, повышенный процент на остаток.</summary>
    Premium,

    /// <summary>VIP-клиент: без комиссии, максимальный процент и кэшбэк на списания.</summary>
    Vip
}