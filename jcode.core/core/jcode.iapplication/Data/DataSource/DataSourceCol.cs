
namespace jcode.iapplication
{
  /// <summary>
  /// 数据源转化列
  /// </summary>
  public class DataSourceCol
  {
    /// <summary>
    /// 数据源编码
    /// </summary>
    /// <value></value>
    public string Code { get; set; }

    /// <summary>
    /// 对应值字段
    /// </summary>
    /// <value></value>
    public string Valuekey { get; set; }

    /// <summary>
    /// 显示值字段
    /// </summary>
    /// <value></value>
    public string LabelKey { get; set; }

    /// <summary>
    /// 需要转化的列名
    /// </summary>
    /// <value></value>
    public string Prop { get; set; }

  }
}
