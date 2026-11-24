
using System.Text;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;

namespace jcode.application
{
    /// <summary>
    /// 桌面设计模版导入导入
    /// </summary>
    [BLLName("SettingImport")]
    public class SettingImport : IFileImportMethod, BLL
    {
        private readonly ISettingBLL _iSettingBLL;

        public SettingImport(ISettingBLL iSettingBLL)
        {
            _iSettingBLL = iSettingBLL;
        }

        /// <summary>
        /// 数据导入处理
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <returns></returns>
        public async Task<bool> Execute(byte[] data)
        {
            await _iSettingBLL.Import(Encoding.UTF8.GetString(data));
            return true;
        }
    }
}

