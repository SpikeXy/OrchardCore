using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Scripting;

namespace OrchardCore.Forms.Entities.Scripting
{
    public class ValidateRuleMethod : IGlobalMethodProvider
    {
        private readonly GlobalMethod _contains;
        private readonly GlobalMethod _equals;
        private readonly GlobalMethod _isAfter;
        private readonly GlobalMethod _isBefore;
        private readonly GlobalMethod _isBoolean;
        private readonly GlobalMethod _isByteLength;
        private readonly GlobalMethod _isDate;
        private readonly GlobalMethod _isDecimal;
        private readonly GlobalMethod _isDivisibleBy;
        private readonly GlobalMethod _isEmpty;
        private readonly GlobalMethod _isFloat;
        private readonly GlobalMethod _isInt;
        private readonly GlobalMethod _isJSON;
        private readonly GlobalMethod _isLength;
        private readonly GlobalMethod _isNumeric;
        private readonly GlobalMethod _matches;

        public ValidateRuleMethod()
        {
            _contains = new GlobalMethod
            {
                Name = "contains",
                Method = serviceProvider => (Func<string, string, bool>)((str, compare) =>
                {
                    return str.Contains(compare, StringComparison.InvariantCulture);
                })
            };
            _equals = new GlobalMethod
            {
                Name = "equals",
                Method = serviceProvider => (Func<string, string, bool>)((str, compare) =>
                {
                    return str.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
                })
            };
            _isAfter = new GlobalMethod
            {
                Name = "isAfter",
                Method = serviceProvider => (Func<string, string, bool>)((str, compare) =>
                {
                    var originResult = DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var originDate);
                    var compareResult = DateTime.TryParse(compare, CultureInfo.InvariantCulture, DateTimeStyles.None, out var compareDate);
                    return originResult && compareResult && originDate > compareDate;
                })
            };
            _isBefore = new GlobalMethod
            {
                Name = "isBefore",
                Method = serviceProvider => (Func<string, string, bool>)((str, compare) =>
                {
                    var originResult = DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var originDate);
                    var compareResult = DateTime.TryParse(compare, CultureInfo.InvariantCulture, DateTimeStyles.None, out var compareDate);
                    return originResult && compareResult && originDate < compareDate;
                })
            };
            _isBoolean = new GlobalMethod
            {
                Name = "isBoolean",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    return Boolean.TryParse(str, out var flag);
                })
            };
            _isByteLength = new GlobalMethod
            {
                Name = "isByteLength",
                Method = serviceProvider => (Func<string, string, bool>)((str, option) =>
                 {
                     return ValidateLength(Encoding.UTF8.GetByteCount(str), option);
                 })
            };
            _isDate = new GlobalMethod
            {
                Name = "isDate",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    return DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
                })
            };
            _isDecimal = new GlobalMethod
            {
                Name = "isDecimal",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    return ValidateIs<decimal>(str);
                })
            };
            _isDivisibleBy = new GlobalMethod
            {
                Name = "isDivisibleBy",
                Method = serviceProvider => (Func<string, string, bool>)((str, compare) =>
                {
                    if (Single.TryParse(str, out var originalNumber) && Int32.TryParse(compare, out var divisor))
                    {
                        if (divisor == 0) return false;
                        return originalNumber % divisor == 0;
                    }
                    else
                    {
                        return false;
                    }
                })
            };
            _isEmpty = new GlobalMethod
            {
                Name = "isEmpty",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    return String.IsNullOrEmpty(str);
                })
            };
            _isFloat = new GlobalMethod
            {
                Name = "isFloat",
                Method = serviceProvider => (Func<string, string, bool>)((str, option) =>
                 {
                     if (!Single.TryParse(str, out var original)) return false;
                     float min = 0;
                     var max = Single.MaxValue;
                     var obj = JToken.Parse(option);
                     Single.TryParse(obj["max"]?.ToString(), out max);
                     Single.TryParse(obj["min"]?.ToString(), out min);
                     if (original >= min && (max == 0 || original <= max)) return true;
                     return false;
                 })
            };
            _isInt = new GlobalMethod
            {
                Name = "isInt",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    return ValidateIs<int>(str);
                })
            };
            _isJSON = new GlobalMethod
            {
                Name = "isJSON",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    if (String.IsNullOrWhiteSpace(str)) return false;
                    var value = str.Trim();
                    if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                        (value.StartsWith("[") && value.EndsWith("]"))) //For array
                    {
                        try
                        {
                            var obj = JToken.Parse(value);
                            return true;
                        }
                        catch (JsonReaderException)
                        {
                            return false;
                        }
                    }
                    return false;
                })
            };
            _isLength = new GlobalMethod
            {
                Name = "isLength",
                Method = serviceProvider => (Func<string, string, bool>)((str, option) =>
                 {
                     return ValidateLength(str.Length, option);
                 })
            };
            _isNumeric = new GlobalMethod
            {
                Name = "isNumeric",
                Method = serviceProvider => (Func<string, bool>)((str) =>
                {
                    var exp = @"^[0-9]+$";
                    return Regex.IsMatch(str, exp);
                })
            };
            _matches = new GlobalMethod
            {
                Name = "matches",
                Method = serviceProvider => (Func<string, string, bool>)((str, regexStr) =>
                {
                    regexStr = regexStr.Replace("|-BackslashPlaceholder-|", "\\");
                    return Regex.IsMatch(str, regexStr);
                })
            };
        }

        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[]
            {
                _contains, _equals, _isAfter, _isBefore, _isBoolean, _isByteLength,
                _isDate, _isDecimal, _isDivisibleBy, _isEmpty, _isFloat,
                _isInt, _isJSON, _isLength, _isNumeric, _matches
            };
        }

        public static bool ValidateLength(int len, string option)
        {
            try
            {
                var min = 0;
                var max = Int32.MaxValue;
                var obj = JToken.Parse(option);
                Int32.TryParse(obj["max"]?.ToString(), out max);
                Int32.TryParse(obj["min"]?.ToString(), out min);
                if (len >= min && (max == 0 || len <= max)) return true;
                return false;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public static bool ValidateIs<T>(string value)
        {
            if (value == null) return false;
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            try
            {
                var result = (T)converter.ConvertFromString(value.ToString());
                return result != null;
            }
            catch
            {
                return false;
            }
        }
    }
}