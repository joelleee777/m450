namespace M450.Bankkonto;

/// <summary>
/// Status eines Bankkunden. Beeinflusst ab CHF 50'000 Guthaben den
/// effektiven Aktivzins.
/// </summary>
public enum KontoStatus
{
    Standard,
    VIP
}
