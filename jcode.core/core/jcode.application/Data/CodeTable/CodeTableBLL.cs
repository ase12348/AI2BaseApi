using jcode.autofac;
using jcode.orm;
using jcode.iapplication;
using jcode.iapplication.Data.CodeTable;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present null
    /// 创建人：施赛一
    /// 日 期： 2021-09-18 10:38:26
    /// 描 述： CodeTable
    /// </summary>
    public class CodeTableBLL : BLLBase, CodeTableIBLL,BLL
    {
        private readonly CodeColumnsIBLL _codeColumnsIBLL;
        private readonly DbHistoryIBLL _dbHistoryIBLL;
        private readonly CodeTableService codeTableService = new CodeTableService();
        private readonly DatabaseLinkIBLL _databaseLinkIBLL;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeColumnsIBLL"></param>
        /// <param name="dbHistoryIBLL"></param>
        public CodeTableBLL(CodeColumnsIBLL codeColumnsIBLL, DbHistoryIBLL dbHistoryIBLL, DatabaseLinkIBLL databaseLinkIBLL)
        {
            _codeColumnsIBLL = codeColumnsIBLL;
            _dbHistoryIBLL = dbHistoryIBLL;
            _databaseLinkIBLL = databaseLinkIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取CodeTable的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="dbCode">数据库编码</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeTableEntity>> GetPageList(Pagination pagination, string keyWord,string dbCode, string appId = null)
        {
            return codeTableService.GetPageList(pagination, keyWord, dbCode, appId);
        }
        
        /// <summary>
        /// 获取主表lr_db_codetable的实体
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <returns></returns>
        public Task<CodeTableEntity> GetEntity(string f_Id)
        {
            return codeTableService.GetEntity(f_Id);
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="tableNames">数据库表名</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeTableEntity>> GetEntitys(string dbCode, List<string> tableNames) {
            return codeTableService.GetEntitys(dbCode, tableNames);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        public async Task Delete(string f_Id)
        {
            await codeTableService.Delete(f_Id);
        }

        /// <summary>
        /// 批量删除登记：只删元数据，不动物理表
        /// </summary>
        /// <param name="ids">登记主键，逗号分隔</param>
        public async Task Deletes(string ids)
        {
            await codeTableService.Deletes(ids);
        }

        /// <summary>
        /// 保存(新增,更新)
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <param name="lr_db_codetableEntity">数据库表信息lr_db_codetable实体数据</param>
        /// <param name="lr_db_codecolumnsList">数据库表字段信息lr_db_codecolumns实体数据列表</param>
        public async Task<bool> SaveEntity(string f_Id, CodeTableEntity lr_db_codetableEntity, IEnumerable<CodeColumnsEntity> lr_db_codecolumnsList)
        {
            // 手工新增/编辑登记走的也是这一道门：宿主自有表不许进模型
            if (FrameworkTableGuard.IsFrameworkTable(lr_db_codetableEntity?.F_TableName))
            {
                throw new Exception($"宿主自有表【{lr_db_codetableEntity.F_TableName}】不能登记进数据表模型");
            }

            lr_db_codetableEntity.F_ClassName = Str.CsharpName(lr_db_codetableEntity.F_TableName);

            bool res = false;
            var entity = await codeTableService.GetEntity(f_Id, lr_db_codetableEntity);
            if (entity != null)
            {// 数据表类名或表名已存在
                return res;
            }

            if (!string.IsNullOrEmpty(f_Id))
            {
                var oldEntity =await GetEntity(f_Id);
                if (oldEntity.F_TableName != lr_db_codetableEntity.F_TableName)
                {
                    res = !codeTableService.BaseRepository(lr_db_codetableEntity.F_DbId).IsAnyTable(lr_db_codetableEntity.F_TableName);
                }
                else {
                    res = true;
                }
            }
            else {
                res = !codeTableService.BaseRepository(lr_db_codetableEntity.F_DbId).IsAnyTable(lr_db_codetableEntity.F_TableName);
            }

            if (!res) {
                return res;
            }



            codeTableService.BeginTrans();
            try
            {
                await codeTableService.SaveEntity(f_Id, lr_db_codetableEntity);

                var oldColumns = (List<CodeColumnsEntity>)await _codeColumnsIBLL.GetList(new CodeColumnsEntity { F_CodeTableId = lr_db_codetableEntity.F_Id });
                List<CodeColumnsEntity> updateList = new List<CodeColumnsEntity>();
                List<CodeColumnsEntity> addList = new List<CodeColumnsEntity>();
                int columnSort = 1;
                foreach (var dbColumnItem in lr_db_codecolumnsList) {
                    dbColumnItem.F_CodeTableId = lr_db_codetableEntity.F_Id;
                    dbColumnItem.F_Sort = columnSort;
                    columnSort++;
                    var oldColumn = oldColumns.Find(t => t.F_DbColumnName == dbColumnItem.F_DbColumnName);
                    if (oldColumn == null)
                    {
                        dbColumnItem.F_Id = Guid.NewGuid().ToString();
                        addList.Add(dbColumnItem);
                    }
                    else
                    {
                        dbColumnItem.F_Id = oldColumn.F_Id;
                        updateList.Add(dbColumnItem);
                    }
                }

                // 获取需要删除的列
                var deleteList = oldColumns.FindAll(t => updateList.FindIndex(t2 => t2.F_Id == t.F_Id) == -1);

                await _codeColumnsIBLL.Add(addList);
                await _codeColumnsIBLL.Update(updateList);
                await _codeColumnsIBLL.Deletes(deleteList);
                
                codeTableService.Commit();
                res = true;


            }
            catch (Exception)
            {
                codeTableService.Rollback();
                throw;
            }


            return res;
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 导入表信息
        /// </summary>
        /// <param name="dbcode">数据库编码</param>
        /// <param name="tableList">数据表信息</param>
        /// <param name="appId">所属应用主键：从应用工作台导入时自动归属该应用</param>
        /// <returns></returns>
        public async Task ImportTable(string dbcode, List<DbTableInfo> tableList, string appId = null) {

            // 宿主自有表由平台代码拥有：登记进模型后「更新到表」就会反向改写框架表结构，这里先拒收
            var names = (tableList ?? new List<DbTableInfo>()).Select(t => t.Name).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            var blocked = FrameworkTableGuard.PickFrameworkTables(names);
            if (blocked.Count > 0)
            {
                throw new Exception($"宿主自有表不能导入数据表模型：{string.Join("、", blocked)}");
            }
            var unsafeName = names.Where(t => FrameworkTableGuard.HasDdlUnsafeChar(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (unsafeName.Count > 0)
            {
                throw new Exception($"表名带不能拼进建表语句的字符，不能导入数据表模型：{string.Join("、", unsafeName)}");
            }

            codeTableService.BeginTrans();
            try
            {
                var oldTables = (List<CodeTableEntity>)await codeTableService.GetList(dbcode);

                List<CodeColumnsEntity> updateList = new List<CodeColumnsEntity>();
                List<CodeColumnsEntity> addList = new List<CodeColumnsEntity>();
                List<CodeColumnsEntity> deleteList = new List<CodeColumnsEntity>();

                foreach (var tableItem in tableList)
                {
                    CodeTableEntity entity = oldTables.Find(t => t.F_TableName == tableItem.Name);
                    var dbColumns = codeTableService.BaseRepository(dbcode).GetColumnInfosByTableName(tableItem.Name);

                    int columnSort = 1;

                    if (entity == null)
                    {
                        // 新增
                        entity = new CodeTableEntity();
                        entity.F_DbId = dbcode;
                        entity.F_ClassName = Str.CsharpName(tableItem.Name);
                        entity.F_TableName = tableItem.Name;
                        entity.F_Description = tableItem.Description;
                        entity.F_IsLock = 0;
                        entity.F_DeleteMark = 0;
                        entity.F_State = 1;
                        entity.F_AppId = appId;
                        await codeTableService.SaveEntity(null, entity);


                        // 新增列
                        foreach (var dbColumnItem in dbColumns) {
                            var codeColumnsEntity = CreateColumn(dbColumnItem);
                            codeColumnsEntity.F_CodeTableId = entity.F_Id;

                            codeColumnsEntity.F_Id = Guid.NewGuid().ToString();
                            codeColumnsEntity.F_Sort = columnSort;
                            columnSort++;

                            addList.Add(codeColumnsEntity);
                        }


                    }
                    else {
                        // 编辑
                        if (entity.F_Description != tableItem.Description 
                            || entity.F_State != 1
                            || entity.F_IsLock != 0
                            || entity.F_DeleteMark != 0
                            ) {
                            entity.F_State = 1;
                            entity.F_Description = tableItem.Description;
                            entity.F_IsLock = 0;
                            entity.F_DeleteMark = 0;
                            await codeTableService.SaveEntity(entity.F_Id, entity);
                        }

                        var oldColumns = (List<CodeColumnsEntity>)await _codeColumnsIBLL.GetList(new CodeColumnsEntity { F_CodeTableId = entity.F_Id });

                        foreach (var dbColumnItem in dbColumns) {
                            var codeColumnsEntity = CreateColumn(dbColumnItem);
                            codeColumnsEntity.F_CodeTableId = entity.F_Id;
                            codeColumnsEntity.F_Sort = columnSort;
                            columnSort++;

                            var oldColumn = oldColumns.Find(t => t.F_DbColumnName == dbColumnItem.DbColumnName);
                            if (oldColumn == null)
                            {
                                codeColumnsEntity.F_Id = Guid.NewGuid().ToString();
                                addList.Add(codeColumnsEntity);
                            }
                            else {
                                codeColumnsEntity.F_Id = oldColumn.F_Id;
                                updateList.Add(codeColumnsEntity);
                            }
                        }


                        // 获取需要删除的列
                        var deleteColumns = oldColumns.FindAll(t =>updateList.FindIndex(t2=>t2.F_Id == t.F_Id) == -1);
                        deleteList.AddRange(deleteColumns);
                    }
                }
                await _codeColumnsIBLL.Add(addList);
                await _codeColumnsIBLL.Update(updateList);
                await _codeColumnsIBLL.Deletes(deleteList);

                codeTableService.Commit();
            }
            catch (Exception)
            {
                codeTableService.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 创建更新数据库表
        /// </summary>
        /// <param name="tableid">数据表id</param>
        /// <param name="dbcode">数据库编码</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="dropFields">确认过要回收的列名，逗号分隔；不给就只加不减</param>
        /// <returns></returns>
        public async Task CreateOrUpdateTable(string tableid, string dbcode,string dbName, string dropFields = null)
        {
            var codeTableEntity = await codeTableService.GetEntity(tableid);//获取对应表的具体属性
            var codeColumnsList = await _codeColumnsIBLL.GetList(new CodeColumnsEntity { F_CodeTableId = tableid });
            await CreateOrUpdateTable(dbcode, dbName, codeTableEntity, codeColumnsList, dropFields);
        }

        /// <summary>
        /// 模型与物理表的列差异：操作者要先看到「会新建哪些列、可以回收哪些列」，确认后才发更新请求
        /// </summary>
        /// <param name="tableid">数据表id</param>
        /// <param name="dbcode">数据库编码</param>
        /// <returns></returns>
        public async Task<CodeTableColumnDiffDto> GetColumnDiff(string tableid, string dbcode)
        {
            var codeTableEntity = await codeTableService.GetEntity(tableid);
            if (codeTableEntity == null)
            {
                throw new Exception("这条数据表模型登记已经不存在，请刷新列表");
            }
            var codeColumnsList = (List<CodeColumnsEntity>)await _codeColumnsIBLL.GetList(new CodeColumnsEntity { F_CodeTableId = tableid });

            var res = new CodeTableColumnDiffDto
            {
                TableName = codeTableEntity.F_TableName,
                IsFrameworkTable = FrameworkTableGuard.IsFrameworkTable(codeTableEntity.F_TableName)
            };
            if (codeTableService.BaseRepository(dbcode).IsAnyTable(codeTableEntity.F_TableName))
            {
                res.TableExists = true;
            }
            var oldCols = res.TableExists
                ? codeTableService.BaseRepository(dbcode).GetColumnInfosByTableName(codeTableEntity.F_TableName)
                : new List<DbColumnInfo>();

            var modelNames = new HashSet<string>(codeColumnsList.Where(t => !string.IsNullOrWhiteSpace(t.F_DbColumnName)).Select(t => t.F_DbColumnName), StringComparer.OrdinalIgnoreCase);
            var phyNames = new HashSet<string>(oldCols.Where(t => !string.IsNullOrWhiteSpace(t.DbColumnName)).Select(t => t.DbColumnName), StringComparer.OrdinalIgnoreCase);

            foreach (var item in codeColumnsList.Where(t => !string.IsNullOrWhiteSpace(t.F_DbColumnName)))
            {
                if (!phyNames.Contains(item.F_DbColumnName))
                {
                    res.Add.Add(new CodeTableDiffColumnDto
                    {
                        Name = item.F_DbColumnName,
                        Description = item.F_Description,
                        DataType = item.F_DbType,
                        IsPrimaryKey = item.F_IsPrimaryKey == 1,
                        IsIdentity = item.F_IsIdentity == 1
                    });
                }
            }

            foreach (var item in oldCols.Where(t => !string.IsNullOrWhiteSpace(t.DbColumnName)))
            {
                if (!modelNames.Contains(item.DbColumnName))
                {
                    res.Drop.Add(new CodeTableDiffColumnDto
                    {
                        Name = item.DbColumnName,
                        Description = item.ColumnDescription,
                        DataType = item.DataType,
                        IsPrimaryKey = item.IsPrimarykey,
                        IsIdentity = item.IsIdentity,
                        IsProtected = item.IsPrimarykey || item.IsIdentity
                    });
                }
            }

            return res;
        }

        /// <summary>
        /// 数据表模型里已经不该存在的登记行：库里没有这张表，或它是宿主自有表
        /// </summary>
        /// <param name="dbcode">数据库编码</param>
        /// <returns></returns>
        public async Task<List<CodeTableInvalidDto>> GetInvalidTableList(string dbcode)
        {
            var list = (List<CodeTableEntity>)await codeTableService.GetList(dbcode);
            var res = new List<CodeTableInvalidDto>();
            if (list.Count == 0)
            {
                return res;
            }

            // 一次把库里的表枚举完，别拿登记行去做 N 次元数据查询
            HashSet<string> physical = null;
            try
            {
                physical = new HashSet<string>(codeTableService.BaseRepository(dbcode).GetTableInfoList().Select(t => t.Name), StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                // 库连接已经读不出来：这批登记没法核对物理表，只能标成 unknown 交回人工
            }

            foreach (var item in list)
            {
                var exists = physical != null && physical.Contains(item.F_TableName);
                var framework = FrameworkTableGuard.IsFrameworkTable(item.F_TableName);
                if (!framework && (exists || physical == null))
                {
                    continue;
                }
                res.Add(new CodeTableInvalidDto
                {
                    Id = item.F_Id,
                    TableName = item.F_TableName,
                    Description = item.F_Description,
                    PhysicalExists = exists,
                    Reason = framework ? "framework" : (physical == null ? "unknown" : "missing")
                });
            }

            return res;
        }

        /// <summary>
        /// 创建更新数据库表
        /// </summary>
        /// <param name="dbcode">数据库id</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="codeTableEntity">数据对象实体</param>
        /// <param name="codeColumnsList">数据对象属性列表</param>
        /// <param name="dropFields">确认过要回收的列名，逗号分隔；不给就只加不减</param>
        /// <returns></returns>
        public async Task CreateOrUpdateTable(string dbcode, string dbName, CodeTableEntity codeTableEntity, IEnumerable<CodeColumnsEntity> codeColumnsList, string dropFields = null) {

            if (FrameworkTableGuard.IsFrameworkTable(codeTableEntity.F_TableName))
            {
                throw new Exception($"宿主自有表【{codeTableEntity.F_TableName}】不能由数据表模型改结构，请清掉这条登记");
            }

            // 判断是更新表还是创建表
            // 获取旧的表字段
            List<DbColumnInfo> oldCols = null;
            if (codeTableService.BaseRepository(dbcode).IsAnyTable(codeTableEntity.F_TableName)) {
                oldCols = codeTableService.BaseRepository(dbcode).GetColumnInfosByTableName(codeTableEntity.F_TableName);
            }

            var dropList = ResolveDropColumns(codeTableEntity.F_TableName, oldCols, codeColumnsList, dropFields);

            var genentity = CreateEntitiesGen(codeTableEntity, codeColumnsList, oldCols);
            codeTableService.BaseRepository(dbcode).CreateOrUpdateTable(genentity);
            if (dropList.Count > 0)
            {
                codeTableService.BaseRepository(dbcode).DropColumns(genentity.TableName, dropList);
            }

            var newCols = codeTableService.BaseRepository(dbcode).GetColumnInfosByTableName(genentity.TableName);


            codeTableService.BeginTrans();
            try
            {
                // 更新状态
                codeTableEntity.F_State = 1;
                await codeTableService.SaveEntity(codeTableEntity.F_Id, codeTableEntity);
                /*记录当前更新表操作*/
                await _dbHistoryIBLL.SaveEntity(dbcode, dbName, genentity.TableName, oldCols, newCols);
                codeTableService.Commit();
            }
            catch (Exception)
            {
                codeTableService.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 核回收列清单：只认「库里真有、且模型里已经去掉」的列，认不出的名字一律停在这里不发 DDL。
        /// 交给 DDL 的列名一律换成库里读回来的原名，不接受操作者手写的形状。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="oldCols">物理表现有列，表不存在时为 null</param>
        /// <param name="codeColumnsList">模型登记的列</param>
        /// <param name="dropFields">操作者确认过的回收列名，逗号分隔</param>
        /// <returns></returns>
        private static List<string> ResolveDropColumns(string tableName, List<DbColumnInfo> oldCols, IEnumerable<CodeColumnsEntity> codeColumnsList, string dropFields)
        {
            var res = new List<string>();
            var asked = CodeTableService.SplitIds(dropFields);
            if (asked.Count == 0)
            {
                return res;
            }
            if (oldCols == null || oldCols.Count == 0)
            {
                throw new Exception($"表【{tableName}】在库里还不存在，没有列可以回收");
            }

            var modelNames = new HashSet<string>((codeColumnsList ?? Enumerable.Empty<CodeColumnsEntity>())
                .Where(t => !string.IsNullOrWhiteSpace(t.F_DbColumnName))
                .Select(t => t.F_DbColumnName), StringComparer.OrdinalIgnoreCase);

            foreach (var field in asked)
            {
                var col = oldCols.Find(t => string.Equals(t.DbColumnName, field, StringComparison.OrdinalIgnoreCase));
                if (col == null)
                {
                    throw new Exception($"库里没有列【{field}】：回收清单已经过期，请重新确认");
                }
                if (modelNames.Contains(col.DbColumnName))
                {
                    throw new Exception($"列【{field}】仍在数据表模型里：要先把这一列从模型里去掉，才能回收");
                }
                if (!res.Contains(col.DbColumnName))
                {
                    res.Add(col.DbColumnName);
                }
            }

            return res;
        }

        /// <summary>
        /// 导出数据库表数据
        /// </summary>
        /// <param name="dbcode"></param>
        /// <returns></returns>
        public async Task<string> ExportDatabase(string dbcode) {
            // 获取表数据
            var tableList = codeTableService.BaseRepository(dbcode).GetTableInfoList();
            // 导入到sqllite数据库
            var desName = CommonHelper.CreateNo();
            var sourcePath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/database/sqlite.db";
            var desPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/database/sqlite_{desName}.db";
            File.Copy(sourcePath, desPath);

            var db = codeTableService.BaseRepository("DataSource=" + desPath, "Sqlite").BeginTrans();

            try
            {
                List<CodeTableEntity> addTableList = new List<CodeTableEntity>();
                List<CodeColumnsEntity> addColList = new List<CodeColumnsEntity>();
                foreach (var tableItem in tableList)
                {
                    // 新增
                    var entity = new CodeTableEntity();
                    entity.F_DbId = dbcode;
                    entity.F_ClassName = Str.CsharpName(tableItem.Name);
                    entity.F_TableName = tableItem.Name;
                    entity.F_Description = tableItem.Description;
                    entity.F_IsLock = 0;
                    entity.F_DeleteMark = 0;
                    entity.F_State = 1;
                    entity.F_Id = Guid.NewGuid().ToString();
                    addTableList.Add(entity);

                    var dbColumns = codeTableService.BaseRepository(dbcode).GetColumnInfosByTableName(tableItem.Name);
                    int columnSort = 1;
                    // 新增列
                    foreach (var dbColumnItem in dbColumns)
                    {
                        var codeColumnsEntity = CreateColumn(dbColumnItem);
                        codeColumnsEntity.F_CodeTableId = entity.F_Id;

                        codeColumnsEntity.F_Id = Guid.NewGuid().ToString();
                        codeColumnsEntity.F_Sort = columnSort;
                        columnSort++;

                        addColList.Add(codeColumnsEntity);
                    }
                }

                await db.Inserts(addTableList);
                await db.Inserts(addColList);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }


            return desPath;

        }

        /// <summary>
        /// 迁移数据库表结构
        /// </summary>
        /// <param name="fromDb">原数据库</param>
        /// <param name="toDb">目标数据库</param>
        /// <param name="filterTable">过滤表（不要迁移）</param>
        /// <returns></returns>
        public async Task MoveDataBase(string fromDb,string toDb,string filterTable = "") {
            if (fromDb == toDb) {
                return;
            }

            if (!codeTableService.BaseRepository(fromDb, false).TestConnection())
            {
                throw new Exception($"链接地址异常{fromDb}");
            }
            if (!codeTableService.BaseRepository(toDb, false).TestConnection())
            {
                throw new Exception($"链接地址异常{toDb}");
            }

            var ver = CommonHelper.CreateNo();
            // 判断是否有lr_db_version表
            if (!codeTableService.BaseRepository(fromDb,false).IsAnyTable("lr_db_version")) {
                var table = new DbTableInfo();
                table.Name = "lr_db_version";
                table.Description = "用来保存当前数据库版本";

                var dbColumns = codeTableService.BaseRepository(false).GetColumnInfosByTableName(table.Name);
                var genentity = CreateEntitiesGen(table, dbColumns);
                codeTableService.BaseRepository(fromDb, false).CreateOrUpdateTable(genentity);

                
                var versionData = new CodeTableVersionEntity() { 
                    F_Id = "tanent",
                    F_Version_No = ver
                };
                await codeTableService.BaseRepository(fromDb, false).Insert(versionData);
            }
            else {
                var entity = await codeTableService.BaseRepository(fromDb, false).FindEntityByKey<CodeTableVersionEntity>("tanent");
                if (entity == null)
                {
                    var versionData = new CodeTableVersionEntity()
                    {
                        F_Id = "tanent",
                        F_Version_No = ver
                    };
                    await codeTableService.BaseRepository(fromDb, false).Insert(versionData);
                }
                else {
                    ver = entity.F_Version_No;
                }
            }

            var isMove = true;
            if (codeTableService.BaseRepository(toDb, false).IsAnyTable("lr_db_version"))
            {
                var newEntity = await codeTableService.BaseRepository(toDb, false).FindEntityByKey<CodeTableVersionEntity>("tanent");
                if (newEntity != null && newEntity.F_Version_No == ver) {
                    isMove = false;
                }
            }

            if (isMove) {
                var tableList = codeTableService.BaseRepository(fromDb, false).GetTableInfoList();
                foreach (var table in tableList)
                {
                    try
                    {
                        if (!filterTable.Contains(table.Name) && table.Name.ToLower().IndexOf("lr_base_log", StringComparison.Ordinal) == -1)
                        {
                            var dbColumns = codeTableService.BaseRepository(fromDb, false).GetColumnInfosByTableName(table.Name);
                            var genentity = CreateEntitiesGen(table, dbColumns);
                            codeTableService.BaseRepository(toDb, false).CreateOrUpdateTable(genentity);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                var sql = $"select * from lr_db_version";
                var data = await codeTableService.BaseRepository(fromDb, false).ORM.SqlQueryable<object>(sql).ToDictionaryListAsync();
                await codeTableService.BaseRepository(toDb, false).ExecuteSql("delete from lr_db_version ");
                codeTableService.BaseRepository(toDb, false).ORM.Insertable(data).AS("lr_db_version").ExecuteCommand();
            }
        }


        /// <summary>
        /// 获取类字段信息
        /// </summary>
        /// <param name="dbColumnInfo">数据库字段信息</param>
        /// <returns></returns>
        private CodeColumnsEntity CreateColumn(DbColumnInfo dbColumnInfo) {
            var codeColumnsEntity = new CodeColumnsEntity();
            //codeColumnsEntity.F_ClassProperName = Str.CsharpName(dbColumnInfo.DbColumnName);
            codeColumnsEntity.F_DbColumnName = dbColumnInfo.DbColumnName;
            codeColumnsEntity.F_IsNullable = dbColumnInfo.IsNullable == true ? 1 : 0;
            codeColumnsEntity.F_IsIdentity = dbColumnInfo.IsIdentity == true ? 1 : 0;
            codeColumnsEntity.F_IsPrimaryKey = dbColumnInfo.IsPrimarykey == true ? 1 : 0;
            codeColumnsEntity.F_Description = dbColumnInfo.ColumnDescription;

            codeColumnsEntity.F_CsType = dbColumnInfo.PropertyName;
            codeColumnsEntity.F_DbType = dbColumnInfo.DataType;
            codeColumnsEntity.F_Length = dbColumnInfo.Length;
            codeColumnsEntity.F_DecimalDigits = dbColumnInfo.DecimalDigits;
            

            return codeColumnsEntity;
        }

        /// <summary>
        /// 获取对应数据表以及数据表列的信息
        /// </summary>
        /// <param name="tableEntity">表信息</param>
        /// <param name="codeColumnsList">表字段</param>
        private EntitiesGen CreateEntitiesGen(CodeTableEntity tableEntity, IEnumerable<CodeColumnsEntity> codeColumnsList, List<DbColumnInfo> oldCols)
        {
            EntitiesGen gen = new EntitiesGen();
            //数据表信息
            gen.ClassName = tableEntity.F_ClassName;
            gen.Description = tableEntity.F_Description;
            gen.TableName = tableEntity.F_TableName;
            gen.PropertyGens = new List<PropertyGen>();
            //数据表对应列信息
            if (codeColumnsList != null)
            {
                foreach (var column in codeColumnsList)
                {
                    if (column.F_CsType != "ignore")
                    {
                        PropertyGen proGen = new PropertyGen()
                        {
                            DbColumnName = column.F_DbColumnName,
                            PropertyName = column.F_DbColumnName,
                            Description = column.F_Description,
                            IsIdentity = column.F_IsIdentity == 1 ? true : false,
                            IsPrimaryKey = column.F_IsPrimaryKey == 1 ? true : false,
                            IsNullable = column.F_IsPrimaryKey != 1 && column.F_IsNullable == 1 ? true : false,
                            Type = column.F_CsType,
                            DbType = column.F_DbType,
                            Length = column.F_Length,
                            DecimalDigits = column.F_DecimalDigits,
                            IsSpecialType = false,
                            IsIgnore = false
                        };

                        if (oldCols != null) {
                            var old = oldCols.Find(t => t.DbColumnName == column.F_DbColumnName);
                            if (old != null && old.PropertyName == column.F_CsType) {
                                proGen.IsNotUpdate = true;
                                proGen.DbType = old.DataType;
                            }
                            
                        }


                        gen.PropertyGens.Add(proGen);
                    }
                }
            }
            return gen;
        }



        /// <summary>
        /// 获取对应数据表以及数据表列的信息
        /// </summary>
        /// <param name="table">表信息</param>
        /// <param name="columnsList">表字段</param>
        private EntitiesGen CreateEntitiesGen(DbTableInfo table, IEnumerable<DbColumnInfo> columnsList)
        {
            EntitiesGen gen = new EntitiesGen();
            //数据表信息
            gen.ClassName = table.Name;
            gen.Description = table.Description;
            gen.TableName = table.Name;
            gen.PropertyGens = new List<PropertyGen>();
            //数据表对应列信息
            if (columnsList != null)
            {
                foreach (var column in columnsList)
                {
                    PropertyGen proGen = new PropertyGen()
                    {
                        DbColumnName = column.DbColumnName,
                        PropertyName = column.DbColumnName,
                        Description = column.ColumnDescription,
                        IsIdentity = column.IsIdentity,
                        IsPrimaryKey = column.IsPrimarykey,
                        IsNullable = !column.IsPrimarykey && column.IsNullable? true : false,
                        Type = column.PropertyName,
                        DbType = column.DataType,
                        Length = column.Length,
                        DecimalDigits = column.DecimalDigits,
                        IsSpecialType = false,
                        IsIgnore = false
                    };
                    gen.PropertyGens.Add(proGen);
                }
            }
            return gen;
        }


        #region 项目初始化的时候还原数据库
        /// <summary>
        /// 获取对应数据表以及数据表列的信息
        /// </summary>
        /// <param name="tableEntity">表信息</param>
        /// <param name="codeColumnsList">表字段</param>
        private EntitiesGen CreateEntitiesGenDb(CodeTableDbEntity tableEntity, IEnumerable<CodeColumnsDbEntity> codeColumnsList)
        {
            EntitiesGen gen = new EntitiesGen();
            //数据表信息
            gen.ClassName = tableEntity.F_ClassName;
            gen.Description = tableEntity.F_Description;
            gen.TableName = tableEntity.F_TableName;
            gen.PropertyGens = new List<PropertyGen>();
            //数据表对应列信息
            if (codeColumnsList != null)
            {
                foreach (var column in codeColumnsList)
                {
                    if (column.F_CsType != "ignore")
                    {
                        PropertyGen proGen = new PropertyGen()
                        {
                            DbColumnName = column.F_DbColumnName,
                            PropertyName = column.F_DbColumnName,
                            Description = column.F_Description,
                            IsIdentity = column.F_IsIdentity == 1 ? true : false,
                            IsPrimaryKey = column.F_IsPrimaryKey == 1 ? true : false,
                            IsNullable = column.F_IsPrimaryKey != 1 && column.F_IsNullable == 1 ? true : false,
                            Type = column.F_CsType,
                            DbType = column.F_DbType,
                            Length = column.F_Length,
                            DecimalDigits = column.F_DecimalDigits,
                            IsSpecialType = false,
                            IsIgnore = false
                        };
                        gen.PropertyGens.Add(proGen);
                    }
                }
            }
            return gen;
        }
        private Type GetCsType(string csType)
        {
            string _csType = csType;
            switch (csType)
            {
                case "int":
                    return  typeof(int?);
                case "long":
                    return  typeof(long?);
                case "short":
                    return  typeof(short?);
                case "bool":
                    _csType = "System.Boolean";
                    break;
                case "float":
                    _csType = "System.Single";
                    break;
                case "text":
                    _csType = "System.String";
                    break;
                default:
                    if (!string.IsNullOrEmpty(csType))
                    {
                        _csType = $"System.{Str.FirstUpper(csType)}";
                    }
                    break;
            }

            return System.Type.GetType(_csType);
        }
        public async Task ImportDatabaseBySqllite(string path, string code, bool isUpdateData = true)
        {
            Console.WriteLine("检测数据库更新状态");
            var db = codeTableService.BaseRepository($"DataSource={path}", "Sqlite",false).BeginTrans();
            try
            {
                var newVerison = await db.FindEntity<CodeTableVersionEntity>(t => t.F_Id == "1");
                // 1.判断目标库版本，是否需要更新
                var isVersionTable = codeTableService.BaseRepository(code).IsAnyTable("lr_db_version");
                bool needUpdate = true;
                if (isVersionTable)
                {
                    var version = await codeTableService.BaseRepository(code).FindEntity<CodeTableVersionEntity>(t => t.F_Id == "1");
                    if (version != null && newVerison.F_Version_No == version.F_Version_No)
                    {
                        needUpdate = false;
                        Console.WriteLine("当前已是最新版本，无需更新");
                    }
                    else if (!isUpdateData && version != null)
                    {
                        var newVerisonNo = newVerison.F_Version_No + "nodata";
                        if (newVerisonNo == version.F_Version_No)
                        {
                            needUpdate = false;
                            Console.WriteLine("当前已是最新版本，无需更新");
                        }
                    }
                }
                
                if (needUpdate)//
                {
                    Console.WriteLine("开始更新极简搭AI应用开发基座主体数据库");
                    var codeTableList = await db.FindAll<CodeTableDbEntity>();
                    foreach (var tableItem in codeTableList)
                    {
                        
                        Console.WriteLine($"更新表{tableItem.F_TableName}");
                        
                        /*if (tableItem.F_TableName.ToLower() != "lr_base_dataitem")
                        {
                            continue;
                        }*/

                        var hasTable = codeTableService.BaseRepository(code).IsAnyTable(tableItem.F_TableName);
                        
                        var codeColumnsList = (List<CodeColumnsDbEntity>)await db.FindList<CodeColumnsDbEntity>(t => t.F_CodeTableId == tableItem.F_Id, "F_Sort");
                        var genentity = CreateEntitiesGenDb(tableItem, codeColumnsList);
                        // 创建动态类
                        
                        var tableType = codeTableService.BaseRepository(code).GetTypeBilder(genentity.ToJson().ToObject<EntitiesGen>());
                        try
                        {
                            codeTableService.BaseRepository(code).CreateOrUpdateTable(genentity);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"更新表异常：{ex}！");
                        }
                        
                        
                        

                        try
                        {
                            // 更新表数据
                            if (isUpdateData && tableItem.F_TableName.ToLower() != "lr_base_area")
                            {
                                var dataList = (List<CodeTableDataEntity>)await db.FindList<CodeTableDataEntity>(t => t.F_TableName == tableItem.F_TableName, "F_Sort");
                                
                                if (dataList.Count > 0)
                                {
                                    string mainkey = dataList[0].F_MainKey;
                                    var addList = new List<object>();
                                    int addNum = 0;
                                    var toDb = codeTableService.BaseRepository(code).BeginTrans();
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(mainkey) || (!hasTable && tableItem.F_TableName.ToLower() == "lr_lg_map")  || (!hasTable && tableItem.F_TableName.ToLower() == "lr_base_authorize"))
                                        {
                                            Console.WriteLine($"更新表{tableItem.F_TableName}数据{dataList.Count}条");
                                            foreach (var data in dataList)
                                            {
                                           
                                                var entityDic = data.F_Data.ToObject<Dictionary<string, object>>();
                                                if (!string.IsNullOrEmpty(tableType.ByteColName) && !string.IsNullOrEmpty(entityDic[tableType.ByteColName].ToString()))
                                                {
                                                    //entityDic[tableType.ByteColName] = System.Text.Encoding.Default.GetBytes(entityDic[tableType.ByteColName].ToString());
                                                    entityDic[tableType.ByteColName] = Convert.FromBase64String(entityDic[tableType.ByteColName].ToString());
                                                }

                                                if (!string.IsNullOrEmpty(mainkey) && entityDic[mainkey].ToString().IsEmpty())
                                                {
                                                    continue;
                                                }
                                                
                                                
                                                var entityObj= toDb.ORM.DynamicBuilder().CreateObjectByType(tableType.ClassType,entityDic);
                                                addList.Add(entityObj);
                                                addNum += 1;
                                            }
                                        }
                  

                                   
                                    
                                        addNum = 0;
                                        if (string.IsNullOrEmpty(mainkey) && addList.Count > 0)
                                        {
                                            toDb.ORM.InsertableByObject(addList).ExecuteCommand();
                                        }
                                        else if (tableType.IsBigData)
                                        {
                                            toDb.ORM.Utilities.PageEach(addList, 1, (pageList) => {
                                                toDb.ORM.StorageableByObject(pageList[0]).AsInsertable.ExecuteCommand();
                                                addNum += pageList.Count;
                                                Console.WriteLine($"新增表{tableItem.F_TableName}数据完成{pageList.Count} | {addNum}/{addList.Count}！");
                                            });
                                        }
                                        else if (addList.Count > 1000)
                                        {
                                            
                                            toDb.ORM.Utilities.PageEach(addList, 1000, (pageList) => {
                                                toDb.ORM.StorageableByObject(pageList).ExecuteCommand();
                                                addNum += pageList.Count;
                                                Console.WriteLine($"新增表{tableItem.F_TableName}数据完成{pageList.Count} | {addNum}/{addList.Count}！");
                                            });
                                        }
                                        else if (addList.Count > 0)
                                        {
                                            toDb.ORM.StorageableByObject(addList).ExecuteCommand();
                                        }
                                        
                                        toDb.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        toDb.Rollback();
                                        Console.WriteLine($"更新表{tableItem.F_TableName}数据异常:{ex}！");

                                    }
                                }
                                Console.WriteLine($"更新表{tableItem.F_TableName}数据完成！");
                            }
                            else if (isUpdateData && !hasTable)
                            {
                                var dataList = (List<CodeTableDataEntity>)await db.FindList<CodeTableDataEntity>(t => t.F_TableName == tableItem.F_TableName, "F_Sort");
                                Console.WriteLine($"更新表{tableItem.F_TableName}数据{dataList.Count}条");
                                if (dataList.Count > 0)
                                {
                                    var addList = new List<Dictionary<string,object>>();
                                    int addNum = 0;
                                    foreach (var data in dataList)
                                    {
                                        var entityDic = data.F_Data.ToObject<Dictionary<string, object>>();
                                        
                                        
                                        foreach (var column in codeColumnsList)
                                        {
                                            if (column.F_CsType == "DateTime")
                                            {
                                                if (entityDic[column.F_DbColumnName] != null)
                                                {
                                                    entityDic[column.F_DbColumnName] = DateTime.Parse(entityDic[column.F_DbColumnName].ToString());
                                                }
                                                else
                                                {
                                                    entityDic[column.F_DbColumnName] = new DateTime();
                                                }
                                            }
                                        }
                                        addList.Add(entityDic);
                                    }

                                    var toDb = codeTableService.BaseRepository(code).BeginTrans();
                                    try
                                    {
                                        addNum = 0;
                                        if (addList.Count > 1000)
                                        {
                                            
                                            toDb.ORM.Utilities.PageEach(addList, 1000, (pageList) => {
                                                toDb.ORM.Insertable(pageList).AS(tableItem.F_TableName).ExecuteCommand();
                                                addNum += pageList.Count;
                                                Console.WriteLine($"新增表{tableItem.F_TableName}数据完成{pageList.Count} | {addNum}/{addList.Count}！");
                                            });
                                        }
                                        else if (addList.Count > 0)
                                        {
                                            toDb.ORM.Insertable(addList).AS(tableItem.F_TableName).ExecuteCommand();
                                        }
                                        
                                        toDb.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        toDb.Rollback();
                                        Console.WriteLine($"更新表{tableItem.F_TableName}数据异常:{ex}！");

                                    }
                                }
                                Console.WriteLine($"更新表{tableItem.F_TableName}数据完成！");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"更新表数据异常：{ex}！");
                        }
                        
                    }
                    
                    
                    Console.WriteLine($"开始写入最新版本！");
                    await codeTableService.BaseRepository(code).Delete<CodeTableVersionEntity>(t => t.F_Id == "1");
                    if (!isUpdateData)
                    {
                        newVerison.F_Version_No += "nodata";
                    }
                    await codeTableService.BaseRepository(code).Insert(newVerison);
                    Console.WriteLine($"写入最新版本！{newVerison.F_Version_No}");
                }

                db.Commit();
                Console.WriteLine($"已经成功，服务已启动！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"有数据异常{ex}！");
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 获取对应数据表以及数据表列的信息
        /// </summary>
        /// <param name="tableDbEntity">表信息</param>
        /// <param name="codeColumnsList">表字段</param>
        private EntitiesGen CreateEntitiesGen(CodeTableDbEntity tableDbEntity, IEnumerable<CodeColumnsDbEntity> codeColumnsList)
        {
            EntitiesGen gen = new EntitiesGen();
            //数据表信息
            gen.ClassName = tableDbEntity.F_ClassName;
            gen.Description = tableDbEntity.F_Description;
            gen.TableName = tableDbEntity.F_TableName;
            gen.PropertyGens = new List<PropertyGen>();
            //数据表对应列信息
            if (codeColumnsList != null)
            {
                foreach (var column in codeColumnsList)
                {
                    if (column.F_CsType != "ignore")
                    {
                        PropertyGen proGen = new PropertyGen()
                        {
                            DbColumnName = column.F_DbColumnName,
                            PropertyName = column.F_DbColumnName,
                            Description = column.F_Description,
                            IsIdentity = column.F_IsIdentity == 1 ? true : false,
                            IsPrimaryKey = column.F_IsPrimaryKey == 1 ? true : false,
                            IsNullable = column.F_IsPrimaryKey != 1 && column.F_IsNullable == 1 ? true : false,
                            Type = column.F_CsType,
                            DbType = column.F_DbType,
                            Length = column.F_Length,
                            DecimalDigits = column.F_DecimalDigits,
                            IsSpecialType = false,
                            IsIgnore = false
                        };
                        gen.PropertyGens.Add(proGen);
                    }
                }
            }
            return gen;
        }
        #endregion

        
        /// <summary>
        /// 更新数据表数据
        /// </summary>
        /// <param name="tableDataUpdateModel">配置数据</param>
        public async Task UpdateTable(TableDataUpdateModel tableDataUpdateModel)
        {
            var whereSql = await FormulaHelper.GetSql(tableDataUpdateModel.Conditions,tableDataUpdateModel.Formula);
            var updateData = new List<SugarParameter>();
            foreach (var item in tableDataUpdateModel.UpdateData)
            {
                switch (item.CsType)
                {
                    case "userId":
                        item.Value = GetUserId();
                        item.CsType = "";
                        break;
                    case "userAccount":
                        item.Value = GetUserAccount();
                        item.CsType = "";
                        break;
                    case "userName":
                        item.Value = GetUserName();
                        item.CsType = "";
                        break;
                    case "userCompanyId":
                        {
                            var companyId = await GetCompanyId();
                            item.Value = companyId;
                            item.CsType = "";
                        }
                        break;
                    case "userCompanyName":
                        {
                            var companyId = await GetCompanyId();
                            if (!string.IsNullOrEmpty(companyId))
                            {
                                var company = await this.GetCompany(companyId);
                                item.Value = company?.F_FullName;
                            }
                            item.CsType = "";
                        }
                        break;
                    case "userDeptId":
                        {
                            var deptId = await this.GetDepartmentId();
                            item.Value = deptId;
                            item.CsType = "";
                        }
                        break;
                    case "userDeptName":
                        {
                            var deptId = await this.GetDepartmentId();
                            if (!string.IsNullOrEmpty(deptId))
                            {
                                var dept = await this.GetDepartment(deptId);
                                item.Value = dept?.F_FullName;
                            }
                            item.CsType = "";
                        }
                        break;
                    case "nowDateTime":
                        item.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        item.CsType = "DateTime";
                        break;
                }
                
                updateData.Add(FormulaHelper.GetDbParameter(item.Name,item.Value,item.CsType));
            }
                
            await codeTableService.BaseRepository(tableDataUpdateModel.DbCode).ExecuteUpdateByWhere(tableDataUpdateModel.DbTable,updateData,whereSql);
        }

        #endregion
        
        

    }
}
