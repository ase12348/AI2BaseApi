using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using jcode.web.util.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace jcode.controllers.Data
{

    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：数据库
    /// </summary>
    public class DblinkController : BaseApiController
    {
        private readonly DatabaseLinkIBLL _databaseLinkIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="databaseLinkIBLL"></param>
        public DblinkController(DatabaseLinkIBLL databaseLinkIBLL)
        {
            _databaseLinkIBLL = databaseLinkIBLL;
        }

        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("data/dblinks")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DatabaseLinkEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _databaseLinkIBLL.GetList();
            return Success(data);
        }

        /// <summary>
        /// 新增数据库连接
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("data/dblink")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(DatabaseLinkEntity entity)
        {
            bool res = await _databaseLinkIBLL.SaveEntity(string.Empty, entity);
            if (res)
            {
                return Success("保存成功",entity.F_Id);
            }
            else
            {
                return Fail("保存失败,连接串信息有误");
            }
        }

        /// <summary>
        /// 更新数据库连接
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("data/dblink/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,DatabaseLinkEntity entity)
        {
            bool res = await _databaseLinkIBLL.SaveEntity(id, entity);
            if (res)
            {
                return SuccessInfo("保存成功");
            }
            else
            {
                return Fail("保存失败,连接串信息有误");
            }
        }

        /// <summary>
        /// 删除数据库连接
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/dblink/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _databaseLinkIBLL.Delete(id);
            return SuccessInfo("删除成功");
        }

        /// <summary>
        /// 测试连接串是否正确
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("data/dblink/test")]
        [ProducesResponseType(typeof(ResponseDtoBool), 200)]
        public IActionResult TestConnection(DatabaseLinkEntity entity)
        {
            return CheckConnection(entity);
        }

        /// <summary>
        /// 本机专用的免登录测试连接：桌面客户端要在「还没管理员账号」的时候就试得出这条库地址，
        /// 而宿主没有任何匿名接口能报「这条库连不连得上」，所以单开这一条窄路。
        /// 只放本机直连：来源不是回环、或带了代理写进来的 X-Real-IP / X-Forwarded-For，一律按拒绝处理。
        /// 不加这道判据，生产实例上这条口就等于一个不用认证的「拿任意主机和口令去试连」的口子。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("data/dblink/test-local")]
        [AllowAnonymous]
        [SkipApiMaintain]
        [ProducesResponseType(typeof(ResponseDtoBool), 200)]
        public IActionResult TestConnectionLocal(DatabaseLinkEntity entity)
        {
            if (!IsLocalRequest())
            {
                // 用 fail 不用 nologin：这条口本来就不要登录信息，回 401 会让客户端把它当成「登录过期」去刷票
                return Fail("这条接口只允许本机直连");
            }
            return CheckConnection(entity);
        }

        /// <summary>
        /// 测连接本体：带登录的 data/dblink/test 与只放本机的 data/dblink/test-local 都走这一处，不写第二份判据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IActionResult CheckConnection(DatabaseLinkEntity entity)
        {
            var res = _databaseLinkIBLL.TestConnection(
                entity.F_Type,
                entity.F_DbConnection, 
                entity.F_DbType, 
                entity.F_Server,
                entity.F_Port,
                entity.F_UserId,
                entity.F_Password,
                entity.F_DataBase);
            if (res == "success")
            {
                return Success("连接成功",true);
            }
            else
            {
                return Success(res,false);
            }
        }

        /// <summary>
        /// 是否本机直连：来源必须是回环地址，且请求里没有代理转发的来源头。
        /// 走反向代理时 RemoteIpAddress 是代理自己的地址（常常就是 127.0.0.1），只看它会把远端放行；
        /// 那两头只要出现，说明这条请求经过了转发，不当本机。
        /// </summary>
        /// <returns></returns>
        private bool IsLocalRequest()
        {
            var remote = HttpContext.Connection.RemoteIpAddress;
            if (remote == null || !IPAddress.IsLoopback(remote))
            {
                return false;
            }
            var headers = HttpContext.Request.Headers;
            if (headers.ContainsKey("X-Real-IP") || headers.ContainsKey("X-Forwarded-For"))
            {
                return false;
            }
            return true;
        }
    }
}
