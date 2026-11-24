using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace jcode.web.util.Swagger
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.17
    /// 描 述：API分组约定
    /// </summary>
    public class ApiExplorerGroupConvention : IControllerModelConvention
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ControllerModel controller)
        {
            if (string.IsNullOrWhiteSpace(controller.ControllerType.Namespace))
                return;

            string[] array = controller.ControllerType.FullName.Split('.');
            if (array.Length == 7)
            {
                controller.ApiExplorer.GroupName = array[2] + "_" + array[5];
                return;
            }
            controller.ApiExplorer.GroupName = array[2];
        }
    }
}
