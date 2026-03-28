using System;

public static class CoinEvents
{
    public static event Action<bool> OnCoinResult;

    public static void FireResult(bool isHeads)
    {
        OnCoinResult?.Invoke(isHeads);
    }
}