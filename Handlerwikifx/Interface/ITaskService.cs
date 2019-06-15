using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Handlerwikifx.Interface
{
    /// <summary>
    /// 任务统一接口
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// 开始运行
        /// </summary>
        void Run();
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        /// <summary>
        /// 恢复
        /// </summary>
        void Resume();
        /// <summary>
        /// 删除
        /// </summary>
        void Delete();
    }
}
