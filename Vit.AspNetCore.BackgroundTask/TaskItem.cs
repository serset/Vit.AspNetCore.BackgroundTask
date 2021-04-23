using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using Vit.Core.Module.Log;
using System.Linq;

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
        /// 任务的函数名，如:"SendMessage"
        /// </summary>
        public string methodName;



        internal IServiceProvider serviceProvider;
        public void Start()
        {
            if (timer != null) return;

            Logger.Info("[TimeBackgroundService][" + name + "]init ");

            DateTime now = DateTime.Now;


            #region (x.1)获取起始时间点            
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


            //如果当前时间大于设置时间，后移到对应任务周期内
            while (startTime < now)
            {
                startTime += peroid;
            }
            var dueTime = startTime - now;

            timer = new System.Threading.Timer(DoWork);
            timer.Change(dueTime, peroid);
        }

        Timer timer = null;


        private void DoWork(object state)
        {
            try
            {
                Logger.Info("[TimeBackgroundService][" + name + "]run ");

                Type type = Assembly.GetEntryAssembly().GetType(className);

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
