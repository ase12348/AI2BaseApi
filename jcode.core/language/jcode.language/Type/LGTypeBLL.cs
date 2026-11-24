using jcode.autofac;
using jcode.iapplication;
using jcode.ilanguage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.language
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.22
    /// 描 述：语言类型
    /// </summary>
    public class LGTypeBLL : BLLBase, LGTypeIBLL, BLL
    {
        private LGTypeService lGTypeService = new LGTypeService();

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<LGTypeEntity>> GetList()
        {
            return lGTypeService.GetList();
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteEntity(string keyValue)
        {
            await lGTypeService.DeleteEntity(keyValue);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LGTypeEntity entity)
        {
            await lGTypeService.SaveEntity(keyValue, entity);
        }
        #endregion

    }
}
