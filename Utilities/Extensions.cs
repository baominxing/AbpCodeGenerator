using System.Globalization;

namespace ABPCodeGenerator.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// 将字符串转换成小驼峰式大小写:small house->small House
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLowerCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// 将字符串转换成驼峰式大小写:red house->Red House
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLowerFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToLower(str[0]) + str.Substring(1);
            }

            return str.ToLower();
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str.ToUpper();
        }
    }
}
