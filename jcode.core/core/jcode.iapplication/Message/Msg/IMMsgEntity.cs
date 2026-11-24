using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：即时通讯消息内容
    /// </summary>
    [MyTable("lr_im_msg")]
    public class IMMsgEntity
    {
        #region 实体成员
        /// <summary>
        /// 消息主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_MsgId { get; set; }
        /// <summary>
        /// 发送者ID
        /// </summary>
        public string F_SendUserId { get; set; }
        /// <summary>
        /// 接收者ID
        /// </summary>
        public string F_RecvUserId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string F_Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 是否是系统消息
        /// </summary>
        public int? F_IsSystem { get; set; }
        /// <summary>
        /// 消息是否已读 1 是 0 不是；系统消息起作用
        /// </summary>
        public int? F_IsRead { get; set; }
        /// <summary>
        /// 是否删除1 是 0 不是
        /// </summary>

        public int? F_DeleteMark { get; set; }

        /// <summary>
        /// 消息内容id
        /// </summary>
        public string F_ContentId { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
