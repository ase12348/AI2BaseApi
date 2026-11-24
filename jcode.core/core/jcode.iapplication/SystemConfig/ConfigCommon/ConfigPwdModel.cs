namespace jcode.iapplication;

/// <summary>
/// 密码设置
/// </summary>
public class ConfigPwdModel
{
    /// <summary>
    /// 更换周期/天
    /// </summary>
    public int? PwdDay { get; set; }
    /// <summary>
    /// 密码长度/指定>=6位以上的长度
    /// </summary>
    public int? PwdLength { get; set; }
    /// <summary>
    /// 复杂度(多选，1大写字母、2.小写字母、3数字、4.特殊字符)
    /// </summary>
    public string PwdComplexity { get; set; }
}