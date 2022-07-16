namespace HR.BrightspaceConnector.Utilities
{
    /// <summary>
    /// Exposes the Szudzik pairing function and its inverse as static methods.
    /// </summary>
    public static class ElegantPairingFunctions
    {
        public static ulong Pair(uint x, uint y)
        {
            return x >= y ? (ulong)x * x + x + y : (ulong)y * y + x;
        }

        /// <summary>
        /// Note: Due to <see cref="Math.Sqrt"/>, which does floating point calculation, there might be some inaccuracy in the results.
        /// </summary>
        public static void Unpair(ulong z, out uint x, out uint y)
        {
            var sqrtZ = (ulong)Math.Sqrt(z);
            var sqZ = sqrtZ * sqrtZ;

            if ((z - sqZ) >= sqrtZ)
            {
                x = (uint)sqrtZ;
                y = (uint)(z - sqZ - sqrtZ);
            }
            else
            {
                x = (uint)(z - sqZ);
                y = (uint)sqrtZ;
            }
        }
    }
}
