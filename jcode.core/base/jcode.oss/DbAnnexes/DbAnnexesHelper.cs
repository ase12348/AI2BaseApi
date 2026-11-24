using System;
using System.Threading.Tasks;
using jcode.orm;
using jcode.util;

namespace jcode.oss
{
    public class DbAnnexesHelper:RepositoryFactory
    {
        #region 提交数据
        
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <param name="index">分片文件下标</param>
        public DbAnnexesEntity GetEntity(string fileId,int index)
        {
            return this.BaseRepository(false, true).ORM
                .Queryable<DbAnnexesEntity>()
                .Where(t=>t.F_FileId == fileId && t.F_Index == index)
                .First();
        }
        
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="path">文件地址</param>
        public DbAnnexesEntity GetEntity(string path)
        {
            return this.BaseRepository(false, true).ORM
                .Queryable<DbAnnexesEntity>()
                .Where(t=>t.F_Path == path)
                .First();
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <param name="index">分片文件下标</param>
        public void Delete(string fileId,int index)
        {
            this.BaseRepository(false, true).ORM
                .Deleteable<DbAnnexesEntity>(t=>t.F_FileId == fileId && t.F_Index == index).ExecuteCommand();
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="path">文件地址</param>
        public void Delete(string path)
        {
            this.BaseRepository(false, true).ORM
                .Deleteable<DbAnnexesEntity>(t=>t.F_Path == path).ExecuteCommand();
        }
        
        
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public void SaveEntity(DbAnnexesEntity entity)
        {
            entity.F_Id = Guid.NewGuid().ToString();
            entity.F_CreateDate = DateTime.Now;
            entity.F_CreateUserId =  ContextHelper.GetItem("userId") as string;;
            entity.F_CreateUserName = ContextHelper.GetItem("userId") as string;;
            this.BaseRepository(false, true).ORM.Insertable(entity).ExecuteCommand();
        }
        #endregion
    }
}