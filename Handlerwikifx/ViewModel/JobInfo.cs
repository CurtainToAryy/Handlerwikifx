using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlerwikifx.ViewModel
{
    /// <summary>
    /// Job信息
    /// </summary>
    public class JobInfo
    {
        /// <summary>
        /// 计划名称
        /// </summary>
        public string ScheDname { get; set; }
        /// <summary>
        /// Job名
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// Job组
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// Crons表达式
        /// </summary>
        public string Crons { get; set; }

    }
}
