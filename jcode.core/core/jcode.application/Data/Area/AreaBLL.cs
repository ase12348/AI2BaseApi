using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.18
    /// 描 述：行政区域
    /// </summary>
    public class AreaBLL : BLLBase, AreaIBLL,BLL
    {
        private readonly AreaService areaService = new AreaService();
        private readonly ICache _iCache;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public AreaBLL(ICache iCache)
        {
            _iCache = iCache;
        }

        #region 获取数据
        /// <summary>
        /// 获取区域列表数据
        /// </summary>
        /// <param name="parentId">父节点主键（0表示顶层）</param>
        /// <returns></returns>
        public async Task<IEnumerable<AreaEntity>> GetList(string parentId)
        {
            var list = _iCache.Read<IEnumerable<AreaEntity>>($"jcode_area_{parentId}");
            if (list == null)
            {
                list = await areaService.GetList(parentId, string.Empty);
                _iCache.Write($"jcode_area_{parentId}", list);
            }
            /*var code =(string)await this.GetRuleCodeEx("jcode_code_csbm|1234");
            var list2 = (List<AreaEntity>)list;
            list2[0].F_AreaCode = code.Split(" - ");*/
            return list;
        }
        /// <summary>
        /// 区域实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<AreaEntity> GetEntity(string keyValue)
        {
            return areaService.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var entity = await GetEntity(keyValue);
            if (entity != null) {
                _iCache.Remove($"jcode_area_{entity.F_ParentId}");
            }
            
            await areaService.Delete(keyValue);
        }
        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="areaEntity">区域实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, AreaEntity areaEntity)
        {
            if (areaEntity.F_ParentId != "0")
            {
                AreaEntity entity = await GetEntity(areaEntity.F_ParentId);
                if (entity != null)
                {
                    areaEntity.F_Layer = entity.F_Layer + 1;
                }
            }
            else
            {
                areaEntity.F_Layer = 1;
            }

            if (!string.IsNullOrEmpty(keyValue))
            {
                var oldEntity = await GetEntity(keyValue);
                if (oldEntity.F_ParentId != areaEntity.F_ParentId) {
                    _iCache.Remove($"jcode_area_{oldEntity.F_ParentId}");
                }
            }
            _iCache.Remove($"jcode_area_{areaEntity.F_ParentId}");

            await areaService.SaveEntity(keyValue, areaEntity);
        }

        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="areaEntitys">区域实体</param>
        /// <returns></returns>
        public async Task SaveEntitys(List<AreaEntity> areaEntitys)
        {
            await areaService.SaveEntitys(areaEntitys);
        }

        #endregion
    }
}
