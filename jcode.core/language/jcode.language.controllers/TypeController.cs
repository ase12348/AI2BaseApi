using jcode.ilanguage;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Language
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：多语言类型
    /// </summary>
    public class TypeController : BaseApiController
    {
        private readonly LGTypeIBLL _lGTypeIBLL;
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="lGTypeIBLL"></param>
        public TypeController(LGTypeIBLL lGTypeIBLL)
        {
            _lGTypeIBLL = lGTypeIBLL;
        }

        /// <summary>
        /// 获取语言类型数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("language/types")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LGTypeEntity>>), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> GetList()
        {
            var data = await _lGTypeIBLL.GetList();
            return Success(data);
        }

        /// <summary>
        /// 新增语言类型
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("language/type")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(LGTypeEntity entity)
        {
            await _lGTypeIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！",entity.F_Id);
        }


        /// <summary>
        /// 更新语言类型
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("language/type/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,LGTypeEntity entity)
        {
            await _lGTypeIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 删除语言类型
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("language/type/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _lGTypeIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
    }
}
