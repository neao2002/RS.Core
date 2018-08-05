using RS.Lib.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Web.MvcDemo
{
    public class TeacherUserProvider : AppUserProviderBase<TeacherUser,ITeacherUserProvider>,ITeacherUserProvider
    {
        public TeacherUserProvider() : base(2,"Teacher")
        {
        }

        public override IAppSessionOptions GetOptions()
        {
            IAppSessionOptions options= base.GetOptions();
            options.IsSqlSaveCookie = true;
            return options;
        }
        public override string GetUserID(TeacherUser user)
        {
            return user.Id.ToString();
        }

        public override string GetUserName(TeacherUser user)
        {
            return user.Name;
        }

        public override string GetUserNo(TeacherUser user)
        {
            return user.TeacherNo;
        }

        public override bool IsBidden(TeacherUser user)
        {
            return false;
        }

        public override bool IsGlobalUser(TeacherUser user)
        {
            return false;
        }

        public override bool IsLogined(TeacherUser user)
        {
            return user.Id != Guid.Empty;

        }
    }
}
