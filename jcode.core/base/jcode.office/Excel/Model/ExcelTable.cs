using System.Collections.Generic;
using System.Data;

namespace jcode.office
{
  /// <summary>
  /// excel表格
  /// </summary>
  public class ExcelTable {
    
    /// <summary>
    /// 是否需要合并表格头部单元格
    /// </summary>
    public bool IsHeaderMerged{get;set;}
    
    /// <summary>
    /// 表格头部信息
    /// </summary>
    /// <value></value>
    public List<ExcelCell> Header {get;set;}
    /// <summary>
    /// 表格数据
    /// </summary>
    /// <value></value>
    public DataTable Data {get;set;}
  }
}