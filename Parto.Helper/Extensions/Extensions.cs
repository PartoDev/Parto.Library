using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parto.Helper.Extensions
{
    public static class Extensions
    {
        public const string PhoneNumberRegexPattern = @"^(\+98|0)?(9\d{9})$";

        public const string EmailRegexPattern =
            @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*)@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";

        public static void Use<TIn>(this TIn @in, Action<TIn> action) => action(@in);

        public static TOut Use<TIn, TOut>(this TIn @in, Func<TIn, TOut> func) => func(@in);

        public static async ValueTask<TOut> UseAsync<TIn, TOut>(this TIn @in, Func<TIn, ValueTask<TOut>> func) =>
            await func(@in)
                .ConfigureAwait(false);

        public static bool IsPhoneNumber(this string phoneNumber) =>
            Regex.Match(phoneNumber, PhoneNumberRegexPattern)
                .Success;

        public static bool IsEmail(this string email) => Regex.Match(email, EmailRegexPattern)
            .Success;

        public static string ConvertPersianStringNumber(this string number)
        {
            number = number.Replace('۰', '0');
            number = number.Replace('۱', '1');
            number = number.Replace('۲', '2');
            number = number.Replace('۳', '3');
            number = number.Replace('۴', '4');
            number = number.Replace('۵', '5');
            number = number.Replace('۶', '6');
            number = number.Replace('۷', '7');
            number = number.Replace('۸', '8');
            number = number.Replace('۹', '9');
            return number;
        }
    }
}