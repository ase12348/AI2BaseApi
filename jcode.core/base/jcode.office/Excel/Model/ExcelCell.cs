
namespace jcode.office
{
  /// <summary>
  /// excel单元格
  /// </summary>
  public class ExcelCell
  {
    /// <summary>
    /// 单元格文本
    /// </summary>
    /// <value></value>
    public string Text { get; set; }
    /// <summary>
    /// 字段属性
    /// </summary>
    /// <value></value>
    public string Prop {get;set;}
    /// <summary>
    /// 是否加粗
    /// </summary>
    /// <value></value>
    public bool IsBold{get;set;}
    /// <summary>
    /// 字号
    /// </summary>
    /// <value></value>
    public int FontSize{get;set;}
    /// <summary>
    /// 宽度
    /// </summary>
    public int Width{get;set;}
    
    /// <summary>
    /// 高度
    /// </summary>
    public int Height{get;set;}
    
    /// <summary>
    /// 对齐方式
    /// </summary>
    public string Align{get;set;}
    
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
    /// 列合并
    /// </summary>
    public int ColSpan{ get; set; }
    /// <summary>
    /// 行合并
    /// </summary>
    public int RowSpan{ get; set; }

  }
}