using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BL.Misc
{
    public class Hash
    {
        private static readonly SHA512 hashAlgo = SHA512.Create();

        public static string GetStringHash(string s)
        {
            if (s == null) return null;

            var hash = hashAlgo.ComputeHash(Encoding.Unicode.GetBytes(s));

            return string.Join("", hash.Select(item => item.ToString("x2")));
        }
    }
}