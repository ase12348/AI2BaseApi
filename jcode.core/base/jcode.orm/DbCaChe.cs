using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using jcode.autofac;
using jcode.cache;
using jcode.util;

namespace jcode.orm
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：扩展数据库信息缓存
    /// </summary>
    public class DbCaChe
    {
        ///// <summary>
        ///// 扩展数据库缓存
        ///// </summary>
        private static readonly ConcurrentDictionary<string, DbModel> _dblist =
           new ConcurrentDictionary<string, DbModel>();
        
        private  static string clearId = "";
        
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetValue(string key, DbModel value)
        {
            key = GetKey(key);
            if (value.F_DESEncrypt == 1)
            {
                value.F_DbConnection = AESHelper.AesDecrypt(value.F_DbConnection, "jcode_db");
            }

            //IocManager.Instance.GetService<ICache>().Write(key,value);
            
            //key = GetKey(key);
            _dblist.GetOrAdd(key, value);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static DbModel GetValue(string key)
        {
            /*key = GetKey(key);
            return IocManager.Instance.GetService<ICache>().Read<DbModel>(key);*/
            
            key = GetKey(key);
            _dblist.TryGetValue(key, out DbModel result);
            return result;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static void RemoveValue(string key)
        {
            
            key = GetKey(key);
            _dblist.TryRemove(key, out DbModel result);
            
            
            IocManager.Instance.GetService<ICache>().Write("jcode_clear_db",Guid.NewGuid().ToString());
            
            //return result;
        }
        
        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public static void Clear()
        {
            var nowClearId = IocManager.Instance.GetService<ICache>().Read<string>("jcode_clear_db");
            if (clearId != nowClearId)
            {
                _dblist.Clear();
                clearId = nowClearId;
            }
        }


        /// <summary>
        /// 获取key值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetKey(string key)
        {
            var tenantNo = ContextHelper.GetItem("TenantNo") as string;
            if (string.IsNullOrEmpty(tenantNo))
            {
                return key;
            }
            return $"{tenantNo}_{key}";
        }
        
        
        



        /// <summary>
        /// 数据库操作对象缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<IRepositoryWarper>> _dbTrans =
           new ConcurrentDictionary<string, List<IRepositoryWarper>>();
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetTrans(string key, List<IRepositoryWarper> value)
        {
            _dbTrans.GetOrAdd(key, value);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static List<IRepositoryWarper> GetTrans(string key)
        {
            _dbTrans.TryGetValue(key, out List<IRepositoryWarper> result);
            return result;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static List<IRepositoryWarper> RemoveTrans(string key)
        {
            _dbTrans.TryRemove(key, out List<IRepositoryWarper> result);
            return result;
        }


        /// <summary>
        /// 数据库操作对象缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, int> _dbTransNum =
           new ConcurrentDictionary<string, int>();

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetTransNum(string key, int value)
        {
            _dbTransNum.GetOrAdd(key, value);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static int GetTransNum(string key)
        {
            _dbTransNum.TryGetValue(key, out int result);
            return result;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static int RemoveTransNum(string key)
        {
            _dbTransNum.TryRemove(key, out int result);
            return result;
        }
    }
}
