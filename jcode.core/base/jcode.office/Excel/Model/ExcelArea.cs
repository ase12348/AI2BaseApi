
using System.Collections.Generic;

namespace jcode.office
{
  /// <summary>
  /// excel数据区域
  /// </summary>
  public class ExcelArea
  {
    /// <summary>
    /// 数据类型 1.数据行 2.数据表格
    /// </summary>
    /// <value></value>
    public int Type { get; set; }

    /// <summary>
    /// x坐标
    /// </summary>
    /// <value></value>
    public int X { get; set; }

    /// <summary>
    /// y坐标
    /// </summary>
    /// <value></value>
    public int Y { get; set; }

    /// <summary>
    /// 行数据
    /// </summary>
    /// <value></value>
    public List<ExcelCell> Row { get; set; }

    /// <summary>
    /// 表格数据
    /// </summary>
    /// <value></value>
    public ExcelTable Table { get; set; }
  }
}