using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class Hasher
    {
        public static long GetHash(string hashIn)
        {
            string toHash = hashIn;
            long hashedNum = 10937;
            for (int i = 0; i < toHash.Length; i++)
            {
                hashedNum += (toHash[i] * (i + 1)) * (toHash[i] * (i + 1)) % hashedNum;
            }
            hashedNum = hashedNum % 10000000000;
            string temp = hashedNum.ToString();
            for (int i = 0; temp.Length < 10; i++)
            {
                temp = temp.Insert(((i+1) * (i+1)) % temp.Length, i.ToString());
            }
            hashedNum = Convert.ToInt64(temp);
            return hashedNum;
        }
    }
}
