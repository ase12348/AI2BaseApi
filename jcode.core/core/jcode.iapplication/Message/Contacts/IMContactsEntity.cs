using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：最近联系人列表
    /// </summary>
    [MyTable("lr_im_contacts")]
    public class IMContactsEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 发送者ID
        /// </summary>
        public string F_MyUserId { get; set; }
        /// <summary>
        /// 接收者ID
        /// </summary>
        public string F_OtherUserId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string F_Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_Time { get; set; }
        /// <summary>
        /// 未读记录数据
        /// </summary>
        public int? F_NoReadNum { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
