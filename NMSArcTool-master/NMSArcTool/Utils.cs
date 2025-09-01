namespace NMSArcTool;

public static class Utils
{
    public static ulong ReqChunkBytes(ulong chunkSize)
    {
        return 0x10 * DetermineBins(chunkSize);
    }

    public static ulong DetermineBins(ulong numBytes, ulong binSize = 0x10)
    {
        return (numBytes + binSize - 1) / binSize;
    }

    public static ulong RoundUp(ulong x)
    {
        return (x >> 4 << 4) + (ulong)((x & 0xF) != 0 ? 0x10 : 0);
    }

    public static ulong Padding(ulong x)
    {
        return (0x10 - (x & 0xF)) & 0xF;
    }

    public static bool SafeCompare(int signedNumber, ulong unsignedNumber)
    {
        if (signedNumber < 0)
            return false;

        if (unsignedNumber > int.MaxValue)
            return false;

        return (ulong)signedNumber == unsignedNumber;
    }

    public static string TrimEndString(string input, string suffixToRemove)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(suffixToRemove))
            return input;

        while (input.EndsWith(suffixToRemove))
        {
            input = input.Substring(0, input.Length - suffixToRemove.Length);
        }
        return input;
    }
}
