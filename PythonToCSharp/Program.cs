using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PythonToCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Please Input URL: ");
            //string url = Console.ReadLine().Replace("\n", "").Replace("\r", "");
            //Console.WriteLine("Please Client Date: ");
            //string client_date = Console.ReadLine().Replace("\n", "").Replace("\r", "");
            //Console.WriteLine("Please Input Post Parames: ");
            //string post_params = Console.ReadLine().Replace("\n", "").Replace("\r", "");

            string url = "/games/send_message";
            string client_date = "2015-03-07 14:19:30";
            string post_params = "game_id=5933472609730129&text=text";

            AuthKey authkey = new AuthKey();
            Console.WriteLine("Authentication key is : ");
            Console.WriteLine(authkey.get_authorization_code(url, client_date, post_params));
        }
    }

    class AuthKey
    {
        public string host_name = "qkgermany.feomedia.se";
        public string authorization_key = "AIcaqRff3zdCyoBT";
        public string scramble_authorization_code(string data, int key, int reverse)
        {
            string result = "";
            reverse += 1;
            int l = (int)(data.Length/key);
            if(l>0)
            {
                if(reverse%2==0)
                {
                    result += scramble_authorization_code(data.Substring(l),key, reverse);
                    result += scramble_authorization_code(data.Substring(0,l),key, reverse);
                }
                else
                {
                    result += scramble_authorization_code(data.Substring(0,l),key, reverse);
                    result += scramble_authorization_code(data.Substring(l),key, reverse);
                }
                return result;
            }
            return data;
        }
        public string get_authorization_code(string url, string client_date, string post_params = "")
        {
            string msg = "https://" + this.host_name + url + client_date;
            if (!String.IsNullOrEmpty(post_params))
            {
                string paramValues = "";
                List<string> valList = new List<string>();
                foreach(string param in post_params.Split('&'))
                {
                    string[] bits = param.Split('=');
                    valList.Add(bits[bits.Length - 1]);
                }
                valList.Sort();
                foreach (string val in valList)
                {
                    paramValues += val;
                }
                msg += paramValues;
            }
            
            msg = System.Text.RegularExpressions.Regex.Replace(msg, "[^-abefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPRSTUXYZ012346789 ,.()]", "");
            
            int[] keyarr = {2,3,5};
            foreach(int key in keyarr)
            {
                msg = this.scramble_authorization_code(msg, key, 0);
            }
            Console.WriteLine("Message is " + msg);
            HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(this.authorization_key));  
            return DecodeFrom64(EncodeTo64(Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(msg)))));
        }
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
