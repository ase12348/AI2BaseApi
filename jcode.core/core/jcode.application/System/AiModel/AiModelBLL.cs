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
    /// 日 期：2026.09.20
    /// 描 述：模型管理 
    /// </summary>
    public class AiModelBLL : BLLBase, AiModelIBLL, BLL
    {
        private readonly AiModelService aiModelService = new AiModelService();

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<AiModelEntity>> GetList(string keyword)
        {
            return aiModelService.GetList(keyword);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<AiModelEntity> GetEntity(string keyValue)
        {
            return aiModelService.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        public async Task DeleteEntity(string id)
        {
            await aiModelService.DeleteEntity(id);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string id, AiModelEntity entity)
        {
            await aiModelService.SaveEntity(id, entity);
        }
        #endregion
    }
}
