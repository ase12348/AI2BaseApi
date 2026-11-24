using jcode.autofac;
using jcode.iapplication;
using jcode.ilanguage;
using jcode.util;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using jcode.cache;

namespace jcode.language
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.22
    /// 描 述：语言映射
    /// </summary>
    public class LGMapBLL : BLLBase, LGMapIBLL, BLL
    {
        private readonly LGMapService lGMapService = new LGMapService();
        private readonly LGTypeIBLL _lgTypeIBLL;
        private readonly DataItemIBLL _dataItemIBLL;
        private readonly ModuleIBLL _moduleIBLL;
        private readonly ICache _iCache;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lgTypeIBLL"></param>
        /// <param name="dataItemIBLL"></param>
        /// <param name="moduleIBLL"></param>
        public LGMapBLL(ICache iCache, LGTypeIBLL lgTypeIBLL, DataItemIBLL dataItemIBLL, ModuleIBLL moduleIBLL) {
            _lgTypeIBLL = lgTypeIBLL;
            _dataItemIBLL = dataItemIBLL;
            _moduleIBLL = moduleIBLL;

            _iCache = iCache;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        public async Task<DataTable> GetPageList(Pagination pagination, string keyword)
        {
            var typeList = await _lgTypeIBLL.GetList();
            return await lGMapService.GetPageList(pagination, keyword, typeList);
        }

        /// <summary>
        /// 获取语言包映射关系数据集合
        /// </summary>
        /// <param name="code">语言包编码</param>
        /// <param name="mainCode">主语言包</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetMap(string code,string mainCode)
        {
            var mainMap = new Dictionary<string, string>();
            
            var mainList = await lGMapService.GetListByTypeCode(mainCode);
            var myList = await lGMapService.GetListByTypeCode(code);

            if (code == mainCode)
            {
                return mainMap;
            }

            var key = $"jcode_lang_{code}";
            var map = _iCache.Read<Dictionary<string, string>>(key);
            if (map != null)
            {
                return map;
            }

            map = new Dictionary<string, string>();
            



            foreach (var item in mainList)
            {
                if (!mainMap.ContainsKey(item.F_Code))
                {
                    mainMap.Add(item.F_Code,item.F_Name);
                }
            }
            foreach (var item in myList)
            {
                if (mainMap.ContainsKey(item.F_Code)) {
                    if (!map.ContainsKey(mainMap[item.F_Code]))
                    {
                        map.Add(mainMap[item.F_Code], item.F_Name);
                    }
                }
            }
            _iCache.Write(key,map);;
            return map;
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="TypeCode">编码</param>
        /// <returns></returns>
        public Task<IEnumerable<LGMapEntity>> GetListByTypeCode(string TypeCode)
        {
            return lGMapService.GetListByTypeCode(TypeCode);
        }

        /// <summary>
        /// 获取数据字典翻译数据
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public async Task<DataTable> GetDataItemList(string code) {
            var typeList = await _lgTypeIBLL.GetList();
            var data = await _dataItemIBLL.GetDetailList(code);
            List<string> nameList = new List<string>();
            foreach (var item in data) {
                nameList.Add(item.F_ItemName);
            }
            var list =  await lGMapService.GetList(nameList, typeList);
            return list;
        }

        /// <summary>
        /// 获取菜单翻译数据
        /// </summary>
        /// <param name="moduleId">模块编码</param>
        /// <param name="isParent">是否是父级 1是 0不是</param>
        /// <returns></returns>
        public async Task<DataTable> GetModuleList(string moduleId,int isParent)
        {
            var typeList = await _lgTypeIBLL.GetList();
            List<string> nameList = new List<string>();
            if (isParent == 1)
            {
                var data = await _moduleIBLL.GetModuleListByParentId(moduleId);

                foreach (var item in data)
                {
                    nameList.Add(item.F_FullName);
                }
            }
           
            
          
            var list = await lGMapService.GetList(nameList, typeList);
            return list;
        }
        
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="code">一组语言编码</param>
        /// <returns></returns>
        public async Task DeleteEntity(string code)
        {
            await lGMapService.DeleteEntity(code);
            await RemoveCache();
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="list">翻译集合</param>
        /// <param name="code">一组语言编码</param>
        /// <returns></returns>
        public async Task SaveEntity(List<LGMapEntity> list, string code)
        {
            await lGMapService.SaveEntity(list, code);
            await RemoveCache();
        }
        
        /// <summary>
        /// 清空缓存
        /// </summary>
        private async Task RemoveCache()
        {
            var typeList = await _lgTypeIBLL.GetList();
            foreach (var item in typeList)
            {
                var key = $"jcode_lang_{item.F_Code}";
                _iCache.Remove(key);
            }
        }
        #endregion

    }
}
