using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Core
{
    public interface IResponse
    {
        void AddHeader(string key, string value);
        void AppendHeader(string key, string value);

        void Clear();


    }
}
