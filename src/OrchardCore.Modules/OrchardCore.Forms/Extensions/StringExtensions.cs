//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Text;
//using System.Text.RegularExpressions;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace OrchardCore.Forms.Extensions
//{
//    public static class StringExtensions
//    {
//        public static bool ValidateInputByRule(this string type, string input, string option)
//        {
//            switch (type)
//            {
//                case "contains":
//                    return input.Contains(option, StringComparison.InvariantCulture);
//                case "equals":
//                    return input.Equals(option, StringComparison.InvariantCultureIgnoreCase);
//                case "isAfter":
//                    return CompareDatetime(input, option, ">");
//                case "isBefore":
//                    return CompareDatetime(input, option, "<");
//                case "isBoolean":
//                    return Boolean.TryParse(input, out _);
//                case "isByteLength":
//                    return ValidateLength(Encoding.UTF8.GetByteCount(input), option);
//                case "isDate":
//                    return DateTime.TryParse(input, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out _);
//                case "isDecimal":
//                    return ValidateIs<decimal>(input);
//                case "isDivisibleBy":
//                    if (Single.TryParse(input, out var originalNumber) && Int32.TryParse(option, out var divisor))
//                    {
//                        if (divisor == 0) return false;
//                        return originalNumber % divisor == 0;
//                    }
//                    return false;
//                case "isEmpty":
//                    return String.IsNullOrEmpty(input);
//                case "isFloat":
//                    if (!Single.TryParse(input, out var original)) return false;
//                    float min;
//                    var obj = JToken.Parse(option);
//                    Single.TryParse(obj["max"]?.ToString(), out var max);
//                    Single.TryParse(obj["min"]?.ToString(), out min);
//                    if (original >= min && (max == 0 || original <= max)) return true;
//                    return false;
//                case "isInt":
//                    return ValidateIs<int>(input);
//                case "isJSON":
//                    if (String.IsNullOrWhiteSpace(input)) return false;
//                    var value = input.Trim();
//                    if ((value.StartsWith("{") && value.EndsWith("}")) || // For object.
//                        (value.StartsWith("[") && value.EndsWith("]"))) // For array.
//                    {
//                        try
//                        {
//                            var ob = JToken.Parse(value);
//                            return true;
//                        }
//                        catch (JsonReaderException)
//                        {
//                            return false;
//                        }
//                    }
//                    return false;
//                case "isLength":
//                    return ValidateLength(input.Length, option);
//                case "isNumeric":
//                    var exp = @"^[0-9]+$";
//                    return Regex.IsMatch(input, exp);
//                case "matches":
//                    option = option.Replace("|-BackslashPlaceholder-|", "\\");
//                    return Regex.IsMatch(input, option);
//                default:
//                    return false;
//            }
//        }


//    }
//}
