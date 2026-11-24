using System;
using System.Collections.Generic;
using System.Text;
using jcode.office;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：附件管理输入参数
    /// </summary>
    public class AnnexesFileInputDto
    {
        /// <summary>
        /// 附件夹主键
        /// </summary>
        public string FolderId { get; set; }
        /// <summary>
        /// 文件主键
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件总分片数
        /// </summary>
        public int Chunks { get; set; }
        /// <summary>
        /// 是否公共
        /// </summary>
        public int IsPublic { get; set; }

        /// <summary>
        /// 关联数据库
        /// </summary>
        public string DbCode { get; set; }
        /// <summary>
        /// 关联数据库表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 关联数据库字段名
        /// </summary>
        public string TableField { get; set; }

        /// <summary>
        /// excel导入配置
        /// </summary>
        /// <value></value>
        public List<ExcelArea> ExcelAreas{ get; set; }
    }
}
