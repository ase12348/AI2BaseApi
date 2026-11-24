using jcode.autofac;
using jcode.iapplication;
using System.Text;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 打印设计-通用模板导入
    /// </summary>
    [BLLName("printtemplateimport")]
    public class PrintTemplateImport:IFileImportMethod, BLL
    {
        private readonly ILrPrintTemplateinfoBLL _lrPrintTemplateBLL;

        public PrintTemplateImport(ILrPrintTemplateinfoBLL lrPrintTemplateBLL)
        {
            _lrPrintTemplateBLL = lrPrintTemplateBLL;
        }

        /// <summary>
        /// 数据导入处理
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <returns></returns>
        public async Task<bool> Execute(byte[] data)
        {
            return await _lrPrintTemplateBLL.Import(Encoding.UTF8.GetString(data));
        }
    }
}
