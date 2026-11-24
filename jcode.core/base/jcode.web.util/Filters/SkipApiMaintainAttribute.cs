using System;

namespace jcode.web.util.Filters
{
    /// <summary>
    /// 标在动作上：RequestFilter 既不查 api_maintain，也不写请求日志。
    /// 给「宿主自己还没有可用主库时也必须答得出来」的窄口用（首次安装的 data/dblink/test-local）：
    /// api_maintain 查询会先连主库，库不通时这条口会被 500 挡在门口，测连接根本走不到动作里。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SkipApiMaintainAttribute : Attribute
    {
    }
}
