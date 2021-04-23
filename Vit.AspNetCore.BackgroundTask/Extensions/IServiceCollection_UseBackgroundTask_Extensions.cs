﻿
using Microsoft.Extensions.DependencyInjection;

namespace Vit.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static partial class IServiceCollection_UseBackgroundTask_Extensions
    {


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
        /// <para>      "period": "00:00:05",                                                                            </para>
        /// <para>      /* 任务的类名，如:"Main.BackgroundTask.MessageSender" */                                        </para>
        /// <para>      "className": "Main.BackgroundTask.MessageSender",                                               </para>
        /// <para>      /* 任务的函数名，如:"SendMessage"。函数参数（若存在）会通过依赖注入的方式构建 */                </para>
        /// <para>      "methodName": "SendMessage"                                                                     </para>
        /// <para>    }                                                                                                 </para>
        /// <para>  ]                                                                                                   </para>
        /// 
        /// </summary>
        public static IServiceCollection UseBackgroundTask(this IServiceCollection services)
        {
            services.AddHostedService<Vit.AspNetCore.BackgroundTask.BackgroundTaskService>();
            return services;
        }

    }
}
