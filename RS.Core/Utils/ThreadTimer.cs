using System;
using System.Collections.Generic;
using System.Threading;

namespace RS
{
    /// <summary>
    /// 定时触发计时器,用于定时进行同步缓存或其它操作
    /// 一般为三种方式触发：
    /// 1：定时触发，即每隔时、分、秒便激发一次。
    /// 2：时点触发，即提定某个时间激发一次，一般精确到分。
    /// </summary>
    public sealed class ThreadTimer
    {
        /// <summary>
        /// 在时行初始化之前请先激活一次
        /// </summary>
        public ThreadTimer()
        {   
            TriggerType = TriggerType.TimeGap;
            //默认为10秒执行一次
            GapSpan = new TimeSpan(0, 0, 10);
            TriggerTimes = new List<DateTime>();
        }
        /// <summary>
        /// 构造函数:以定时方式激活,毫秒
        /// </summary>
        /// <param name="Milliseconds"></param>
        public ThreadTimer(int Milliseconds)
        {
            GapSpan = new TimeSpan(0, 0, 0, 0, Milliseconds);
            TriggerType = TriggerType.TimeGap;
            TriggerTimes = new List<DateTime>();
        }
        /// <summary>
        /// 构造函数:以定时方式激活
        /// </summary>
        /// <param name="SpanExp">触发器定时表达式</param>
        public ThreadTimer(TimeSpan timeSpan)
        {
            GapSpan = timeSpan;
            TriggerTimes = new List<DateTime>();
            TriggerType = TriggerType.TimeGap;
        }
        /// <summary>
        /// 构造函数：以指定时点激活,因为时点是精确到分种，故间隔执行时间为1分钟
        /// </summary>
        public ThreadTimer(List<DateTime> Times)
        {
            GapSpan = new TimeSpan(0, 1, 0);
            TriggerTimes = Times;
            TriggerType = TriggerType.FixedTime;
        }
        
        #region 对象属性    
        /// <summary>
        /// 计时激活的间隔
        /// </summary>
        public TimeSpan GapSpan { get; set; }

        
        /// <summary>
        /// 定时激发的时点
        /// </summary>
        public List<DateTime> TriggerTimes { get; set; }

        /// <summary>
        /// 线程激活类型：定时激活，时点激活
        /// </summary>
        private TriggerType TriggerType
        {
            get;
            set;
        }
        /// <summary>
        /// 定时激活的委托，该委托应为无参委托，主要用于线程服务中While内部
        /// </summary>
        public ThreadStart Method 
        {
            get;
            set;
        }
        #endregion

        #region 定时激活方法
        /// <summary>
        /// 第一次运行方法
        /// </summary>
        internal void BeginRun()
        {
            //如果是时点激活，则只有达到指定时点后才会执行
            if (TriggerType == TriggerType.FixedTime)
            {
                DateTime time = DateTime.Now;
                if (TriggerTimes.Exists(s => time.Hour == s.Hour && time.Minute == s.Minute))
                    RunMethod();
            }
            else
            {
                RunMethod();
            }
        }
        /// <summary>
        /// 运行方法
        /// </summary>
        private void RunMethod()
        {
            try
            {
                if (Method != null)
                    Method();
            }
            catch {}
        }
        /// <summary>
        /// 定时激活执行方法
        /// </summary>
        internal void TriggerMethod()
        {
            //先进行线程停顿，以便其它线程可使用CPU
            System.Threading.Thread.Sleep(GapSpan);

            //如果是时点激活，则只有达到指定时点后才会执行
            if (TriggerType == TriggerType.FixedTime)
            {
                DateTime time = DateTime.Now;
                if (TriggerTimes.Exists(s => time.Hour == s.Hour && time.Minute == s.Minute))
                    RunMethod();
            }
            else
            {
                RunMethod();
            }
        }
        #endregion

        #region 创建定时器静态方法        
        /// <summary>
        /// 通过定时表达式创建定时激活器
        /// 定时器设置格式如下：
        /// 1、定时触发：0|h|1,表示按每小时激活,0|m|1 表示按每分钟激活,0|s|1 表示按每秒钟激活,0|ms|1 表示按每毫秒激活
        /// 2、时点触发：1|17:30;17:12;02:36
        /// </summary>
        /// <param name="GapSpanExp"></param>
        /// <returns></returns>
        public static ThreadTimer CreateThreadTimerByExp(string GapSpanExp)
        {
            ThreadTimer t = new ThreadTimer();
            if (string.IsNullOrEmpty(GapSpanExp)) return t;
            string[] vs = GapSpanExp.Split('|');
            if (vs[0] == "0" && vs.Length == 3)//定时激活
            {
                if (vs[1] == "h")
                    t.GapSpan = new TimeSpan(vs[2].ToInt(), 0, 0);
                else if (vs[1] == "m")
                    t.GapSpan = new TimeSpan(0, vs[2].ToInt(), 0);
                else if (vs[1] == "ms")//毫秒
                    t.GapSpan = new TimeSpan(0, 0, 0, 0, vs[2].ToInt());
                else
                    t.GapSpan = new TimeSpan(0, 0, vs[2].ToInt());
            }
            else if (vs[0] == "1")
            {
                t.TriggerType = TriggerType.FixedTime;
                t.GapSpan = new TimeSpan(0, 1, 0);
                string[] ts = vs[1].Split(';');
                foreach (string item in ts)
                {
                    string[] time = item.Split(':');
                    if (time.Length >= 2)
                        t.TriggerTimes.Add(new DateTime(1900, 1, 1, time[0].ToInt(), time[1].ToInt(), 0));
                }
            }
            return t;
        }
        #endregion

        #region 更新频率(只对定时执行有效)
        public void ChangeTimeSpan(TimeSpan ts)
        {
            if (TriggerType != TriggerType.FixedTime) GapSpan = ts;
        }
        #endregion
    }

    /// <summary>
    /// 定时激活类型
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// 时间间隔激发，对于60秒间隔的采用这类
        /// </summary>
        TimeGap = 0,
        /// <summary>
        /// 定时激发,仅限于每一天的某个时点
        /// </summary>
        FixedTime = 1
    }
}