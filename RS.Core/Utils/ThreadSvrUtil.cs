using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RS
{
    /// <summary>
    /// 线程服务控制容器，本容器可以托管各类定时激活的活动
    /// </summary>
    public class ThreadSvrUtil
    {
        private bool IsSuspend = false;//线程是否已挂起

        private ThreadTimer threadTimer;
        private System.Threading.Thread threadSync;//进行同步的线程 

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Milliseconds"></param>
        /// <param name="method"></param>
        public ThreadSvrUtil(int Milliseconds, ThreadStart method)
        {
            threadTimer = new ThreadTimer(Milliseconds);
            threadTimer.Method = method;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="method"></param>
        public ThreadSvrUtil(TimeSpan ts, ThreadStart method)
        {
            threadTimer = new ThreadTimer(ts);
            threadTimer.Method = method;
        }

        /// <summary>
        /// 线程服务控制容器构造函数
        /// </summary>
        /// <param name="timer"></param>
        public ThreadSvrUtil(ThreadTimer timer)
        {
            threadTimer = timer;
            Runing = false;
        }

        #region 服务相关方法及属性

        public void ChangeTimeSpan(TimeSpan ts)
        {
            threadTimer.ChangeTimeSpan(ts);
        }

        private bool isDSRun = false;
        public void SetDSRun()
        {
            isDSRun = true;
        }
        /// <summary>
        /// 是否正在运行服务
        /// </summary>
        public bool Runing
        {
            get;
            set;
        }
        /// <summary>
        /// 开始启动服务
        /// </summary>
        public void Start()
        {
            if (ThreadStarting != null)
            {
                ThreadSvrUtilEventArgs e = new ThreadSvrUtilEventArgs();
                ThreadStarting(this, e);
                if (e.Cancel) return; //中止执行
            }

            Runing = true;
            if (threadSync != null && threadSync.ThreadState == ThreadState.Running)
                return;
            else if (threadSync != null && !(threadSync.ThreadState == ThreadState.Stopped || threadSync.ThreadState == ThreadState.Aborted || threadSync.ThreadState == ThreadState.AbortRequested))
            {
                try { threadSync.Abort(); }
                catch { }
            }            
            threadSync = new Thread(new ThreadStart(ThreadMethod));
            IsSuspend = false;
            Runing = true;
            threadSync.Start();//开始进行线程服务
            if (this.ThreadStarted != null) this.ThreadStarted(this, new EventArgs());
        }
        /// <summary>
        /// 停止运行服务
        /// </summary>
        public void Stop()
        {
            if (ThreadStoping != null)
            {
                ThreadSvrUtilEventArgs e = new ThreadSvrUtilEventArgs();
                ThreadStoping(this, e);
                if (e.Cancel) return; //中止执行
            }
            IsSuspend = false;
            Runing = false;

            if (threadSync == null || threadSync.ThreadState == ThreadState.Aborted || threadSync.ThreadState == ThreadState.Stopped || threadSync.ThreadState == ThreadState.AbortRequested)
                return;
            //中止当前线程
            try {
                threadSync.Join(1500);
                threadSync.Abort(); 
            }
            catch { }
            threadSync = null;

            if (this.ThreadStoped != null) this.ThreadStoped(this, new EventArgs());
        }

        /// <summary>
        /// 线程执行的方法
        /// </summary>
        protected void ThreadMethod()
        {
            if (!isDSRun) threadTimer.BeginRun();
            while (Runing)
            {
                if (IsSuspend) //已挂起，则不执行
                    Thread.Sleep(30);
                else
                    threadTimer.TriggerMethod();
            }
        }
        #endregion

        /// <summary>
        /// 挂起
        /// </summary>
        public void Suspend()
        {
            IsSuspend = Runing && true;
        }
        /// <summary>
        /// 继续
        /// </summary>
        public void Resume()
        {
            IsSuspend = false;
        }

        #region 快速应用本线程服务控制容器相关静态方法
        /// <summary>
        /// 创建服务控制容器:根据指定定时配置及线程方法
        /// </summary>
        /// <param name="appconfigExp">定时配置键值</param>
        /// <param name="method">线程服务要执行的具体代码委托</param>
        public static ThreadSvrUtil CreateSvrUtilByExp(string appconfigExp, ThreadStart method)
        {
            ThreadTimer t = ThreadTimer.CreateThreadTimerByExp(appconfigExp);
            t.Method = method;
            return new ThreadSvrUtil(t);
        }
        /// <summary>
        /// 创建服务控制容器:根据指定定时配置及线程方法
        /// </summary>
        /// <param name="ts">定时间隔</param>
        /// <param name="method">线程服务要执行的具体代码委托</param>
        /// <returns></returns>
        public static ThreadSvrUtil CreateSvrUtil(TimeSpan ts, ThreadStart method)
        {
            ThreadTimer t = new ThreadTimer(ts);
            t.Method = method;
            return new ThreadSvrUtil(t);
        }
        /// <summary>
        /// 创建服务控制容器:根据指定毫秒及线程方法
        /// </summary>
        /// <param name="Milliseconds"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ThreadSvrUtil CreateSvrUtil(int Milliseconds, ThreadStart method)
        {
            ThreadTimer t = new ThreadTimer(Milliseconds);
            t.Method = method;
            return new ThreadSvrUtil(t);
        }
        #endregion
        /// <summary>
        /// 服务启动前激活的事件
        /// </summary>
        public event ThreadStartingEventHandler ThreadStarting;
        /// <summary>
        /// 服务停止前激活的事件
        /// </summary>
        public event ThreadStopingEventHandler ThreadStoping;

        /// <summary>
        /// 服务启动后激活的事件
        /// </summary>
        public event ThreadStartedEventHandler ThreadStarted;
        /// <summary>
        /// 服务停止后激活的事件
        /// </summary>
        public event ThreadStopedEventHandler ThreadStoped;

    }
    /// <summary>
    /// 线程启动前事件委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ThreadStartingEventHandler(object sender,ThreadSvrUtilEventArgs e);
    
    /// <summary>
    /// 线程停止前事件委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ThreadStopingEventHandler(object sender, ThreadSvrUtilEventArgs e);

    /// <summary>
    /// 线程已启动时事件委托定义
    /// </summary>
    public delegate void ThreadStartedEventHandler(object sender,EventArgs e);
    /// <summary>
    /// 线程已停止时事件委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ThreadStopedEventHandler(object sender,EventArgs e);

    /// <summary>
    /// 线程服务事件参数对象
    /// </summary>
    public class ThreadSvrUtilEventArgs : EventArgs
    {
        public ThreadSvrUtilEventArgs()
        {
            Cancel = false;
        }
        /// <summary>
        /// 是否要中止执行
        /// </summary>
        public bool Cancel { get; set; }
    }
}
