using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.cache;
using jcode.util;
using Microsoft.AspNetCore.Http;


namespace jcode.web.util;

/// <summary>
/// 版 本 极简搭AI应用开发基座
/// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
/// 创建人：tobin
/// 日 期：2020.09.27
/// 描 述：在线用户统计
/// </summary>
public class OnlineUserHelper
{
    private static readonly ConcurrentDictionary<string, OnlineUser> _onineUserDic =
        new ConcurrentDictionary<string, OnlineUser>();
    
    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void AddUser(string key,OnlineUser value)
    {
        try
        {
            // 防止因为ip地址错误导致的日志记录异常
            if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
            {
                var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                if (accessor.HttpContext != null)
                {
                    if (accessor.HttpContext.Request.Headers["X-Real-IP"].IsEmpty())
                    {
                        value.Ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        value.Ip = accessor.HttpContext.Request.Headers["X-Real-IP"].ToString();
                    }
                    
                    if (!string.IsNullOrEmpty(value.Ip))
                    {
                        value.Ip = value.Ip.Replace("::ffff:", "127.0.0.1");
                        value.Ip = value.Ip.Replace("::1", "127.0.0.1");
                    }

                  

                    if (!accessor.HttpContext.Request.Headers["sec-ch-ua"].IsEmpty())
                    {
                        value.Browser = accessor.HttpContext.Request.Headers["sec-ch-ua"].ToString();
                    }
                    else if (!accessor.HttpContext.Request.Headers["User-Agent"].IsEmpty())
                    {
                        var ua = accessor.HttpContext.Request.Headers["User-Agent"].ToString();
                        var uas = ua.Split(" ");
                        value.Browser = uas[uas.Length - 1];
                    }
                    
                    if (!accessor.HttpContext.Request.Headers["dc"].IsEmpty())
                    {
                        value.Device = accessor.HttpContext.Request.Headers["dc"].ToString();
                    }
                    
                    

                }
            }
        }
        catch (Exception)
        {
        }
        _onineUserDic.GetOrAdd(key, value);
    }
    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="key">键值</param>
    /// <returns></returns>
    public static OnlineUser GetUser(string key)
    {
        _onineUserDic.TryGetValue(key, out OnlineUser result);
        return result;
    }
    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="key">键值</param>
    /// <returns></returns>
    public static OnlineUser RemoveUser(string key)
    {
        _onineUserDic.TryRemove(key, out OnlineUser result);
        return result;
    }
    
    /// <summary>
    /// 获取所有keys
    /// </summary>
    /// <returns></returns>
    public static List<string> GetKeys()
    {
        return _onineUserDic.Keys.ToList();
    }


    /// <summary>
    /// 定时更新
    /// </summary>
    /// <returns></returns>
    public static void UpdateOnlineUserList()
    {
        var cacheKey = "jcode_onelineusers_cache";
        var cache  = IocManager.Instance.GetService<ICache>();
        cache.Lock("jcode_onelineuser_update_cache", _ =>
        {
            var list = cache.Read<List<OnlineUser>>(cacheKey);
            if (list == null)
            {
                list = new List<OnlineUser>();
            }

            // 更新online
            var keyList = GetKeys();
            foreach (var key in keyList)
            {
                var newUser = GetUser(key);
                var old = list.Find(t => t.LoginRandomCode == key);
                if (old == null)
                {
                    list.Add(newUser);
                }
                else
                {
                    if (newUser.Time > old.Time)
                    {
                        old.Time = newUser.Time;
                        old.Browser = newUser.Browser;
                        old.Ip = newUser.Ip;
                    }
                }

                RemoveUser(key);
            }
            
            // 删除超时的用户，大于10分钟没有消息
            var myTime = DateTime.Now.AddMinutes(-10);
            list = list.FindAll(t => t.Time >= myTime);


            cache.Write(cacheKey, list);


            return true;
        });
    }

    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    /// <returns></returns>
    public static List<OnlineUser> GetList( Pagination pagination,string keyWord)
    {
        var cacheKey = "jcode_onelineusers_cache";
        var cache  = IocManager.Instance.GetService<ICache>();
        var list = cache.Read<List<OnlineUser>>(cacheKey);
        if (list == null)
        {
            list = new List<OnlineUser>();
        }


        if (!string.IsNullOrEmpty(keyWord))
        {
            list = list.FindAll(t => t.Account.Contains(keyWord) || t.Name.Contains(keyWord));
        }

       
        list = list.OrderByDescending(x => x.Time).OrderByDescending(t => t.LoginTime).ToList();
        pagination.records = list.Count;
        return list.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
    }

}

public class OnlineUser
{
    /// <summary>
    /// 用户id
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// 登录随机码
    /// </summary>
    public string LoginRandomCode{ get; set; }
        

    /// <summary>
    /// 用户账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 最后访问时间
    /// </summary>
    public DateTime Time  { get; set; }
    
    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime  { get; set; }


    /// <summary>
    /// IP地址
    /// </summary>
    public string Ip { get; set; }
    
    /// <summary>
    /// 浏览器
    /// </summary>
    public string Browser { get; set; }
    
    /// <summary>
    /// 来源设备
    /// </summary>
    public string Device { get; set; }
}