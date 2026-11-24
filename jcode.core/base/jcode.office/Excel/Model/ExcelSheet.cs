
using System.Collections.Generic;

namespace jcode.office
{
  /// <summary>
  /// excel工作表
  /// </summary>
  public class ExcelSheet
  {
    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    public string Name {get;set;}
    /// <summary>
    /// 数据
    /// </summary>
    /// <value></value>
    public List<ExcelArea> Data { get; set; }
  }
}