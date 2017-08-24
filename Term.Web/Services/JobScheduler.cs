using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace YstProject.Services
{
    public class JobScheduler
    {

        public static void Start()
        {
           IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
           scheduler.Start();

          IJobDetail jobDisks = JobBuilder.Create<UpdateDisksPicturesJob>().Build();

          IJobDetail jobTyres = JobBuilder.Create<UpdateTyresPicturesJob>().Build();

            IJobDetail jobOthers = JobBuilder.Create<UpdateOthersPicturesJob>().Build();


            ITrigger triggerDisks = TriggerBuilder.Create().StartNow().WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever()).Build();

            ITrigger triggerTyres = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(10, 0)).Build();

            ITrigger triggerOthers = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(16, 40)).Build();

            scheduler.ScheduleJob(jobDisks, triggerDisks);
            scheduler.ScheduleJob(jobTyres, triggerTyres);
            scheduler.ScheduleJob(jobOthers, triggerOthers);


        }

    }
}