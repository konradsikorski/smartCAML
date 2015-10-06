namespace System
{
    public static class StringExtension
    {
        public static bool ToBool(this string @this, bool defaultValue = default(bool))
        {
            if (string.IsNullOrEmpty(@this)) return defaultValue;

            bool newValue;
            return bool.TryParse(@this, out newValue)
                ? newValue
                : defaultValue;
        }

        public static double ToDouble(this string @this, double defaultValue = default(double))
        {
            if (string.IsNullOrEmpty(@this)) return defaultValue;

            double newValue;
            return double.TryParse(@this, out newValue)
                ? newValue
                : defaultValue;
        }
    }
}
