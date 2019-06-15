using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Handlerwikifx.ViewModel;
using Quartz;

namespace Handlerwikifx.Quartz
{
    /// <summary>
    /// 调度器
    /// </summary>
    public class SchedulerManaager : IDisposable
    {
        /// <summary>
        /// 调度器
        /// </summary>
        public IScheduler Scheduler;


        /// <summary>
        /// 单类
        /// </summary>
        public static SchedulerManaager Singleton { get; }
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object Locker = new object();

        public static bool _schedulerIsWorking = false;
        /// <summary>
        /// 私有化构造函数
        /// </summary>
        private SchedulerManaager() { }

        static SchedulerManaager()
        {
            Singleton = new SchedulerManaager();
        }

        internal bool Add<T>(JobInfo jobInfo) where T:IJob
        {


            JobKey jobKey = null;

            IJobDetail jobDetail = GetJobDetail(jobInfo, out jobKey);
            if (jobDetail != null)
            {
                Scheduler.DeleteJob(jobKey).Wait();
            }

            jobDetail = CreateJobDetail<T>(jobInfo);
            ITrigger trigger = CreateTrigger(jobInfo);
            Scheduler.ScheduleJob(jobDetail, trigger).Wait();

            return true;

        }

        public bool Pause(JobInfo jobInfo)
        {


            ITrigger trigger = GetTrigger(jobInfo, out TriggerKey triggerKey);
            if (trigger != null)
            {
                Scheduler.PauseTrigger(triggerKey).Wait();
            }
            return true;
        }



        public bool Resume(JobInfo jobInfo)
        {


            ITrigger trigger = GetTrigger(jobInfo, out TriggerKey triggerKey);
            if (triggerKey == null)
            {
                return false;
            }

            Scheduler.ResumeTrigger(triggerKey).Wait();
            return true;

        }


        /// <summary>
        /// 从job池中移除某个job,同时卸载该job所在的AppDomain
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <returns></returns>
        public bool Remove(JobInfo jobInfo)
        {

            ITrigger trigger = GetTrigger(jobInfo, out TriggerKey triKey);
            if (trigger == null)
            {
                return true;
            }
            Scheduler.PauseTrigger(triKey);
            Scheduler.UnscheduleJob(triKey);
            Scheduler.DeleteJob(GetJobKey(jobInfo));

            return true;
            //TODO:记录日志
        }


        public bool CreateJob<T>(JobInfo jobInfo) where T:IJob
        {

            if (jobInfo == null)
            {
                return false;
            }
            else
            {
                return Add<T>(jobInfo);
            }
        }





        private ITrigger CreateTrigger(JobInfo jobInfo)
        {
            TriggerBuilder triggerBuilder = TriggerBuilder.Create().WithIdentity(jobInfo.JobName, jobInfo.JobGroup);

            if (!string.IsNullOrEmpty(jobInfo.Crons))
            {
                //失火策略
                triggerBuilder.WithCronSchedule(jobInfo.Crons);
            }

            triggerBuilder.StartNow();

            ITrigger trigger = triggerBuilder.Build();
            return trigger;

        }

        private IJobDetail CreateJobDetail<T>(JobInfo jobInfo) where T:IJob
        {
            IJobDetail jobDetail = JobBuilder.Create<T>()
                .WithIdentity(jobInfo.JobName, jobInfo.JobGroup)
                .Build();
            return jobDetail;
        }

        private IJobDetail GetJobDetail(JobInfo jobInfo, out JobKey jobKey)
        {
            jobKey = GetJobKey(jobInfo);
            return Scheduler.GetJobDetail(jobKey).Result;
        }

        private JobKey GetJobKey(JobInfo jobInfo)
        {
            return new JobKey(jobInfo.JobName, jobInfo.JobGroup);
        }
        /// <summary>
        /// 获取 Trigger
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <param name="triKey"></param>
        /// <returns></returns>
        private ITrigger GetTrigger(JobInfo jobInfo, out TriggerKey triKey)
        {
            triKey = GetTriggerKey(jobInfo);
            return Scheduler.GetTrigger(triKey).Result;
        }

        public void Dispose()
        {

            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                Scheduler.Shutdown();
            }

        }

        /// <summary>
        /// 获取triggerKey
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <returns></returns>
        private TriggerKey GetTriggerKey(JobInfo jobInfo)
        {
            return new TriggerKey(jobInfo.JobName, jobInfo.JobGroup);
        }
    }
}

