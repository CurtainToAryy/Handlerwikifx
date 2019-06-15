using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FXEye.PhotoManage.Dao;
using FXEye.PhotoManage.Interface;
using FXEye.PhotoManage.Quartz;
using FXEye.PhotoManage.ViewModel;
using Quartz.Impl;
using System.Configuration;
using FXEye.PhotoManage.Job;
using System.Diagnostics;

namespace FXEye.PhotoManage.Service
{
    /// <summary>
    /// 处理服务
    /// </summary>
    public class AttanchMentService : ITaskService
    {

        private static SchedulerManaager Manager = null;
        private static JobInfo jobInfo = null;
        private static AttanchMentService instance =null;

        // 从配置文件读取
        private static string JobGroup = ConfigurationManager.AppSettings["JobGroup"] ?? "";
        private static string JobName = ConfigurationManager.AppSettings["JobName"] ?? "";
        private static string Crons = ConfigurationManager.AppSettings["Crons"] ?? "";
        private static string ScheDname = ConfigurationManager.AppSettings["ScheDname"] ?? "";

        /// <summary>
        /// 静态构造函数实现单例和初始化
        /// </summary>
        static AttanchMentService()
        {

            instance = new AttanchMentService();

            if (jobInfo == null)
            {

                jobInfo = new JobInfo()
                {
                    JobGroup = JobGroup,
                    JobName = JobName,
                    Crons = Crons,
                    ScheDname = ScheDname
                };
            }

            Manager = SchedulerManaager.Singleton;
            Manager.Scheduler = new StdSchedulerFactory().GetScheduler().Result;
            if (!Manager.Scheduler.IsStarted)
            {
                Manager.Scheduler.Start().Wait();
            }


          
        }

        public static AttanchMentService GetAttanchment() {

            return instance;
        }

        public void Delete()
        {
            if (jobInfo != null)
            {
                Manager.Remove(jobInfo);
            }
        }

        public void Pause()
        {
            Manager.Pause(jobInfo);
        }

        public void Resume()
        {

        }

        public void Run()
        {
            if (jobInfo != null)
            {
                Manager.CreateJob<AttanchmentJob>(jobInfo);
            }
        }
    }
}
