using jcode.autofac;
using jcode.ilanguage;
using jcode.operat;

namespace jcode.web.util
{
    public class LangHelper
    {
        #region 多语言
        
        /// <summary>
        /// 获取翻译
        /// </summary>
        /// <param name="label">当前语言</param>
        /// <returns></returns>
        public static string GetLang(string label)
        {
            var langMainType = RequestParams.GetHeadersParamValue("langMType");
            var langType = RequestParams.GetHeadersParamValue("langType");
            if (!string.IsNullOrEmpty(langMainType)
                && !string.IsNullOrEmpty(langType) && langMainType != langType)
            {
                var langData = IocManager.Instance.GetService<LGMapIBLL>().GetMap(langType,langMainType).GetAwaiter().GetResult();
                if (langData.ContainsKey(label))
                {
                    return langData[label];
                }
                else
                {
                    return label;
                }
            }

            return label;
        }

        #endregion
    }
}