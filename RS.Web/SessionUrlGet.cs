using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    class SessionUrlGet : Dictionary<string, DynamicObj>
    {
        public new DynamicObj this[string key]
        {
            get
            {
                DynamicObj ae = null;
                if (ContainsKey(key))
                {
                    try
                    {
                        ae = base[key];
                    }
                    catch { }
                }
                else
                {
                    ae = new DynamicObj();
                    base[key] = ae;
                }
                if (ae == null)
                {
                    ae = new DynamicObj();
                    base[key] = ae;
                }
                return ae;
            }
            set { base[key] = value; }
        }

        public new bool Remove(string key)
        {
            lock (this)
            {
                return base.Remove(key);
            }
        }
    }
}
