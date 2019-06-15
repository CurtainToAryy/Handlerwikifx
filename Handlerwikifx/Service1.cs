using Handlerwikifx.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Handlerwikifx.Common;

namespace Handlerwikifx
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            Debugger.Launch();
            try
            {
                WebHandlerService.GetAttanchment().Run();
                LogHelper.Info($" 服务 {nameof(Service1) } 正常启动了");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"出错了,错误信息为：{ex.Message}"); 
            }

           
        }

        protected override void OnStop()
        {
        
            LogHelper.Info($" 服务 {nameof(Service1) } 正常停止了");
            try
            {
                WebHandlerService.GetAttanchment().Delete();
                LogHelper.Info($" 服务 {nameof(Service1) } 正常停止了");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"出错了,错误信息为：{ex.Message}");
            }
        }
    }
}
