using jcode.autofac;
using jcode.iapplication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.07
    /// 描 述：我的常用移动应用 
    /// </summary>
    public class MyFunctionBLL:BLLBase, MyFunctionIBLL,BLL
    {
        private readonly MyFunctionService myFunctionService = new MyFunctionService();

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="userId">用户主键ID</param>
        /// <returns></returns>
        public Task<IEnumerable<MyFunctionEntity>> GetList(string userId)
        {
            return myFunctionService.GetList(userId);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="strFunctionId">功能id</param>
        /// <returns></returns>
        public async Task SaveEntity(string userId, string strFunctionId)
        {
            await myFunctionService.SaveEntity(userId, strFunctionId);
        }
        #endregion
    }
}
