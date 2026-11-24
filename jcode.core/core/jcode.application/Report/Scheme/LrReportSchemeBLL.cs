using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期： 2022-05-05 10:40:00
    /// 描 述： 简洁报表
    /// </summary>
    public class LrReportSchemeBLL : BLLBase, ILrReportSchemeBLL,BLL
    {
        private readonly LrReportSchemeService lrReportSchemeService = new LrReportSchemeService();

        

        #region 获取数据
        /// <summary>
        /// 获取简洁报表的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrReportSchemeEntity>> GetList(LrReportSchemeEntity queryParams)
        {
            return lrReportSchemeService.GetList(queryParams);
        }

        /// <summary>
        /// 获取简洁报表的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrReportSchemeEntity>> GetPageList(Pagination pagination, LrReportSchemeEntity queryParams)
        {
            return lrReportSchemeService.GetPageList(pagination, queryParams);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrReportSchemeEntity> GetEntity(string keyValue)
        {
            return lrReportSchemeService.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public async Task<ReportSchemeData> GetData(string keyValue)
        {
            var data = await lrReportSchemeService.GetEntity(keyValue);
            var reportSchemeModel = data.F_ParamJson.ToObject<ReportSchemeModel>();
            var res = new ReportSchemeData();
            res.TempStyle = data.F_TempStyle;
            if (data.F_TempStyle == "1" || data.F_TempStyle == "3") {
                res.ListData =await lrReportSchemeService.BaseRepository(reportSchemeModel.F_DataSourceId).FindTable(reportSchemeModel.F_ListSqlString);
            }

            if (data.F_TempStyle == "2" || data.F_TempStyle == "3")
            {
                res.ChartType = data.F_TempType;
                res.ChartData = await lrReportSchemeService.BaseRepository(reportSchemeModel.F_DataSourceId).FindTable(reportSchemeModel.F_ChartSqlString);
            }

            res.Id = keyValue;

            return res;
        }



        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            await lrReportSchemeService.Delete(keyValue);
        }

        

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrReportSchemeEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.F_ParamJson))
            {
                entity.F_ParamJson = AESHelper.AesDecrypt(entity.F_ParamJson, "jcode");//"12345678901234567890123456789012"
            }
            await lrReportSchemeService.SaveEntity(keyValue, entity);
        }
        #endregion
    }
}
