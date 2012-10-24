using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace BeCharming.Listener
{
    public class Settings
    {
        public static string Name
        {
            get
            {
                return "Share Client";
            }
        }

        public static Boolean OpenItemsAutomatically
        {
            get
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();

                using (StreamReader sr = new StreamReader(new IsolatedStorageFileStream("OpenItemsAutomatically", FileMode.OpenOrCreate, isolatedStorage)))
                {
                    string content = sr.ReadToEnd();

                    if (content != null && content.Length > 0)
                    {
                        return bool.Parse(content);
                    }

                    return false;
                }
            }
            set
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();

                using (StreamWriter sr = new StreamWriter(new IsolatedStorageFileStream("OpenItemsAutomatically", FileMode.Truncate, isolatedStorage)))
                {
                    sr.Write(value);
                }
            }
        }
    }
}
