using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace jcode.autofac
{
    /// <summary>
    /// 版 本 autofac 扩展方法
    /// Copyright (c) 2019-present 橙意工作室
    /// 创建人：cgg
    /// 日 期：2019.11.18
    /// 描 述： 依赖注入管理
    /// </summary>
    public class IocManager
    {
        private readonly Dictionary<string, ILifetimeScope> _lifetimeScopeDic;
        /// <summary>
        /// 容器
        /// </summary>
        public ILifetimeScope Container { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public IocManager()
        {
            _lifetimeScopeDic = new Dictionary<string, ILifetimeScope>();
        }
        
        /// <summary>获取子生命周期列表</summary>
        /// <returns></returns>
        public List<ILifetimeScope> AllLifetimeScope() => this._lifetimeScopeDic.Select<KeyValuePair<string, ILifetimeScope>, ILifetimeScope>((Func<KeyValuePair<string, ILifetimeScope>, ILifetimeScope>) (p => p.Value)).ToList<ILifetimeScope>();

        /// <summary>是否存在</summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public bool Any(string pluginName) => this._lifetimeScopeDic.ContainsKey(pluginName);

        /// <summary>移除</summary>
        /// <param name="pluginName"></param>
        public void Remove(string pluginName)
        {
            if (!this._lifetimeScopeDic.ContainsKey(pluginName))
                return;
            this._lifetimeScopeDic.Remove(pluginName);
        }

        /// <summary>获取</summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public ILifetimeScope Get(string pluginName) => this._lifetimeScopeDic.ContainsKey(pluginName) ? this._lifetimeScopeDic[pluginName] : (ILifetimeScope) null;

        /// <summary>添加</summary>
        /// <param name="pluginName"></param>
        /// <param name="context"></param>
        public void Add(string pluginName, ILifetimeScope context) => this._lifetimeScopeDic.Add(pluginName, context);

        
        /// <summary>获取接口实现方法</summary>
        /// <typeparam name="T">接口</typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
          if (this.Container.IsRegistered<T>())
            return this.Container.Resolve<T>();
          List<ILifetimeScope> lifetimeScopeList = this.AllLifetimeScope();
          ILifetimeScope context1 = (ILifetimeScope) null;
          foreach (ILifetimeScope context2 in lifetimeScopeList)
          {
            if (context2.IsRegistered<T>())
            {
              context1 = context2;
              break;
            }
          }
          return context1 != null ? context1.Resolve<T>() : throw new ArgumentNullException(nameof (T));
        }

        /// <summary>获取接口实现方法</summary>
        /// <typeparam name="T">接口</typeparam>
        /// <param name="serviceKey">对应名称</param>
        /// <returns></returns>
        public T GetService<T>(string serviceKey)
        {
          if (this.Container.IsRegisteredWithKey<T>((object) serviceKey))
            return this.Container.ResolveKeyed<T>((object) serviceKey);
          List<ILifetimeScope> lifetimeScopeList = this.AllLifetimeScope();
          ILifetimeScope context1 = (ILifetimeScope) null;
          foreach (ILifetimeScope context2 in lifetimeScopeList)
          {
            if (context2.IsRegisteredWithKey<T>((object) serviceKey))
            {
              context1 = context2;
              break;
            }
          }
          return context1 != null ? context1.ResolveKeyed<T>((object) serviceKey) : throw new ArgumentNullException(nameof (T));
        }

        /// <summary>获取接口实现方法</summary>
        /// <typeparam name="T">接口</typeparam>
        /// <param name="serviceKey">对应名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public T GetService<T>(string serviceKey, params Parameter[] parameters) => this.Container.ResolveKeyed<T>((object) serviceKey, parameters);

        /// <summary>获取接口实现方法</summary>
        /// <param name="serviceType">接口</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
          if (this.Container.IsRegistered(serviceType))
            return this.Container.Resolve(serviceType);
          List<ILifetimeScope> lifetimeScopeList = this.AllLifetimeScope();
          ILifetimeScope context1 = (ILifetimeScope) null;
          foreach (ILifetimeScope context2 in lifetimeScopeList)
          {
            if (context2.IsRegistered(serviceType))
            {
              context1 = context2;
              break;
            }
          }
          return context1 != null ? context1.Resolve(serviceType) : throw new ArgumentNullException(nameof (serviceType));
        }

        /// <summary>获取接口实现方法</summary>
        /// <param name="serviceKey">对应名称</param>
        /// <param name="serviceType">接口</param>
        /// <returns></returns>
        public object GetService(string serviceKey, Type serviceType)
        {
          if (this.Container.IsRegisteredWithKey((object) serviceKey, serviceType))
            return this.Container.ResolveKeyed((object) serviceKey, serviceType);
          List<ILifetimeScope> lifetimeScopeList = this.AllLifetimeScope();
          ILifetimeScope context1 = (ILifetimeScope) null;
          foreach (ILifetimeScope context2 in lifetimeScopeList)
          {
            if (context2.IsRegisteredWithKey((object) serviceKey, serviceType))
            {
              context1 = context2;
              break;
            }
          }
          if (context1 != null)
            return context1.ResolveKeyed((object) serviceKey, serviceType);
          throw new ArgumentNullException(nameof (serviceType));
        }

        /// <summary>是否实现</summary>
        /// <typeparam name="T">接口</typeparam>
        /// <returns></returns>
        public bool IsRegistered<T>()
        {
          if (this.Container.IsRegistered<T>())
            return true;
          foreach (IComponentContext context in this.AllLifetimeScope())
          {
            if (context.IsRegistered<T>())
              return true;
          }
          return false;
        }

        /// <summary>是否实现</summary>
        /// <typeparam name="T">接口</typeparam>
        /// <param name="serviceKey">对应名称</param>
        /// <returns></returns>
        public bool IsRegistered<T>(string serviceKey)
        {
          if (this.Container.IsRegisteredWithKey<T>((object) serviceKey))
            return true;
          foreach (IComponentContext context in this.AllLifetimeScope())
          {
            if (context.IsRegisteredWithKey<T>((object) serviceKey))
              return true;
          }
          return false;
        }

        /// <summary>是否实现</summary>
        /// <param name="serviceType">接口</param>
        /// <returns></returns>
        public bool IsRegistered(Type serviceType)
        {
          if (this.Container.IsRegistered(serviceType))
            return true;
          foreach (IComponentContext context in this.AllLifetimeScope())
          {
            if (context.IsRegistered(serviceType))
              return true;
          }
          return false;
        }

        /// <summary>是否实现</summary>
        /// <param name="serviceKey">对应名称</param>
        /// <param name="serviceType">接口</param>
        /// <returns></returns>
        public bool IsRegisteredWithKey(string serviceKey, Type serviceType)
        {
          if (this.Container.IsRegisteredWithKey((object) serviceKey, serviceType))
            return true;
          foreach (IComponentContext context in this.AllLifetimeScope())
          {
            if (context.IsRegisteredWithKey((object) serviceKey, serviceType))
              return true;
          }
          return false;
        }
        /// <summary>
        /// 获取一个静态实例
        /// </summary>
        public static IocManager Instance = new IocManager();
    }
}
