using System.Collections.Generic;

namespace jcode.iapplication;

/// <summary>
/// excel导入配置
/// </summary>
public class ExcelImportConfig
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
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
    /// Excel数据导入设置
    /// </summary>
    public ExcelImportEntity Entity { get; set; }
    /// <summary>
    /// Excel数据导入设置字段
    /// </summary>
    public List<ExcelImportFieldEntity> List { get; set; }
}