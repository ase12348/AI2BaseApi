namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.09
    /// 描 述：分页参数
    /// </summary>
    public class Pagination: PaginationInputDto
    {

        /// <summary>
        /// 总记录数
        /// </summary>
        public long records { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long total
        {
            get
            {
                if (records > 0)
                {
                    return records % this.rows == 0 ? records / this.rows : records / this.rows + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }


    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.14
    /// 描 述：分页参数
    /// </summary>
    public class PaginationInputDto {
        /// <summary>
        /// 每页行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string sidx { get; set; }
        /// <summary>
        /// 排序类型
        /// </summary>
        public string sord { get; set; }
    }
    /// <summary>
    /// 分页返回数据
    /// </summary>
    public class PaginationOutputDto<T> where T : class
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T rows { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public long records { get; set; }
    }
}
