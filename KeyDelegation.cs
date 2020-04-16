using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Helpers
{
    public class KeyDelegation
    {
        public static List<KeyStore> Temp
        {
            get
            {
                if (IsHttpRuntime() && System.Web.HttpContext.Current.Session["_delegate"] != null)
                {
                    return (List<KeyStore>)System.Web.HttpContext.Current.Session["_delegate"];
                }
                return new List<KeyStore>();
            }
            set
            {
                if (IsHttpRuntime())
                {
                    System.Web.HttpContext.Current.Session["_delegate"] = value;
                }
            }
        }

        private static bool IsHttpRuntime()
        {
            if (HttpRuntime.AppDomainAppId == null)
            {
                throw new ArgumentNullException(@"HttpRuntime.AppDomainAppId is null.  SessionManager can only be used in a web application");
            }
            return true;
        }

        public string GetDelegate(string id)
        {
            var _delegate = GetSession();
            string _key;

            _delegate.Add(new KeyStore()
            {
                Key = randomKey(_delegate, out _key),
                Id = id
            });

            Save(_delegate);

            return _key;
        }

        public string Find(string key)
        {
            var _delegate = GetSession();
            var result = _delegate.Where(x => x.Key == key).FirstOrDefault();
            return result?.Id ?? null;
        }

        private void Save(List<KeyStore> keys)
        {
            Temp = keys;
        }

        private List<KeyStore> GetSession()
        {
            return Temp;
        }

        private string randomKey(List<KeyStore> list, out string key, int attempt = 0, int max = 3)
        {
            // Check max retry
            if ( attempt == max )
            {
                throw new Exception("maximum attempts was reached.");
            }

            // Generate random key
            key = RandomString();

            // Search is key already existing.
            string _key = key;
            var result = list.Where(x => x.Key == _key).FirstOrDefault();
            if ( result != null )
            {
                randomKey(list, out key, ++attempt, max);
            }
            
            return key;
        }

        private static Random random = new Random();

        private static string RandomString(int length = 6)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class KeyStore
    {
        public string Key { get; set; }
        public string Id { get; set; }
    }
}