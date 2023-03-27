using Microsoft.CodeAnalysis.Differencing;
using System.Security.Cryptography;
using System.Text;

namespace DeliveryProject.Helpers
{
    public class HelperCryptography
    {
        public static string GenerateSalt()
        {
            Random random = new Random();
            string salt = "";
            for (int i = 1; i <= 50; i++)
            {
                int aleat = random.Next(0, 255);
                char letter = Convert.ToChar(aleat);
                salt += letter;
            }
            return salt;
        }

        public static bool CompareArrays(byte[] a, byte[] b)

        {
            bool same = true;

            if (a.Length != b.Length)
            {
                same = false;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].Equals(b[i]) == false)
                    {
                        same = false;
                        break;
                    }
                }
            }
            return same;
        }

        public static byte[] EncryptPassword(string password, string salt)
        {
            string content = password + salt;
            SHA512 sHA = SHA512.Create();
            byte[] exit = Encoding.UTF8.GetBytes(content);

            for (int i = 1; i <= 107; i++)
            {
                exit = sHA.ComputeHash(exit);
            }

            sHA.Clear();

            return exit;
        }
    }
}
