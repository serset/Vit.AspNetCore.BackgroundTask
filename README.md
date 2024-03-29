# Vit.AspNetCore.BackgroundTask
AspNetCore后台定时任务

# (x.1)安装nuget包
``` 
// *.csproj

<ItemGroup>	 
	  <PackageReference Include="Vit.AspNetCore.BackgroundTask" Version="2.2.3" />
</ItemGroup>
```

# (x.2)启用定时任务

``` c#
//Startup.cs 

public void ConfigureServices(IServiceCollection services)
{   
    //...
    
    //启用定时任务
    services.UseBackgroundTask();
}
```

# (x.3)配置定时任务
```
// appsettings.json

{
  "AspNetCore": {
    "BackgroundTask": [
      {
        /* 任务名称 */
        "name": "消息提醒",
        /* 是否在创建任务时立即执行一次。默认：false */
        "invokeWhenCreate": false,
        /* 首次任务开始时间。如"13:00:00"代表下午1点才会执行第一次任务 */
        "startTime": "00:00:00",
        /* 执行周期，默认1天执行一次（"24:00:00"）。如"01:00:00"代表一个小时执行一次 */
        "period": "00:00:05",
        /* 任务的类名，如:"Main.BackgroundTask.MessageSender" */
        "className": "Main.BackgroundTask.MessageSender",
        /* 任务的函数名，如:"SendMessage"。函数参数（若存在）会通过依赖注入的方式构建 */
        "methodName": "SendMessage"
      }
    ]
}
```
