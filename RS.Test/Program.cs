using System;
using System.Collections.Generic;
using RS.Core;
using RS.Core.Data;

namespace RS.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            LibHelper.InitDbUtils("Data Source=172.17.4.45,3433;Initial Catalog=Apollo;Integrated Security=false;User ID=dev-sa;Password=WhU6htL8;Min Pool Size=0;Max Pool Size=100;", DbDriverType.SQLServer);
            DbUtils db = DbUtils.NewDB();

            List<DynamicObj> obj = db.GetDynamicObjs($"select * from tb_product with(nolock)");

            obj.ForEach(p =>
            {
                Console.WriteLine($"产品编号:{p.Get<string>("ProductCode")},产品名称：{p.Get<string>("ProductChineseName")}");
            });
            string strsql = LibHelper.CreateIFilter(db)
                .AppendSqlFilter("ProductCode='FX012'")
                .AppendDefineItems("F1", Core.Filter.FilterDataType.String, Core.Filter.CompareType.Equal, new string[] { })
                .AppendDefineItems("F2", Core.Filter.FilterDataType.DateTime, Core.Filter.CompareType.Equal, new string[] { })
                .AppendDefineItems("A", Core.Filter.FilterDataType.String, Core.Filter.CompareType.Equal, null)
                .AppendDefineItems("B",Core.Filter.FilterDataType.String,Core.Filter.CompareType.NoEqual,new int[]{ 0,1,2})
                .AppendSqlFilter("RealName='李建平'")
                .GetFilter("where");

            


            Console.WriteLine(strsql);


            Console.Read();
        }
    }
}
