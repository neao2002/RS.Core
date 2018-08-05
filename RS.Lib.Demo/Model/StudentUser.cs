using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Lib.Demo
{
    public class StudentUser:AppUserBase<IStudentUserProvider>, IAppUser
    {
        /// <summary>
        /// 学生对象记录ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        public string StudentNo { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 学生身份证号
        /// </summary>
        public string IDCardNo { get; set; }
        /// <summary>
        /// 所生所在班级
        /// </summary>
        public string ClassName { get; set; }

        public bool IsLogined()
        {
            return Id != Guid.Empty;
        }

    }
}
