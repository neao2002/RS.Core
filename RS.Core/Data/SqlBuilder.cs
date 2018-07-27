using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Data
{
    public class SqlBuilder
    {
        StringBuilder _sb = new StringBuilder();
        public string ToSql()
        {
            return this._sb.ToString();
        }
        public SqlBuilder Append(object obj)
        {
            this._sb.Append(obj);
            return this;
        }
        public SqlBuilder Append(object obj1, object obj2)
        {
            return this.Append(obj1).Append(obj2);
        }
        public SqlBuilder Append(object obj1, object obj2, object obj3)
        {
            return this.Append(obj1).Append(obj2).Append(obj3);
        }
        public SqlBuilder Append(object obj1, object obj2, object obj3, object obj4)
        {
            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4);
        }
        public SqlBuilder Append(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4).Append(obj5);
        }
        public SqlBuilder Append(params object[] objs)
        {
            if (objs == null)
                return this;

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                this.Append(obj);
            }

            return this;
        }

        public SqlBuilder AppendLine()
        {
            _sb.AppendLine();
            return this;
        }

        public SqlBuilder AppendLine(string value)
        {
            _sb.AppendLine(value);
            return this;
        }

    }
}
