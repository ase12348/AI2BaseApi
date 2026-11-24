using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：即时通讯用户注册
    /// </summary>
    public class IMSysUserBLL : BLLBase, IMSysUserIBLL, BLL
    {
        private readonly IMSysUserService iMSysUserService = new IMSysUserService();
        private readonly IMMsgIBLL _iMMsgIBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iMMsgIBLL"></param>
        public IMSysUserBLL(IMMsgIBLL iMMsgIBLL) {
            _iMMsgIBLL = iMMsgIBLL;
        }
        
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<IMSysUserEntity>> GetList(string keyWord)
        {
            return iMSysUserService.GetList(keyWord);
        }

        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<IMSysUserEntity>> GetPageList(Pagination pagination, string keyWord)
        {
            return iMSysUserService.GetPageList(pagination, keyWord);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<IMSysUserEntity> GetEntity(string keyValue)
        {
            return iMSysUserService.GetEntity(keyValue);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<bool> DeleteEntity(string keyValue)
        {
            var entity = await iMSysUserService.GetEntity(keyValue);
            if (entity != null) {
                var list = (List<IMMsgEntity>)await _iMMsgIBLL.GetPageSysList(new Pagination { page =1,rows =1},"", entity.F_Code,"",null);
                if (list.Count > 0) {
                    return false;
                }
                await iMSysUserService.DeleteEntity(keyValue);
            }

            return true;
        }
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, IMSysUserEntity entity)
        {
            await iMSysUserService.SaveEntity(keyValue, entity);
        }
        #endregion
    }
}
