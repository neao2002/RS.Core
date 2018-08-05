using RS.Lib.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Web.MvcDemo
{
     public class StudentUserProvider : AppUserProviderBase<StudentUser,IStudentUserProvider>,IStudentUserProvider
    {
        public StudentUserProvider() : base(1,"Student")
        {
        }
        public override IAppSessionOptions GetOptions()
        {
            IAppSessionOptions options = base.GetOptions();
            options.IsSqlSaveCookie = true;
            return options;
        }
        public override string GetUserID(StudentUser user)
        {
            return user.Id.ToString();
        }

        public override string GetUserName(StudentUser user)
        {
            return user.Name;
        }

        public override string GetUserNo(StudentUser user)
        {
            return user.StudentNo;
        }

        public override bool IsBidden(StudentUser user)
        {
            return false;
        }

        public override bool IsGlobalUser(StudentUser user)
        {
            return false;
        }

        public override bool IsLogined(StudentUser user)
        {
            if (user == null) return false;

            return user.Id != Guid.Empty;
        }
    }
}
