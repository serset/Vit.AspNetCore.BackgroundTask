using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Vit.Core.Module.Log;
using System.Linq;
using Vit.Core.Util.Reflection;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Vit.AspNetCore.BackgroundTask
{
    internal class TaskItem
    {

        /// <summary>
        /// 任务名称
        /// </summary>
        public string name;

        /// <summary>
        /// 首次任务开始时间。如"13:00:00"代表下午1点才会执行第一次任务
        /// </summary>
        public string startTime = "00:00:00";


        /// <summary>
        /// 执行周期，默认1天执行一次（"24:00:00"）。如"01:00:00"代表一个小时执行一次
        /// </summary>
        public string period = "24:00:00";

        /// <summary>
        /// 任务的类名，如:"Ast.Module.Ast.BackgroundTask.SendAstExpireMessage"
        /// </summary>
        public string className;

        /// <summary>
        /// 任务的函数名，如:"SendMessage"。函数参数（若存在）会通过依赖注入的方式构建
        /// </summary>
        public string methodName;


        /// <summary>
        /// 是否在创建任务时立即执行一次。默认：false
        /// </summary>
        public bool? invokeWhenCreate;

        /// <summary>
        /// 在执行任务前是否打印日志。默认：false
        /// </summary>
        public bool? printLog;

        internal IServiceProvider serviceProvider;
        public void Start()
        {
            if (timer != null) return;

            if (printLog == true)
                Logger.Info("[TimeBackgroundService][" + name + "]init ");

            DateTime now = DateTime.Now;

            #region (x.1)type
            try
            {
                type = ObjectLoader.GetType(className);
                //type = Assembly.GetEntryAssembly().GetType(className);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            if (type == null)
            {
                Logger.Info("[TimeBackgroundService][" + name + "] 出错。 未找到类型“ " + className + "”。");
                return;
            }

            #endregion


            #region (x.2)获取任务周期
            TimeSpan peroid = TimeSpan.Zero;
            {
                var times = (period ?? "24:00:00").Split(':');
                if (times.Length >= 1)
                {
                    peroid = peroid.Add(TimeSpan.FromHours(Convert.ToDouble(times[0])));
                }
                if (times.Length >= 2)
                {
                    peroid = peroid.Add(TimeSpan.FromMinutes(Convert.ToDouble(times[1])));
                }
                if (times.Length >= 3)
                {
                    peroid = peroid.Add(TimeSpan.FromSeconds(Convert.ToDouble(times[2])));
                }
            }
            #endregion


            #region (x.3)获取起始时间点            
            DateTime startTime = DateTime.Today;
            {
                var times = (this.startTime ?? "00:00:00").Split(':');
                if (times.Length >= 1)
                {
                    startTime = startTime.AddHours(Convert.ToDouble(times[0]));
                }
                if (times.Length >= 2)
                {
                    startTime = startTime.AddMinutes(Convert.ToDouble(times[1]));
                }
                if (times.Length >= 3)
                {
                    startTime = startTime.AddSeconds(Convert.ToDouble(times[2]));
                }
            }

            //如果当前时间大于设置时间，后移到对应任务周期内
            while (startTime < now)
            {
                startTime += peroid;
            }
            var dueTime = startTime - now;

            #endregion


            #region (x.4)是否在创建任务时立即执行一次            
            if (invokeWhenCreate == true)
            {
                try
                {
                    Task.Run(() => DoWork(null));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            #endregion


            //(x.5)启动定时器
            timer = new System.Threading.Timer(DoWork);
            timer.Change(dueTime, peroid);
        }

        Timer timer = null;
        Type type = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DoWork(object state)
        {
            try
            {
                if (printLog == true)
                    Logger.Info("[TimeBackgroundService][" + name + "]run ");

                if (type == null) return;

                using (var scope = serviceProvider.CreateScope())
                {
                    object obj;
                    #region (x.1)创建实例对象     
                    {
                        var constructor = type.GetConstructors().Last();
                        var args = constructor.GetParameters()
                            .Select(param => scope.ServiceProvider.GetRequiredService(param.ParameterType))
                            .ToArray();
                        obj = System.Activator.CreateInstance(type, args);
                    }
                    #endregion

                    #region (x.2)调用函数
                    {
                        var method = type.GetMethod(methodName);

                        var args = method.GetParameters()
                      .Select(param => scope.ServiceProvider.GetRequiredService(param.ParameterType))
                      .ToArray();

                        method.Invoke(obj, args);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public void Stop()
        {
            if (timer == null) return;

            if (printLog == true)
                Logger.Info("[TimeBackgroundService][" + name + "]stop ");

            //停止
            timer.Change(-1, 0);
            timer = null;
        }

        ~TaskItem()
        {
            Stop();
        }
    }
}
