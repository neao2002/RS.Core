using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace RS.Core.Utils
{
    public static class PathUtils
    {
        #region 上传目录创建相关
        /// <summary>
        /// 检查是否为虚拟目录
        /// </summary>
        /// <param name="saveVPath"></param>
        /// <returns></returns>
        private static string GetMapPath(string saveVPath, string UploadRootPath, string SiteUrl)
        {
            string path = saveVPath.Trim().Replace("/", @"\");
            while (path.StartsWith(@"\") && path.Length > 1)
            {
                path = path.Substring(1);
            }
            if (path == @"\") path = "";
            //以上代码是把path中前面为"/"的字符去除
            if (UploadRootPath.IsWhiteSpace())
                UploadRootPath = SiteUrl;

            string m_UploadPath = "";
            try
            {
                string SitePath = Path.Combine(SiteUrl, path);//获取指定保存目录在站点下物理路径  当前站点跟目录
                string FactPath = Path.Combine(UploadRootPath, path);

                if (!SitePath.EndsWith(@"\")) SitePath += @"\";
                if (!FactPath.EndsWith(@"\")) FactPath += @"\";
                if (SitePath.ToLower() == FactPath.ToLower()) //当前目录是在站点目录下
                    m_UploadPath = FactPath;
                else //则有两种情况，一种是设定的这个，本身就为虚拟目录，另外一种就是系统设置在站点根目录之外的某个目录下
                {
                    string WebPath = Path.Combine(SiteUrl, path);
                    if (!WebPath.EndsWith(@"\")) WebPath += @"\";
                    if (SitePath.ToLower() != WebPath.ToLower()) //为虚拟目录
                        m_UploadPath = SitePath;
                    else //为站点下一个目录，则按系统初始路径保存，
                        m_UploadPath = FactPath;
                }
            }
            catch
            {
                //有异常，则保存在初始设置路径下
                m_UploadPath = Path.Combine(UploadRootPath, path);
            }
            return m_UploadPath;
        }
        //上传路径和目录 相关方法 
        /// <summary>
        /// 创建并获取上传要保存的路径（含物理路径和虚拟路径）
        /// </summary>
        /// <param name="SaveDirName">保存上传文件的主目录名，如为空，则默认为UploadFile</param>
        /// <param name="UploadRootPath">要保存上传文件所在根路径，如为空，则为当前应用物理路径</param>
        /// <param name="WebVirthPath">上传后访问的虚拟路径，如为空，则为相对路径</param>
        /// <param name="SiteUrl">如果是Web项目，则为该web项目的根路径</param>
        /// <returns></returns>
        public static UploadPathInfo CreateUploadPath(string MainDirName, UploadPathType PathType, string UploadRootPath, string WebVirthPath)
        {
            //TODO:物理路径
            string SiteUrl = "";
            
            //HttpContext page = HttpContext.Current;

            //if (page != null)
            //{
            //    try
            //    {
            //        SiteUrl = page.Server.MapPath("~/");
            //    }
            //    catch { }
            //}



            MainDirName = RemovePathString(MainDirName);

            if (MainDirName.IsWhiteSpace())
                MainDirName = "Upload";

            string path="";
            if (PathType == UploadPathType.DaylyChildDir)
                path = string.Format(@"{0}\{1:yyyyMMdd}", MainDirName, DateTime.Now);
            else if (PathType == UploadPathType.MonthlyChildDir)
                path = string.Format(@"{0}\{1:yyyyMM}", MainDirName, DateTime.Now);
            else
                path = MainDirName;


            System.IO.Directory.GetCurrentDirectory();
            

            string m_RelatPath, m_UploadPath;//物理路径和虚拟路径

            m_RelatPath = path;
            m_UploadPath = GetMapPath(path, UploadRootPath, SiteUrl);//上传文件保存目录

            //移除m_RelatPath前面的"/"字符
            m_RelatPath=m_RelatPath.Replace(@"\","/");

            while (m_RelatPath.StartsWith(@"/") && m_RelatPath.Length > 1)
            {
                m_RelatPath = m_RelatPath.Substring(1);
            }
            if (m_RelatPath == @"/") m_RelatPath = "";



            string wvp = WebVirthPath;
            if (!string.IsNullOrWhiteSpace(wvp))
            {
                if (!wvp.EndsWith("/")) wvp += "/";
                m_RelatPath = string.Format("{0}{1}", wvp, m_RelatPath);
            }

            if (!Directory.Exists(m_UploadPath)) //如果目录不存在，则建该目录
            {
                Directory.CreateDirectory(m_UploadPath);
            }

            return new UploadPathInfo()
            {
                PhysicalPath = m_UploadPath,
                VirthPath = m_RelatPath
            };
        }

        private static string RemovePathString(string DirName)
        {
            string path = DirName.Trim().Replace("/", @"\");
            //移除前缀\
            while (path.StartsWith(@"\") && path.Length > 1)
            {
                path = path.Substring(1);
            }
            //移除后缀\
            while (path.EndsWith(@"\") && path.Length > 1)
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        #endregion

        /// <summary>
        /// 创建文件名(不带扩展名(
        /// </summary>
        /// <returns></returns>
        public static string CreateUpFileNameNoExp(string SavePath, string OriginalFileName, UploadFielNameType FnType)
        {
            if (FnType == UploadFielNameType.RandomName)
            {
                Random rd = new Random(Guid.NewGuid().GetHashCode());
                int f = rd.Next(295, 999999);
                DateTime d = DateTime.Now;
                return string.Format("{0:yyyyMMddHHmmss}{1}{1}", d, d.Millisecond.ToString().PadLeft(3, '0'), f.ToString().PadLeft(6, '0'));
            }
            else if (FnType == UploadFielNameType.GuidName)
                return Guid.NewGuid().ToString();
            else
            {
                return OriginalFileName;
            }
        }

        public static string CreateUpFileNameNoExp(string OriginalFileName, UploadFielNameType FnType)
        {
            return CreateUpFileNameNoExp("", OriginalFileName, FnType);
        }
    }

    public enum UploadFielNameType
    {
        /// <summary>
        /// 生成随机命名
        /// </summary>
        RandomName=0,
        /// <summary>
        /// 按Guid进行重命名
        /// </summary>
        GuidName=1,
        /// <summary>
        /// 保持原有命名，为防重复，在后面增加相关标识
        /// </summary>
        OriginalName=2
    }
    public enum UploadPathType
    {
        /// <summary>
        /// 按日作为子目录
        /// </summary>
        DaylyChildDir,
        /// <summary>
        /// 按月作为子目录
        /// </summary>
        MonthlyChildDir,
        /// <summary>
        /// 以当前目录存放
        /// </summary>
        NoChildDir
    }

    public struct UploadPathInfo
    {
        /// <summary>
        /// 物理保存路径（完整路径)
        /// </summary>
        public string PhysicalPath { get; set; }
        /// <summary>
        /// 虚拟访问路径（完整路径）
        /// </summary>
        public string VirthPath { get; set; }
    }
}
