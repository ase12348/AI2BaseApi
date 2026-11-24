using System;
using System.Collections.Generic;
using System.Text;

namespace jcode.util.NLog
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2023.03.31
    /// 描 述：审计日志配置详情实体类
    /// </summary>
    public class NLogModel
    {
        /// <summary>
        /// 请求唯一id
        /// </summary>
        public string OnlyId { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public string RequestMethod { get; set; }
        /// <summary>
        /// 请求ip
        /// </summary>
        public string MachineIp { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 操作前记录
        /// </summary>
        public string BeforeData { get; set; }
        /// <summary>
        /// 操作后记录
        /// </summary>
        public string AfterData { get; set; }
        /// <summary>
        /// 直接保存日志记录
        /// </summary>
        public string OtherData { get; set; }
    }
}
