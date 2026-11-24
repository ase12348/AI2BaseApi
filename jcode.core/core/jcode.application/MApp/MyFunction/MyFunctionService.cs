using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.07
    /// 描 述：我的常用移动应用 
    /// </summary>
    public class MyFunctionService: ServiceBase
    {
        #region 获取数据 
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="userId">用户主键ID</param>
        /// <returns></returns>
        public Task<IEnumerable<MyFunctionEntity>> GetList(string userId)
        {
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                var appCode = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
                return this.BaseRepository().FindList<MyFunctionEntity>(t=>t.F_UserId == userId && appCode.Contains(t.F_AppCode), "F_Sort");
            }
            return this.BaseRepository().FindList<MyFunctionEntity>(t=>t.F_UserId == userId, "F_Sort");
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="strFunctionId">功能id</param>
        /// <returns></returns>
        public async Task SaveEntity(string userId,string strFunctionId)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                string[] functionIds = strFunctionId.Split(',');
                await db.Delete<MyFunctionEntity>(t => t.F_UserId == userId);
                int num = 0;
                foreach (var functionId in functionIds) {
                    MyFunctionEntity entity = new MyFunctionEntity
                    {
                        F_Id = Guid.NewGuid().ToString(),
                        F_UserId = userId,
                        F_FunctionId = functionId,
                        F_Sort = num
                    };
                    await db.Insert(entity);
                    num++;
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        #endregion
    }
}
