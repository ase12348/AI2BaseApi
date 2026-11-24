using System.Collections.Generic;

namespace jcode.iapplication
{

    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：施赛一
    /// 日 期：2021-09-26 16:30:17
    /// 描 述：数据库中间表
    /// </summary>
    public class CodeTableColumnsDto
    {
        /// <summary>lr_db_codetable(数据库表信息)表的实体
        /// </summary>
        public CodeTableEntity Lr_db_codetableEntity { get; set; }
        /// <summary>lr_db_codecolumns(数据库表字段信息)表的实体
        /// </summary>
        public List<CodeColumnsEntity> Lr_db_codecolumnsList { get; set; }

    }
}
