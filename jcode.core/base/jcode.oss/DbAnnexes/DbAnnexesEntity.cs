using System;
using jcode.orm;

namespace jcode.oss
{
    [MyTable("lr_base_filedata")]
    public class DbAnnexesEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 文件id
        /// </summary>
        public string F_FileId { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int F_Index{ get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string F_Path { get; set; }
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
        /// 文件内容
        /// </summary>
        public byte[] F_Content{ get; set; }
    }
}