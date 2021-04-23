using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Vit.Core.Module.Log;

namespace Vit.AspNetCore.BackgroundTask
{



    //     //(x.1)Startup.cs 启用定时任务
    //     public void ConfigureServices(IServiceCollection services)
    //     {   
    //         //启用定时任务
    //         services.UseBackgroundTask();
    //     }
    //      

    //     //(x.2)appsettings.json  配置定时任务        
    //     "AspNetCore": {
    //      "BackgroundTask": [
    //        {
    //          /* 任务名称 */
    //          "name": "消息提醒",
    //          /* 首次任务开始时间。如"13:00:00"代表下午1点才会执行第一次任务 */
    //          "startTime": "00:00:00",
    //          /* 执行周期，默认1天执行一次（"24:00:00"）。如"01:00:00"代表一个小时执行一次 */
    //          "period": "00:00:05",
    //          /* 任务的类名，如:"Main.BackgroundTask.MessageSender" */
    //          "className": "Main.BackgroundTask.MessageSender",
    //          /* 任务的函数名，如:"SendMessage"。函数参数（若存在）会通过依赖注入的方式构建 */
    //          "methodName": "SendMessage"
    //        }
    //      ]



    /// <summary>
    /// 
    /// <para> //(x.1)Startup.cs 启用定时任务                                                                       </para>
    /// <para> public void ConfigureServices(IServiceCollection services)                                           </para>
    /// <para> {                                                                                                    </para>
    /// <para>     //启用定时任务                                                                                   </para>
    /// <para>     services.UseBackgroundTask();                                                                    </para>
    /// <para> }                                                                                                    </para>
    /// <para>                                                                                                      </para>
    /// <para>                                                                                                      </para>
    /// <para> //(x.2)appsettings.json  配置定时任务                                                                </para>
    /// <para> "AspNetCore": {                                                                                      </para>
    /// <para>  "BackgroundTask": [                                                                                 </para>
    /// <para>    {                                                                                                 </para>
    /// <para>      /* 任务名称 */                                                                                  </para>
    /// <para>      "name": "消息提醒",                                                                             </para>
    /// <para>      /* 首次任务开始时间。如"13:00:00"代表下午1点才会执行第一次任务 */                               </para>
    /// <para>      "startTime": "00:00:00",                                                                        </para>
    /// <para>      /* 执行周期，默认1天执行一次（"24:00:00"）。如"01:00:00"代表一个小时执行一次 */                 </para>
    /// <para>      "period": "00:00:05",                                                                           </para>
    /// <para>      /* 任务的类名，如:"Main.BackgroundTask.MessageSender" */                                        </para>
    /// <para>      "className": "Main.BackgroundTask.MessageSender",                                               </para>
    /// <para>      /* 任务的函数名，如:"SendMessage"。函数参数（若存在）会通过依赖注入的方式构建 */                </para>
    /// <para>      "methodName": "SendMessage"                                                                     </para>
    /// <para>    }                                                                                                 </para>
    /// <para>  ]                                                                                                   </para>
    /// 
    /// </summary>
    public class BackgroundTaskService : Microsoft.Extensions.Hosting.BackgroundService
    {
        // https://www.cnblogs.com/inttochar/p/11511508.html
        // https://www.it610.com/article/1282377157410045952.htm



        IServiceProvider serviceProvider;
        public BackgroundTaskService(IServiceProvider serviceProvider) 
        {
            this.serviceProvider = serviceProvider;
        }

        //服务一启动就会执行此方法
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Console.WriteLine("Execute执行！");
            if (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run((Action)StartAllTimedTask);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                //Console.WriteLine("已注销停止服务！"); 

                taskList.ForEach(taskItem =>
                {
                    try
                    {
                        taskItem.Stop();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }              
                });

            });
        }



        List<TaskItem>taskList;

        private void StartAllTimedTask()
        {
            taskList = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<TaskItem[]>("AspNetCore.BackgroundTask")?
                .ToList();


            taskList.ForEach(taskItem =>
            {
                taskItem.serviceProvider = serviceProvider;

                taskItem.Start();
            });
        }
    }
}
