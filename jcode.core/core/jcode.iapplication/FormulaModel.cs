

namespace jcode.iapplication;


/// <summary>
/// 设置条件
/// </summary>
public class Condition
{
    /// <summary>
    /// 编号
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 字段ID
    /// </summary>
    /// <returns></returns>
    public string FieldId { get; set; }
    /// <summary>
    /// 比较符1.等于2.大于3.大于等于4.小于5.小于等于6.包含7.包含于8.不等于9.不包含10.不包含于
    /// </summary>
    /// <returns></returns>
    public int? Symbol { get; set; }
    /// <summary>
    /// 字段值类型1.文本(字串)10.文本（数字）2.登录者ID3.登录者账号4.登录者公司41.登录者公司及下属公司42.固定公司5.登录者部门51.登录者部门及下属部门52.固定部门6.登录者岗位7.登录者角色
    /// </summary>
    public int? FiledValueType { get; set; }
    /// <summary>
    /// 字段值
    /// </summary>
    /// <returns></returns>
    public string FiledValue { get; set; }
}