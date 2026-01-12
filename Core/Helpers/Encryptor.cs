using System;
using System.Security.Cryptography;
using System.Text;

namespace My_program.Views.helper
{
    public static class Encryptor
    {
        public static string MD5Hash(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower(); // คืนค่าเป็นตัวพิมพ์เล็กตามมาตรฐาน MySQL ทั่วไป
            }
        }
    }
}