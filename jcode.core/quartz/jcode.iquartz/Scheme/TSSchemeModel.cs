using System;
using System.Collections.Generic;

namespace jcode.iquartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务计划模板信息
    /// </summary>
    public class TSSchemeModel
    {
        /// <summary>
        /// 开始方式1配置完立即执行2根据设置的开始时间
        /// </summary>
        public int StartType { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束方法 1无限期2有结束时间
        /// </summary>
        public int EndType { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 执行频率类别 
        /// 1：只执行一次
        /// 2：简单重复执行 涉及分钟，小时，天，周
        /// 3：明细频率设置 
        /// 4：表达式设置 corn表达式
        /// </summary>
        public int ExecuteType { get; set; }


        /// <summary>
        /// 间隔时间值 对应2
        /// </summary>
        public int SimpleValue { get; set; }
        /// <summary>
        /// 间隔类型 对应2 minute分hours小时day天week周
        /// </summary>
        public string SimpleType { get; set; }

        /// <summary>
        /// 间隔类型 对应3 频率明显
        /// </summary>
        public List<DetailFrequencyModel> FrequencyList { get; set; }

        /// <summary>
        /// cron表达式 对应4
        /// </summary>
        public string CornValue { get; set; }


        /// <summary>
        /// 是否重启1是0不是
        /// </summary>
        public int IsRestart { get; set; }
        /// <summary>
        /// 间隔重启时间（分钟）
        /// </summary>
        public int RestartMinute { get; set; }
        /// <summary>
        /// 重启次数
        /// </summary>
        public int RestartNum { get; set; }
        /// <summary>
        /// 方法类型1sql 2存储过程 3接口 4ioc依赖注入
        /// </summary>
        public int MethodType { get; set; }
        /// <summary>
        /// 数据ID
        /// </summary>
        public string DbId { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string StrSql { get; set; }
        /// <summary>
        /// 存储过程
        /// </summary>
        public string ProcName { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 接口请求方式 1get 2post
        /// </summary>
        public string UrlType { get; set; }
        /// <summary>
        /// 依赖注入方法名
        /// </summary>
        public string IocName { get; set; }
    }


    /// <summary>
    /// 明细频率类
    /// </summary>
    public class DetailFrequencyModel
    {
        /// <summary>
        /// 小时
        /// </summary>
        public string Hour { get; set; }
        /// <summary>
        /// 分钟
        /// </summary>
        public string Minute { get; set; }
        /// <summary>
        /// 间隔类型 每天day，每周week，每月month
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        ///间隔执行值
        /// </summary>
        public string Week { get; set; }
        /// <summary>
        ///间隔执行值
        /// </summary>
        public string Day { get; set; }
        /// <summary>
        /// 执行月
        /// </summary>
        public string Mounth { get; set; }
    }
}
