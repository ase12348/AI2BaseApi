using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-OA模块
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期： 2021-08-19 15:29:43
    /// 描 述： 桌面配置数据库执行类
    /// </summary>
    public class SettingService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取桌面配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_infoEntity>> GetList(Lr_desktop_infoEntity queryParams)
        {
            var expression = LinqExtensions.True<Lr_desktop_infoEntity>();
            
            if(!string.IsNullOrEmpty(queryParams.F_Name))
            {
                expression = expression.And(t => t.F_Name.Contains(queryParams.F_Name));
            }

            if (!string.IsNullOrEmpty(queryParams.F_AppId))
            {
                expression = expression.And(t => t.F_AppId == queryParams.F_AppId);
            }

            if(queryParams.F_Type != null)
            {
                expression = expression.And(t => t.F_Type == queryParams.F_Type);
            }

            if (queryParams.F_EnabledMark != null)
            {
                expression = expression.And(t => t.F_EnabledMark == queryParams.F_EnabledMark);
            }
            
            
            if (queryParams.F_IsFirst != null)
            {
                expression = expression.And(t => t.F_IsFirst == queryParams.F_IsFirst);
            }


            return this.BaseRepository().FindList(expression);
        }

        /// <summary>
        /// 获取桌面配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_infoEntity>> GetPageList(Pagination pagination, Lr_desktop_infoEntity queryParams, string tags = null)
        {
            return this.BaseRepository().FindListByQueryable<Lr_desktop_infoEntity>(q => {
                var queryable = q.LeftJoin<Lr_desktop_schemeEntity>((t,t1)=>t.F_SchemeId == t1.F_Id);
                var exp = Expressionable.Create<Lr_desktop_infoEntity>()
                    .AndIF(queryParams.F_Type != null, t => t.F_Type == queryParams.F_Type)
                    .AndIF(!string.IsNullOrEmpty(queryParams.F_Name), t => t.F_Name.Contains(queryParams.F_Name))
                    .AndIF(!string.IsNullOrEmpty(queryParams.F_AppId), t => t.F_AppId == queryParams.F_AppId)
                    // 标签过滤：F_Category 存逗号分隔的标签串，包含匹配即可命中其中任一标签
                    .AndIF(!string.IsNullOrEmpty(tags), t => t.F_Category.Contains(tags));

                return queryable
                .Where(exp.ToExpression())
                .Select((t, t1) =>new Lr_desktop_infoEntity()
                { 
                    F_Id = t.F_Id,
                    F_Name = t.F_Name,
                    F_Type = t.F_Type,
                    F_Code = t.F_Code,
                    F_EnabledMark = t.F_EnabledMark,
                    F_IsFirst = t.F_IsFirst,
                    F_SchemeId = t.F_SchemeId,
                    F_SortCode = t.F_SortCode,
                    F_ReleaseId = t.F_ReleaseId,
                    F_ModuleId = t.F_ModuleId,
                    F_Category = t.F_Category,
                    F_AppId = t.F_AppId,

                    F_CreateTime = t1.F_CreateTime,
                    F_CreateUserId = t1.F_CreateUserId
                });
            }, pagination);
        }
        
        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        public Task<Lr_desktop_infoEntity> GetEntity(string f_Id)
        {
            return this.BaseRepository().FindEntityByKey<Lr_desktop_infoEntity>(f_Id);
        }

        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<Lr_desktop_infoEntity> GetEntityByCode(String code)
        {
            return this.BaseRepository().FindEntity<Lr_desktop_infoEntity>(t=>t.F_Code == code);
        }


        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="moduleId">菜单id</param>
        /// <returns></returns>
        public Task<Lr_desktop_infoEntity> GetEntityByModuleId(String moduleId)
        {
            return this.BaseRepository().FindEntity<Lr_desktop_infoEntity>(t => t.F_ModuleId == moduleId || t.F_Id == moduleId);
        }

        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        public Task<Lr_desktop_schemeEntity> GetLr_desktop_scheme(String f_Id)
        {
            return this.BaseRepository().FindEntityByKey<Lr_desktop_schemeEntity>(f_Id);
        }
        
        
        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<Lr_desktop_schemeEntity> GetSchemeEntityByInfoId(string keyValue)
        {
            var pagination = new Pagination()
            {
                page = 1,
                sidx = "F_CreateTime DESC",
                rows = 1,
            };
            var list = (List<Lr_desktop_schemeEntity>)await this.BaseRepository(false).FindList<Lr_desktop_schemeEntity>(t=>t.F_InfoId == keyValue && t.F_Type == 1, pagination);

            if (list.Count > 0)
            {
                return list[0];
            }

            return null;
        }


        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="f_InfoId">关联键</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_schemeEntity>> GetLr_desktop_schemeList(string f_InfoId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT F_Id,F_CreateTime,F_CreateUserId,F_Ver,F_Des,F_InfoId,F_Type ");
            strSql.Append(" FROM lr_desktop_scheme t WHERE 1=1 ");
            strSql.Append(" AND F_InfoId = @f_InfoId {JCODE_SASSID} order by  F_CreateTime DESC ");



            return this.BaseRepository().FindList<Lr_desktop_schemeEntity>(strSql.ToString(), new { f_InfoId });
        }


        /// <summary>
        /// 获取表lr_desktop_scheme的分页列表
        /// </summary>
        /// <param name="f_InfoId">关联键</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_schemeEntity>> GetLr_desktop_schemePage(String f_InfoId, Pagination pagination)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT F_Id,F_CreateTime,F_CreateUserId,F_Ver,F_Des,F_InfoId,F_Type ");
            strSql.Append(" FROM lr_desktop_scheme t WHERE 1=1 {JCODE_SASSID} ");
            strSql.Append(" AND F_InfoId = @f_InfoId ");


            return this.BaseRepository().FindList<Lr_desktop_schemeEntity>(strSql.ToString(),new { f_InfoId }, pagination);
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">主键</param>
        public async Task Delete(String f_Id)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<Lr_desktop_infoEntity>(f_Id);
                await db.Delete<Lr_desktop_schemeEntity>(t=>t.F_InfoId == f_Id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 删除lr_desktop_scheme表数据
        /// </summary>
        /// <param name="id">主键</param>
        public async Task DeleteLr_desktop_scheme(String id)
        {
            await this.BaseRepository().Delete<Lr_desktop_schemeEntity>(t=>t.F_Id == id);
        }
        /// <summary>
        /// 保存(新增,更新)
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <param name="lr_desktop_infoEntity">lr_desktop_info实体数据</param>
        /// <param name="lr_desktop_schemeEntity">lr_desktop_scheme实体数据列表</param>
        public async Task SaveEntity(String f_Id,Lr_desktop_infoEntity lr_desktop_infoEntity, Lr_desktop_schemeEntity lr_desktop_schemeEntity)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                if (lr_desktop_schemeEntity != null)
                {
                    lr_desktop_schemeEntity.F_Id = Guid.NewGuid().ToString();
                    lr_desktop_infoEntity.F_SchemeId = lr_desktop_schemeEntity.F_Id;
                }

                if (!string.IsNullOrEmpty(f_Id)) // 更新
                {
                    lr_desktop_infoEntity.F_Id = f_Id;
                    // 更新是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                    if (string.IsNullOrEmpty(lr_desktop_infoEntity.F_AppId))
                    {
                        var old = await db.FindEntityByKey<Lr_desktop_infoEntity>(f_Id);
                        lr_desktop_infoEntity.F_AppId = old?.F_AppId;
                    }
                    await db.Update(lr_desktop_infoEntity);
                }
                else // 新增
                {
                    if(string.IsNullOrEmpty(lr_desktop_infoEntity.F_Id))
                    {
                        lr_desktop_infoEntity.F_Id = Guid.NewGuid().ToString();
                    }
                    await db.Insert(lr_desktop_infoEntity);
                }

                if (lr_desktop_schemeEntity != null) {
                    lr_desktop_schemeEntity.F_CreateTime = DateTime.Now;
                    lr_desktop_schemeEntity.F_CreateUserId = this.GetUserId();
                    lr_desktop_schemeEntity.F_InfoId = lr_desktop_infoEntity.F_Id;
                    await db.Insert(lr_desktop_schemeEntity);
                }


                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="lr_desktop_infoEntity"></param>
        /// <returns></returns>
        public async Task UpdateEntity(Lr_desktop_infoEntity lr_desktop_infoEntity) {
            await this.BaseRepository().Update(lr_desktop_infoEntity,true);
        }
        #endregion
        

    }
}
