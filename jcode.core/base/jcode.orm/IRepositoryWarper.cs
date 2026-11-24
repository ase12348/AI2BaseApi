namespace jcode.orm
{
    /// <summary>
    /// 数据库仓库对象包裹
    /// </summary>
    public class IRepositoryWarper
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        ///  数据库仓库对象
        /// </summary>
        public IRepository Db { get; set; }
    }
}
