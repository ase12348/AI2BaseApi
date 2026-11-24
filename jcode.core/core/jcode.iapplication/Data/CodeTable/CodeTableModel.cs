using System.Collections.Generic;

namespace jcode.iapplication.Data.CodeTable;

/// <summary>
/// 表数据更新模型
/// </summary>
public class TableDataUpdateModel
{
    /// <summary>
    /// 数据库编码
    /// </summary>
    public string DbCode { get; set; }

    /// <summary>
    /// 数据库表
    /// </summary>
    public string DbTable { get; set; }

    /// <summary>
    /// 需要更新的数据
    /// </summary>
    public List<TableDataFieldModel> UpdateData { get; set; }

    /// <summary>
    /// 设置条件
    /// </summary>
    public List<Condition> Conditions { get; set; }
    
    /// <summary>
    /// 设置公式
    /// </summary>
    public string Formula { get; set; }
}

/// <summary>
/// 表数据
/// </summary>
public class TableDataFieldModel
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string CsType { get; set; }
    
    /// <summary>
    /// 数据值
    /// </summary>
    public string Value { get; set; }
}