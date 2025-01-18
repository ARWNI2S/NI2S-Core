namespace ARWNI2S.Extensibility
{
    internal static class ModuleNameStringExtensions
    {
        internal static string ToSystemName(this Type type)
        {
            var result = type.Name;
            var hasNiisMark = false;

            result = result.Replace("Module", "");
            if (result.Contains("NI2S"))
            {
                hasNiisMark = true;
                result = result.Replace("NI2S", "");
            }

            for (int i = 1; i < result.Length; i++)
            {
                if (char.IsUpper(result[i]))
                {
                    result = result.Insert(i, ".");
                    i++;
                }
            }

            if (hasNiisMark)
                result = "NI2S." + result;

            return result;
        }

        internal static string ToDisplayName(this Type type)
        {
            var result = type.Name;
            result = result.Replace("Module", "");
            if (result.Contains("NI2S"))
            {
                result = result.Replace("NI2S", "");
            }
            for (int i = 1; i < result.Length; i++)
            {
                if (char.IsUpper(result[i]))
                {
                    result = result.Insert(i, " ");
                    i++;
                }
            }
            return result;
        }
    }
}
