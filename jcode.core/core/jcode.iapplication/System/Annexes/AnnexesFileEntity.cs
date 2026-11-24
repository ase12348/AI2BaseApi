using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：附件管理
    /// </summary>
    [MyTable("lr_base_annexesfile")]
    public class AnnexesFileEntity
    {
        #region 实体成员
        /// <summary>
        /// 文件主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 附件夹主键（暂存夹为 "{主键}-draft"，列宽必须容纳后缀）
        /// </summary>
        public string F_FolderId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string F_FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string F_FilePath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string F_FileSize { get; set; }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string F_FileExtensions { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string F_FileType { get; set; }
        /// <summary>
        /// 下载次数
        /// </summary>
        public int? F_DownloadCount { get; set; }
        
        /// <summary>
        /// 数据库保存类型 Local,Db,AliYunOss,TencentOss,HuaWeiOss,MinIoOss
        /// </summary>
        public string F_SaveType{ get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUserName { get; set; }

        /// <summary>
        /// 关联数据库
        /// </summary>
        public string F_DbCode { get; set; }
        /// <summary>
        /// 关联数据库表名
        /// </summary>
        public string F_TableName { get; set; }
        /// <summary>
        /// 关联数据库字段名
        /// </summary>
        public string F_TableField { get; set; }
        
        /// <summary>
        /// 文件密级
        /// </summary>
        public int?F_SecretLevel { get; set; }
        
        /// <summary>
        /// 文件是否加密
        /// </summary>
        public int?F_IsEncrypt { get; set; }

        #endregion
        
        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
