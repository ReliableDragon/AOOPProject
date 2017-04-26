using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WindowsFormsApplication2
{
    class HashCracker
    {
        public static System.Collections.Concurrent.ConcurrentQueue<string> conQ = new System.Collections.Concurrent.ConcurrentQueue<string>();
        public static System.Collections.Concurrent.ConcurrentQueue<string> conBruteHash = new System.Collections.Concurrent.ConcurrentQueue<string>();
        public static bool threadFound = false;
        public static long threadHashIn = 0;
        public static bool genDict = true;
        public static string threadBruteHash = "a";
        public static int numTested = 0;
        public static string lastTested = "";
        public static char[] chars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
                                       'p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G',
                                       'H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                        '0','1','2','3','4','5','6','7','8','9' };
        public static bool listLocked = false;
        public static Object thisLock = new Object();
        public static List<int> bruteList = new List<int>();
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern bool AllocConsole();

        public static string IncrementString(ref string bruteHash)
        {
            bool addChar = false;
            if (bruteHash[bruteHash.Length - 1] == 'z')
            {
                if (bruteHash.Length > 1)
                {
                    bruteHash = bruteHash.Substring(0, bruteHash.Length - 1) + "A";
                }
                else
                {
                    bruteHash = "A";
                }
            }
            else if (bruteHash[bruteHash.Length - 1] == 'Z')
            {
                if (bruteHash.Length > 1)
                {
                    bruteHash = bruteHash.Substring(0, bruteHash.Length - 1) + 'a';
                    int i;
                    for (i = bruteHash.Length-2; i >= 0 && bruteHash[i] == 'Z'; i--)
                    {

                        bruteHash = bruteHash.Substring(0, i) + "a" + bruteHash.Substring(i+1);

                        if (i == 0)
                        {
                            addChar = true;
                        }
                    }
                    if (addChar)
                    {
                        bruteHash = bruteHash + "a";
                    }
                    else
                    {
                        if (bruteHash[i] != 'z')
                        {
                            bruteHash = bruteHash.Substring(0, i) + ((char)(bruteHash[i] + 1)).ToString() + bruteHash.Substring(i + 1);
                        }
                        else
                        {
                            bruteHash = bruteHash.Substring(0, i) + "A" + bruteHash.Substring(i + 1);

                        }
                    }
                }
                else
                {
                    bruteHash = "aa";
                }
            }
            else
            {
                if (bruteHash.Length > 1)
                {
                    bruteHash = bruteHash.Substring(0, bruteHash.Length - 1) + (char)(bruteHash[bruteHash.Length - 1] + 1);
                }
                else
                {
                    bruteHash = ((char)(bruteHash[0] + 1)).ToString();
                }
            }
            return bruteHash;
        }

        public static void IncString5thDigit(ref List<int> bruteHash)
        {
            lock (thisLock)
            {
                if (bruteHash.Count > 4)
                {
                    int currChar = bruteHash.Count - 5;
                    bool keepGoing = true;
                    while (keepGoing && currChar >= 0)
                    {
                        if (bruteHash[currChar] != 61)
                        {
                            ++bruteHash[currChar];
                            keepGoing = false;
                        }
                        else
                        {

                            bruteHash[currChar] = 0;
                            --currChar;
                        }
                    }
                    if (keepGoing)
                    {
                        bruteHash.Add(0);
                    }
                    string outStr = "";
                    foreach (var c in bruteHash)
                    {
                        outStr += chars[c];
                    }
                }
                else
                {
                    while (bruteHash.Count < 5)
                        bruteHash.Add(0);
                }
            }
        }

        public static string IncString(ref List<int> bruteHash)
        {
            //char[] chars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            //                   'p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G',
            //                   'H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            //                    '0','1','2','3','4','5','6','7','8','9'};
            
            int currChar = bruteHash.Count- 1;
            bool keepGoing = true;
            while (keepGoing && currChar >= 0)
            {
                numTested++;
                if (numTested % 62 == 0)
                {
                    int jkj = 5;
                }
                if (bruteHash[currChar] != 61)
                {
                    ++bruteHash[currChar];
                    keepGoing = false;
                }
                else
                {
                    bruteHash[currChar] = 0;
                    --currChar;
                }
            }
            if (keepGoing)
            {
                bruteHash.Add(0);
            }
            string outStr = "";
            foreach (var c in bruteHash)
            {
                outStr += chars[c];
            }

            return outStr;
        }

        public static long Hash(string toHash)
        {
            long hashedNum = 10937;
            for (int i = 0; i < toHash.Length; i++)
            {
                hashedNum += (toHash[i] * (i + 1)) * (toHash[i] * (i + 1)) % hashedNum;
            }
            hashedNum = hashedNum % 10000000000;
            string temp = hashedNum.ToString();
            for (int i = 0; temp.Length < 10; i++)
            {
                temp = temp.Insert((i + 1) * (i + 1) % temp.Length, i.ToString());
            }
            hashedNum = Convert.ToInt64(temp);
            return hashedNum;
        }

        public static void DoCracking()
        {
            AllocConsole();
            genDict = false;
            System.IO.StreamReader inFile;
            System.IO.StreamWriter outFile;
            try
            {
                inFile = new System.IO.StreamReader("hashdictionary.txt");
            }
            catch (System.IO.FileNotFoundException)
            {
                genDict = true;
            }

            if (genDict)
            {
                using (inFile = new System.IO.StreamReader("dictionary.txt"))
                using (outFile = new System.IO.StreamWriter("hashdictionary.txt"))
                {
                //System.IO.StreamReader inFile = new System.IO.StreamReader("dictionary.txt");
                //System.IO.StreamWriter outFile = new System.IO.StreamWriter("hashdictionary.txt");

                    string word = inFile.ReadLine();
                    while (!inFile.EndOfStream)
                    {
                        long hash = Hash(word);
                        outFile.WriteLine(hash.ToString() + ": " + word);
                        word = word.Substring(0, 1).ToUpper() + word.Substring(1, word.Length - 1);
                        hash = Hash(word);
                        outFile.WriteLine(hash.ToString() + ": " + word);
                        word = inFile.ReadLine();
                    }
                    long hash2 = Hash("");
                    outFile.WriteLine(hash2.ToString() + ": [blank]");
                }
                outFile.Close();
                inFile.Close();
            }
            using (inFile = new System.IO.StreamReader("hashdictionary.txt"))
            {

                long hashIn = 0;
                bool keepGoing = false;
                bool found = false;

                Console.WriteLine("Hash to lookup?");
                try
                {
                    hashIn = Convert.ToInt64(Console.ReadLine());
                    keepGoing = true;
                }
                catch
                {
                    Console.WriteLine("Invalid input.");
                    keepGoing = true;
                }

                while (hashIn != -1)
                {
                    threadFound = false;
                    found = false;
                    //inFile = new System.IO.StreamReader("hashdictionary.txt");
                    while (!inFile.EndOfStream && keepGoing)
                    {
                        string line = inFile.ReadLine();
                        try
                        {
                            if (Convert.ToInt64(line.Substring(0, 10)) == hashIn)
                            {
                                Console.WriteLine(line.Substring(12));
                                found = true;
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("Too short to be a valid hash.");
                        }
                    }
                    if (!found && keepGoing)
                    {
                        char inChar;
                        Console.WriteLine("No hashes found in dictionary. Attempt brute force? (Y/N)");
                        inChar = Convert.ToChar(Console.Read());
                        Console.ReadLine(); // Eat endline character from the previous read.
                        if (inChar == 'Y')
                        {
                            bruteList.Add(0);
                            conQ.Enqueue("a");
                            System.Diagnostics.Stopwatch howLong = new System.Diagnostics.Stopwatch();
                            howLong.Start();
                            bool loop = true;
                            //string bruteHash = "a";
                            threadHashIn = hashIn;
                            conBruteHash.Enqueue("a");

                            //var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                            //var random = new Random();
                            for (int i = 0; i < 8; i++)
                            {
                                System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ThreadStart(GenThreadFunc));
                                newThread.Start();
                                //conBruteHash.Enqueue(new string(
                                //    Enumerable.Repeat(chars, 6)
                                //              .Select(s => s[random.Next(s.Length)])
                                //              .ToArray()));
                            }
                            for (int i = 0; i < 24; i++)
                            {
                                System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFunc));
                                newThread.Start();
                            }
                            //int garColl = 15000;
                            while (loop && !threadFound)
                            {
                                //conQ.Enqueue(bruteHash);
                                //IncrementString(ref bruteHash);

                                //System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFunc));
                                //newThread.Start();

                                //if (Hash(bruteHash) == hashIn)
                                //{
                                //    Console.WriteLine("Match found: " + bruteHash);
                                //    //loop = false;
                                //}
                                //if (howLong.ElapsedMilliseconds % 1000 == 0)
                                //    Console.WriteLine(lastTested);

                                //if (howLong.ElapsedMilliseconds > garColl)
                                //{
                                //    GC.Collect();
                                //    garColl += 15000;
                                //}
                                if (howLong.ElapsedMilliseconds > 240000)
                                {
                                    loop = false;
                                    threadFound = true;
                                    Console.WriteLine("Time limit exceeded. " + numTested + " hashes tried.");
                                }
                                System.Threading.Thread.Sleep(25);
                                //    IncrementString(ref bruteHash);
                                //    ++numTested;
                                //    if (numTested % 100000 == 0)
                                //        Console.WriteLine(bruteHash);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Skipping brute force.");
                        }
                    }
                    inFile.DiscardBufferedData();
                    inFile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    inFile.BaseStream.Position = 0;
                    Console.WriteLine("Hash to lookup?");
                    try
                    {
                        hashIn = Convert.ToInt64(Console.ReadLine());
                        if (hashIn != -1)
                        {
                            keepGoing = true;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input.");
                        keepGoing = false;
                    }
                }
            }
        }

        public static void ThreadFunc()
        {
                string bruteString = "";
                while (!threadFound && conQ.TryDequeue(out bruteString))
                {
                    if (Hash(bruteString) == threadHashIn)
                    {
                        //threadFound = true;
                        Console.WriteLine("Match found in thread - " + bruteString);
                        threadFound = true;
                    }
                    ++numTested;
                    //lastTested = bruteString;
                    //Console.WriteLine(bruteString);
                }
        }

        public static void GenThreadFunc()
        {
            //string personalBrute = "";
            //while (!threadFound && conBruteHash.TryDequeue(out personalBrute))
            //{
            //    conQ.Enqueue(IncrementString(ref personalBrute));
            //    conBruteHash.Enqueue(personalBrute);
            //}
            List<int> perList = new List<int>();
            while (!threadFound)
            {
                perList.Clear();
                lock(thisLock)
                {
                    foreach (var c in bruteList)
                    {
                        perList.Add(c);
                    }

                    IncString5thDigit(ref bruteList);
                }
                Console.WriteLine("Thread starting with " + IncString(ref perList));
                if (perList.Count == 1)
                {
                    for (int i = 0; i < 15018570 && !threadFound; i++)
                    {
                        conQ.Enqueue(IncString(ref perList));
                        if (numTested % 100000 == 0)
                            Console.WriteLine(IncString(ref perList));
                        if (conQ.Count > 100000)
                            System.Threading.Thread.Sleep(25);
                    }
                }
                else
                {
                    for (int i = 0; i < 14776336 && !threadFound; i++)
                    {
                        conQ.Enqueue(IncString(ref perList));
                        if (numTested % 100000 == 0)
                            Console.WriteLine(IncString(ref perList));
                        if (conQ.Count > 100000)
                            System.Threading.Thread.Sleep(25);
                    }
                }
            }
        }
    }
}
