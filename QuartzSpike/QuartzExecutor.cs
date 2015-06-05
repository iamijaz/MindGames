using System;
using System.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace QuartzSpike
{
    public class QuartzExecutor
    {
        public static void Execute()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();


            var job = FindOrCreateItem(FindJob, CreateJob, scheduler, "J1", "G1");
            var trigger = FindOrCreateItem(FindTrigger, CreateTrigger, scheduler, "T1", "G1");

            if (!scheduler.CheckExists(job.Key) || !scheduler.CheckExists(trigger.Key))
            {
                scheduler.ScheduleJob(job, trigger);
            }
        }

        private static IJobDetail CreateJob(string name, string group)
        {
            return JobBuilder.Create<HelloJob>().WithIdentity(name, group).Build();
        }

        private static ITrigger CreateTrigger(string name, string group)
        {
            return TriggerBuilder.Create().WithIdentity(name, group)
                .StartNow().WithSimpleSchedule(x =>
                    x.WithIntervalInSeconds(10)
                        .RepeatForever())
                .Build();
        }

        private static T FindOrCreateItem<T>(Func<IScheduler, string, T> find, Func<string, string, T> create, IScheduler scheduler, string name, string group)
        {
            var item = find(scheduler, name);
            if (item == null)
            {
                item = create(name, group);
            }
            return item;
        }

        private static ITrigger FindTrigger(IScheduler scheduler, string name)
        {
            var jobGroups = scheduler.GetJobGroupNames();

            return
                jobGroups.Select(GroupMatcher<JobKey>.GroupContains)
                    .Select(scheduler.GetJobKeys)
                    .SelectMany(jobKeys => jobKeys, (jobKeys, jobKey) => new {jobKeys, jobKey})
                    .Select(@t => new {@t, detail = scheduler.GetJobDetail(@t.jobKey)})
                    .Select(@t => scheduler.GetTriggersOfJob(@t.@t.jobKey))
                    .SelectMany(triggers => triggers, (triggers, trigger) => new {triggers, trigger})
                    .Where(@t => @t.trigger.Key.Name == name)
                    .Select(@t => @t.trigger)
                    .FirstOrDefault();
        }

        private static IJobDetail FindJob(IScheduler scheduler, string name)
        {
            var jobGroups = scheduler.GetJobGroupNames();

            return
                jobGroups.Select(GroupMatcher<JobKey>.GroupContains)
                    .Select(scheduler.GetJobKeys)
                    .SelectMany(jobKeys => jobKeys, (jobKeys, jobKey) => scheduler.GetJobDetail(jobKey))
                    .FirstOrDefault(detail => detail.Key.Name == name);
        }
    }
}