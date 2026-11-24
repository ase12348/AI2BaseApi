using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.util;
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
    public class CodeRuleBLL :BLLBase, CodeRuleIBLL, BLL
    {
        #region 属性
        private readonly CodeRuleService codeRuleService = new CodeRuleService();

        // 组织单位
        private readonly CompanyIBLL _companyIBLL;
        private readonly DepartmentIBLL  _departmentIBLL;


        private readonly ICache _iCache;


        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyIBLL"></param>
        /// <param name="departmentIBLL"></param>
        /// <param name="iCache"></param>
        public CodeRuleBLL(CompanyIBLL companyIBLL, DepartmentIBLL departmentIBLL, ICache iCache)
        {
            _companyIBLL = companyIBLL;
            _departmentIBLL = departmentIBLL;
            _iCache = iCache;


        }


        #region 获取数据
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeRuleEntity>> GetPageList(Pagination pagination, string keyword, string appId = null)
        {
            return codeRuleService.GetPageList(pagination, keyword, appId);
        }
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CodeRuleEntity>> GetList()
        {
            return codeRuleService.GetList();
        } 
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<CodeRuleEntity> GetEntity(string keyValue)
        {
            return codeRuleService.GetEntity(keyValue);
        }
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="enCode">规则编码</param>
        /// <returns></returns>
        public Task<CodeRuleEntity> GetEntityByCode(string enCode)
        {
            return codeRuleService.GetEntityByCode(enCode);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await codeRuleService.Delete(keyValue);
        }
        /// <summary>
        /// 保存规则表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="codeRuleEntity">规则实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, CodeRuleEntity codeRuleEntity)
        {
            await codeRuleService.SaveEntity(keyValue, codeRuleEntity);
        }
        #endregion

        #region 单据编码处理
        /// <summary>
        /// 获得指定模块或者编号的单据号
        /// </summary>
        /// <param name="enCode">编码</param>
        /// <param name="formData">表单值</param>
        /// <returns>单据号</returns>
        public async Task<string> GetBillCode(string enCode,string formData)
        {
            string billCode = "";    //单据号
            string nextBillCode = "";//单据号

            CodeRuleEntity coderuleentity = await GetEntityByCode(enCode);
            if (coderuleentity != null)
            {
                List<CodeRuleFormatModel> codeRuleFormatList = coderuleentity.F_RuleFormatJson.ToList<CodeRuleFormatModel>();
                string dateFormatStr = "";
                foreach (CodeRuleFormatModel codeRuleFormatEntity in codeRuleFormatList)
                {
                    switch (codeRuleFormatEntity.itemType.ToString())
                    {
                        //自定义项
                        case "0":
                            billCode += codeRuleFormatEntity.formatStr;
                            nextBillCode += codeRuleFormatEntity.formatStr;
                            break;
                        //日期
                        case "1":
                            //日期字符串类型
                            dateFormatStr += codeRuleFormatEntity.formatStr.Replace("m", "M");
                            billCode += DateTime.Now.ToString(dateFormatStr);
                            nextBillCode += DateTime.Now.ToString(dateFormatStr);
                            break;
                        //流水号
                        case "2":
                            int seedStep = codeRuleFormatEntity.stepValue == null ? 1 : int.Parse(codeRuleFormatEntity.stepValue.ToString());//如果步长为空默认1
                            int initValue = codeRuleFormatEntity.initValue == null ? 1 : int.Parse(codeRuleFormatEntity.initValue.ToString());//如果初始值为空默认1

                            int nowSerious = await _iCache.Lock<int>(coderuleentity.F_RuleId, id => { return codeRuleService.GetSeedValue(coderuleentity.F_RuleId, initValue, seedStep, dateFormatStr);});
                            //int nowSerious =await codeRuleService.GetSeedValue(coderuleentity.F_RuleId, initValue, seedStep, dateFormatStr);


                            int nextSerious = nowSerious + seedStep;
                            // 最大种子已经过期
                            string seriousStr = new string('0', (int)(codeRuleFormatEntity.formatStr.Length - nowSerious.ToString().Length)) + nowSerious.ToString();
                            string NextSeriousStr = new string('0', (int)(codeRuleFormatEntity.formatStr.Length - nextSerious.ToString().Length)) + nextSerious.ToString();
                            billCode += seriousStr;
                            nextBillCode += NextSeriousStr;
                            break;
                        //部门
                        case "3":
                            var userInfo = await CurrentUser();
                            DepartmentEntity departmentEntity = await _departmentIBLL.GetEntity(userInfo.F_DepartmentId);
                            if (departmentEntity != null) {
                                if (codeRuleFormatEntity.formatStr == "code")
                                {
                                    billCode += departmentEntity.F_EnCode;
                                    nextBillCode = nextBillCode + departmentEntity.F_EnCode;
                                }
                                else
                                {
                                    billCode = billCode + departmentEntity.F_FullName;
                                    nextBillCode = nextBillCode + departmentEntity.F_FullName;

                                }
                            }
                            break;
                        //公司
                        case "4":
                            var userInfo2 = await CurrentUser();
                            CompanyEntity companyEntity = await _companyIBLL.GetEntity(userInfo2.F_CompanyId);
                            if (companyEntity != null) {
                                if (codeRuleFormatEntity.formatStr == "code")
                                {
                                    billCode += companyEntity.F_EnCode;
                                    nextBillCode += companyEntity.F_EnCode;
                                }
                                else
                                {
                                    billCode += companyEntity.F_FullName;
                                    nextBillCode += companyEntity.F_FullName;
                                }
                            }
                            break;
                        //用户
                        case "5":
                            var userInfo3 = await CurrentUser();
                            if (codeRuleFormatEntity.formatStr == "code")
                            {
                                billCode += userInfo3.F_Account;
                                nextBillCode += userInfo3.F_Account;
                            }
                            else
                            {
                                billCode += userInfo3.F_RealName;
                                nextBillCode += userInfo3.F_RealName;
                            }
                            break;
                        case "6":
                            billCode += formData;
                            nextBillCode += formData;
                            break;
                        default:
                            break;
                    }
                }
                coderuleentity.F_CurrentNumber = nextBillCode;
                await codeRuleService.SaveEntity(coderuleentity.F_RuleId, coderuleentity);
            }
            return billCode;

        }
        #endregion
    }
}
