using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((opt, services) =>
    {
        services.AddQuartz();
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    })
    .Build();

var schedulerFactory = builder
    .Services
    .GetRequiredService<ISchedulerFactory>();

var scheduler = await schedulerFactory.GetScheduler();

var job = JobBuilder.Create<HelloJob>()
    .WithIdentity("job1", "group1")
    .Build();

var trigger = TriggerBuilder.Create()
    .WithIdentity("myTrigger", "group1")
    .StartNow()
    .Build();

await scheduler.ScheduleJob(job, trigger);

await builder.RunAsync();

internal class HelloJob : IJob
{
    private static int count = 0;
    
    public async Task Execute(IJobExecutionContext context)
    {
        HelloJob.count += 1;
        
        Console.WriteLine($"Hello World! ({HelloJob.count}x)");
        
        var oldTrigger = context.Trigger;
        var newTrigger = TriggerBuilder.Create()
            .ForJob(context.JobDetail)
            .WithIdentity(
                $"{oldTrigger.Key.Name}-retry", 
                oldTrigger.Key.Group)
            .StartAt(DateTime.UtcNow.AddSeconds(10))
            .Build();

        await context.Scheduler.ScheduleJob(newTrigger);
    }
}