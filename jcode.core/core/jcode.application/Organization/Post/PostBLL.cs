using jcode.autofac;
using jcode.iapplication;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.cache;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：岗位管理
    /// </summary>
    public class PostBLL :BLLBase , PostIBLL ,BLL
    {
        private readonly PostService postService = new PostService();
        private readonly ICache _iCache;
        
        public PostBLL(ICache iCache ) {
            _iCache = iCache;
        }
        
        #region 获取数据
        /// <summary>
        /// 获取岗位数据列表（根据公司列表）
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <returns></returns>
        public Task<IEnumerable<PostEntity>> GetList(string companyId)
        {
            return postService.GetList(companyId);
        }
        /// <summary>
        /// 获取岗位数据列表（根据公司列表）
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门Id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<PostEntity>> GetList(string companyId, string departmentId, string keyword)
        {
            return postService.GetList(companyId, departmentId, keyword);
        }
        /// <summary>
        /// 获取岗位数据列表(根据主键串)
        /// </summary>
        /// <param name="postIds">根据主键串</param>
        /// <returns></returns>
        public Task<IEnumerable<PostEntity>> GetListByPostIds(string postIds)
        {
            return postService.GetListByPostIds(postIds);
        }
        /// <summary>
        /// 获取岗位实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<PostEntity> GetEntity(string keyValue) {
            string key = $"post_{keyValue}";
            var entity = _iCache.Read<PostEntity>(key);
            if (entity == null)
            {
                entity = await postService.GetEntity(keyValue);
                _iCache.Write(key,entity);
            }
            return entity;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await postService.Delete(keyValue);
            string key = $"post_{keyValue}";
            _iCache.Remove(key);
        }
        /// <summary>
        /// 保存岗位（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="postEntity">岗位实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, PostEntity postEntity)
        {
            await postService.SaveEntity(keyValue, postEntity);
            string key = $"post_{keyValue}";
            _iCache.Remove(key);
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 判断是否是有关联
        /// </summary>
        /// <param name="beginId">开始岗位主键</param>
        /// <param name="map">对方的岗位集合</param>
        /// <returns></returns>
        private async Task<bool> HasRelation(string beginId, Dictionary<string,int> map) {
            bool res = false;
            var entity = await postService.GetEntity(beginId);
            if (entity == null || entity.F_ParentId == "0")
            {
                res = false;
            }
            else if (map.ContainsKey(entity.F_ParentId))
            {
                res = true;
            }
            else {
                res =await HasRelation(entity.F_ParentId, map);
            }
            return res;
        }

        /// <summary>
        /// 判断是否是上级
        /// </summary>
        /// <param name="myId">自己的岗位</param>
        /// <param name="otherIds">对方的岗位</param>
        /// <returns></returns>
        public async Task<bool> IsUp(string myId,List<string> otherIds) {
            bool res = false;
            if (!string.IsNullOrEmpty(myId)){
                Dictionary<string, int> map = new Dictionary<string, int>();
                foreach (var otherItem in otherIds )
                {
                    if (!map.ContainsKey(otherItem)) {
                        map.Add(otherItem, 1);
                    }
                }
                if (await HasRelation(myId, map)) {
                    res = true;
                }
            }
            return res;
        }
        /// <summary>
        /// 判断是否是下级
        /// </summary>
        /// <param name="myId">自己的岗位</param>
        /// <param name="otherIds">对方的岗位</param>
        /// <returns></returns>
        public async Task<bool> IsDown(string myId, List<string> otherIds)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(myId))
            {
                Dictionary<string, int> map = new Dictionary<string, int>();
                map.Add(myId, 1);
                foreach (var otherItem in otherIds)
                {
                    if (await HasRelation(otherItem, map))
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 获取上级岗位人员ID
        /// </summary>
        /// <param name="postId">岗位</param>
        /// <param name="level">级数</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetUpIdList(string postId, int level) {
            List<string> res = new List<string>();
            if (!string.IsNullOrEmpty(postId) && level > 0 && level < 6) {// 现在支持1-5级查找
                bool isHave = false; // 判断是否指定级数的职位
                var entity = await postService.GetEntity(postId);
                if (entity != null) {
                    string parentId = entity.F_ParentId;
                    PostEntity parentEntity = null;
                    for (int i = 0; i < level; i++)
                    {
                        parentEntity = await postService.GetEntity(parentId);
                        if (parentEntity != null)
                        {
                            parentId = parentEntity.F_ParentId;
                            if (i == (level - 1))
                            {
                                isHave = true;
                            }
                        }
                        else {
                            break;
                        }
                    }
                    if (isHave)
                    {
                        if (parentEntity != null) {
                            res.Add(parentEntity.F_PostId);
                        }
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 获取下级岗位人员ID
        /// </summary>
        /// <param name="postId">岗位</param>
        /// <param name="level">级数</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDownIdList(string postId, int level)
        {
            List<string> res = new List<string>();
            if (!string.IsNullOrEmpty(postId) &&  level > 0 && level < 6)
            {// 现在支持1-5级查找
                bool isHave = false; // 判断是否指定级数的职位
                List<string> parentList = new List<string>();
                parentList.Add(postId);
                for (int i = 0; i < level; i++)
                {
                    parentList =(List<string>) await postService.GetIdList(parentList);
                    if (parentList.Count > 0)
                    {
                        if (i == (level - 1))
                        {
                            isHave = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (isHave)
                {
                    res.AddRange(parentList);
                }
            }
            return res;
        }
        #endregion
    }
}
