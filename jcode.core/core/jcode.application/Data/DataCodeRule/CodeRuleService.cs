using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：编号规则
    /// </summary>
    public class CodeRuleService : ServiceBase
    {
        //private static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> queueDic = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        

        #region 获取数据
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="keyword">查询参数</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的数据，为空时不过滤</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeRuleEntity>> GetPageList(Pagination pagination, string keyword, string appId = null)
        {
            var exp = Expressionable.Create<CodeRuleEntity>()
                .And(t => t.F_EnabledMark == 1)
                .And(t => t.F_DeleteMark == 0)
                .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_EnCode.Contains(keyword) || t.F_FullName.Contains(keyword))
                .AndIF(!string.IsNullOrEmpty(appId), t => t.F_AppId == appId);
            return this.BaseRepository().FindList<CodeRuleEntity>(exp.ToExpression(), pagination);
        }
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CodeRuleEntity>> GetList()
        {
            return this.BaseRepository().FindList<CodeRuleEntity>(t => t.F_EnabledMark == 1 && t.F_DeleteMark == 0);
        }
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<CodeRuleEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<CodeRuleEntity>(keyValue);
        }
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="enCode">规则编码</param>
        /// <returns></returns>
        public Task<CodeRuleEntity> GetEntityByCode(string enCode)
        {
            return this.BaseRepository().FindEntity<CodeRuleEntity>(t => t.F_EnabledMark == 1 && t.F_DeleteMark == 0 && t.F_EnCode == enCode);
        }
        /// <summary>
        /// 获取种子
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public Task<CodeRuleSeedEntity> GetSeedEntity(string ruleId)
        {
            return this.BaseRepository().FindEntity<CodeRuleSeedEntity>(t=>t.F_RuleId == ruleId);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<CodeRuleEntity>(keyValue);
                await db.Delete<CodeRuleSeedEntity>(t => t.F_RuleId == keyValue );
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存规则表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="codeRuleEntity">规则实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, CodeRuleEntity codeRuleEntity)
        {
            codeRuleEntity.F_ModifyDate = DateTime.Now;
            codeRuleEntity.F_ModifyUserId = this.GetUserId();
            codeRuleEntity.F_ModifyUserName = this.GetUserName();
            if (!string.IsNullOrEmpty(keyValue))
            {
                codeRuleEntity.F_RuleId = keyValue;
                // 更新实体是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                if (string.IsNullOrEmpty(codeRuleEntity.F_AppId))
                {
                    var old = await this.BaseRepository().FindEntityByKey<CodeRuleEntity>(keyValue);
                    codeRuleEntity.F_AppId = old?.F_AppId;
                }
                await this.BaseRepository().Update(codeRuleEntity);
            }
            else
            {
                codeRuleEntity.F_RuleId = Guid.NewGuid().ToString();
                codeRuleEntity.F_CreateDate = DateTime.Now;
                codeRuleEntity.F_DeleteMark = 0;
                codeRuleEntity.F_EnabledMark = 1;
                codeRuleEntity.F_CreateUserId = this.GetUserId();
                codeRuleEntity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(codeRuleEntity);
            }
        }
        #endregion

        #region 单据编码处理
        /// <summary>
        /// 获取当前编码规则流程号值
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="initValue"></param>
        /// <param name="stepNum"></param>
        /// <param name="dateFormatStr"></param>
        /// <returns></returns>
        public async Task<int> GetSeedValue(string ruleId,int initValue,int stepNum,string dateFormatStr)
        {
            // 设置队列，进行排序获取，防止单据编码重复
            /*var queue = queueDic.GetOrAdd(ruleId, new ConcurrentQueue<string>());
            var token = Guid.NewGuid().ToString();
            queue.Enqueue(token);
            queue.TryPeek(out string newToken);
            if (token != newToken)
            {
                while (true) {
                    Thread.Sleep(1);
                    queue.TryPeek(out newToken);
                    if (token == newToken) {
                        break;
                    }
                }
            }*/
            
            /*var myCodeRuleSeed = new CodeRuleSeedEntity();
            myCodeRuleSeed.F_RuleSeedId = ruleId + "1";
            try
            {
                await this.BaseRepository().Insert(myCodeRuleSeed);
            }
            catch (Exception)
            {
                value = await GetSeedValue(ruleId, initValue, stepNum, dateFormatStr);
                return value;
            }*/

            int value = 0;
            var db = this.BaseRepository().BeginTrans();
            try
            {
                var codeRuleSeed = await db.FindEntity<CodeRuleSeedEntity>(t => t.F_RuleId == ruleId);
                if (codeRuleSeed == null)
                {
                    //说明没有种子，插入一条种子
                    codeRuleSeed = new CodeRuleSeedEntity();
                    codeRuleSeed.F_Id = Guid.NewGuid().ToString();
                    codeRuleSeed.F_CreateDate = DateTime.Now;
                    codeRuleSeed.F_ModifyDate = DateTime.Now;
                    codeRuleSeed.F_SeedValue = initValue;
                    codeRuleSeed.F_RuleId = ruleId;

                    value = (int)codeRuleSeed.F_SeedValue;
                    codeRuleSeed.F_SeedValue += stepNum;

                    await db.Insert(codeRuleSeed);
                }
                else {
                    if (dateFormatStr.Contains("dd"))
                    {
                        if ((codeRuleSeed.F_ModifyDate).ToDateString() != DateTime.Now.ToString("yyyy-MM-dd"))
                        {
                            codeRuleSeed.F_SeedValue = initValue;
                        }
                    }
                    else if (dateFormatStr.Contains("mm"))
                    {
                        if (((DateTime)codeRuleSeed.F_ModifyDate).ToString("yyyy-MM") != DateTime.Now.ToString("yyyy-MM"))
                        {
                            codeRuleSeed.F_SeedValue = initValue;
                        }
                    }
                    else if (dateFormatStr.Contains("yy"))
                    {
                        if (((DateTime)codeRuleSeed.F_ModifyDate).ToString("yyyy") != DateTime.Now.ToString("yyyy"))
                        {
                            codeRuleSeed.F_SeedValue = initValue;
                        }
                    }
                    codeRuleSeed.F_ModifyDate = DateTime.Now;
                    value = (int)codeRuleSeed.F_SeedValue;

                    codeRuleSeed.F_SeedValue += stepNum;
                    
                    await db.Update(codeRuleSeed);
                }
                //var ruleIdTmp = ruleId + "1";
                //await db.Delete<CodeRuleSeedEntity>(t=>t.F_RuleSeedId == ruleIdTmp);
                db.Commit();
                //queue.TryDequeue(out string result);
                return value;
            }
            catch (Exception)
            {
                db.Rollback();
                //queue.TryDequeue(out string result);
                throw;
            }
        }
        #endregion
    }
}
