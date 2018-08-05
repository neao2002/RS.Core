using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Lib.Demo
{
    public class TeacherUser: AppUserBase<ITeacherUserProvider>, IAppUser
    {
        /// <summary>
        /// 老师对象记录ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 老师编号
        /// </summary>
        public string TeacherNo { get; set; }
        /// <summary>
        /// 老师姓名
        /// </summary>
        public string Name { get; set; }

        public bool IsLogined()
        {
            return Id != Guid.Empty;
        }
    }
}
