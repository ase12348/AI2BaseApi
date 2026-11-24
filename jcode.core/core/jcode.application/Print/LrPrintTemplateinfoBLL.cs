using System;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using jcode.oss;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-09 14:13:34
    /// 描 述： 打印设计-打印模板
    /// </summary>
    public class LrPrintTemplateinfoBLL : BLLBase, ILrPrintTemplateinfoBLL, BLL
    {
        private readonly LrPrintTemplateinfoService lrPrintTemplateinfoService = new LrPrintTemplateinfoService();
        private readonly ILrPrintTemplateBLL _iLrPrintTemplateBLL;
        private readonly IAnnexes _iAnnexes;
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrPrintTemplateBLL">打印模板信息【lr_print_templateinfo】接口</param>
        public LrPrintTemplateinfoBLL(ILrPrintTemplateBLL iLrPrintTemplateBLL, IAnnexes iAnnexes)
        {
            _iLrPrintTemplateBLL = iLrPrintTemplateBLL ?? throw new ArgumentNullException(nameof(iLrPrintTemplateBLL));
            _iAnnexes = iAnnexes;
        }
        
        #region 获取数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="isEnabled">是否获取正式表单</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintTemplateinfoEntity>> GetPageList(Pagination pagination, LrPrintTemplateinfoEntity queryParams, bool isEnabled = false, string tags = null)
        {
            return lrPrintTemplateinfoService.GetPageList(pagination, queryParams, isEnabled, tags);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrPrintTemplateinfoEntity> GetEntity(string keyValue)
        {
            return lrPrintTemplateinfoService.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取实体数据-根据模板编码
        /// </summary>
        /// <param name="code">模板编码</param>
        /// <returns></returns>
        public Task<LrPrintTemplateinfoEntity> GetEntityByCode(string code)
        {
            return lrPrintTemplateinfoService.GetEntityByCode(code);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrPrintTemplateinfoEntity entity)
        {

            await lrPrintTemplateinfoService.SaveEntity(keyValue, entity);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            await lrPrintTemplateinfoService.Delete(keyValue);
        }
        #endregion

        #region 扩展
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteAll(string keyValue)
        {
            lrPrintTemplateinfoService.BeginTrans();
            try
            {
                await Delete(keyValue);
                await _iLrPrintTemplateBLL.DeleteByInfo(keyValue);
                lrPrintTemplateinfoService.Commit();
            }
            catch (Exception)
            {
                lrPrintTemplateinfoService.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        public async Task SaveAll(string keyValue, PrintTemplateDto dto)
        {
            //判断是否编辑
            if (!string.IsNullOrEmpty(keyValue))
            {
                dto.LrPrintTemplateinfoEntity.F_Id = keyValue;
                //获取模板消息
                var templateOldEntity = await _iLrPrintTemplateBLL.GetEntity(dto.LrPrintTemplateinfoEntity.F_TemplateId);
                //新旧数据对比
                if (templateOldEntity != null
                    && templateOldEntity.F_Type == dto.LrPrintTemplateEntity.F_Type
                    && templateOldEntity.F_Component == dto.LrPrintTemplateEntity.F_Component)
                {
                    dto.LrPrintTemplateEntity = null;
                }
            }
            else
            {
                dto.LrPrintTemplateinfoEntity.F_Id = Guid.NewGuid().ToString();
            }


            lrPrintTemplateinfoService.BeginTrans();
            try
            {
                if (dto.LrPrintTemplateEntity != null)
                {
                    dto.LrPrintTemplateEntity.F_TemplateInfoId = dto.LrPrintTemplateinfoEntity.F_Id;
                    await _iLrPrintTemplateBLL.SaveEntity(string.Empty,dto.LrPrintTemplateEntity);
                    dto.LrPrintTemplateinfoEntity.F_TemplateId = dto.LrPrintTemplateEntity.F_Id;
                }

                await SaveEntity(keyValue, dto.LrPrintTemplateinfoEntity);
                
                lrPrintTemplateinfoService.Commit();
            }
            catch (Exception)
            {
                lrPrintTemplateinfoService.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 启用/停用模板
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="state">状态1启用0禁用</param>
        /// <returns></returns>
        public async Task UpdateState(string id, int state)
        {
            await lrPrintTemplateinfoService.UpdateState(id, state);
        }
        
        
        /// <summary>
        /// 导出打印模板数据
        /// </summary>
        /// <param name="keyValue">自定义表单主键</param>
        /// <param name="path">目录</param>
        /// <returns></returns>
        public async Task<string> Export(string keyValue, string path)
        {
            PrintTemplateDto data = new PrintTemplateDto();
            data.LrPrintTemplateinfoEntity = await GetEntity(keyValue);
            if (data.LrPrintTemplateinfoEntity != null)
            {
                data.LrPrintTemplateinfoEntity.F_Id = string.Empty;
                data.LrPrintTemplateEntity = await _iLrPrintTemplateBLL.GetEntity(data.LrPrintTemplateinfoEntity.F_TemplateId);
            }
          
            
            var fileName = Guid.NewGuid().ToString();
            // 写入一个临时文件
            _iAnnexes.UploadFile(fileName, $"{path}/{fileName}.json", ".json", Encoding.UTF8.GetBytes(data.ToJson()),0);
            return fileName;
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="jsonData"></param>
        public async Task<bool> Import(string jsonData)
        {
            string keyValue = string.Empty;
            PrintTemplateDto dto = jsonData.ToObject<PrintTemplateDto>();
            if (dto.LrPrintTemplateinfoEntity == null)
            {
                return false;
            }
            else
            {
                dto.LrPrintTemplateinfoEntity.F_Id = string.Empty;
                dto.LrPrintTemplateEntity.F_Id = string.Empty;
                var templateInfo = await GetEntityByCode(dto.LrPrintTemplateinfoEntity.F_Code);
                if (templateInfo!=null)
                {
                    keyValue = templateInfo.F_Id;
                    dto.LrPrintTemplateinfoEntity.F_TemplateId = templateInfo.F_TemplateId;
                }
                
                await SaveAll(keyValue,dto);
                return true;
            }

        }
        #endregion
        
    }
}
