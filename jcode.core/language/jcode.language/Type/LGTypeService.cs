using jcode.iapplication;
using jcode.ilanguage;
using System;
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
    public class LGTypeService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<LGTypeEntity>> GetList()
        {
            return this.BaseRepository().FindAll<LGTypeEntity>("F_IsMain DESC");
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
            var entity = await this.BaseRepository().FindEntityByKey<LGTypeEntity>(keyValue);
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<LGMapEntity>(t => t.F_TypeCode == entity.F_Code );
                await db.Delete<LGTypeEntity>(keyValue);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LGTypeEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                await this.BaseRepository().Update(entity);
            }
            else
            {
                entity.F_IsMain = 0;
                entity.F_Id = Guid.NewGuid().ToString();
                await this.BaseRepository().Insert(entity);
            }
        }
        #endregion
    }
}
