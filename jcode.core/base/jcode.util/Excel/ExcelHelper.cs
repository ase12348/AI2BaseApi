using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OOXML.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：NPOI Excel DataTable操作类 application/ms-excel application/vnd-excel
    /// </summary>
    public class ExcelHelper
    {

        #region DataTable导出到Excel文件excelConfig中FileName设置为全路径
        /// <summary>
        /// DataTable导出到Excel文件 Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static void ExcelExport(DataTable dtSource, ExcelConfig excelConfig)
        {
            using (MemoryStream ms = ExportMemoryStream(dtSource, excelConfig))
            {
                using (FileStream fs = new FileStream(excelConfig.FileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region DataTable导出到Excel的MemoryStream
        /// <summary>
        /// DataTable导出到Excel的MemoryStream Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static MemoryStream ExportMemoryStream(DataTable dtSource, ExcelConfig excelConfig)
        {
            //int colint = 0;
            //for (int i = 0; i < dtSource.Columns.Count;)
            //{
            //    DataColumn column = dtSource.Columns[i];
            //    if (excelConfig.ColumnEntity[colint].Column != column.ColumnName)
            //    {
            //        dtSource.Columns.Remove(column.ColumnName);
            //    }
            //    else
            //    {
            //        i++;
            //        if (colint < excelConfig.ColumnEntity.Count - 1)
            //        {
            //            colint++;
            //        }
            //    }
            //}

            Dictionary<string, DataColumn> DataColumnMap = new Dictionary<string, DataColumn>();

            //DataTable dt = dtSource.Clone();
            //var columns = dt.Columns;
            foreach (DataColumn column in dtSource.Columns)
            {
                bool v = excelConfig.ColumnEntity.Exists(t => t.Column == column.ColumnName);
                if (v)
                {
                    DataColumnMap.Add(column.ColumnName, column);
                    //dtSource.Columns.Remove(column.ColumnName);
                }
                /*else {
                    DataColumnMap.Add(column.ColumnName, column);
                }*/
            }

            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

            #region 设置标题样式
            ICellStyle headStyle = workbook.CreateCellStyle();
            int[] arrColWidth = new int[excelConfig.ColumnEntity.Count];
            string[] arrColName = new string[excelConfig.ColumnEntity.Count];//列名
            ICellStyle[] arryColumStyle = new ICellStyle[excelConfig.ColumnEntity.Count];//样式表
            headStyle.Alignment = HorizontalAlignment.Center; // ------------------
            var font = (XSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = excelConfig.TitlePoint;
            if (excelConfig.ForeColor != new Color())
            {
                var xssfColor = new XSSFColor(new byte[] { excelConfig.ForeColor.R, excelConfig.ForeColor.G, excelConfig.ForeColor.B }, new DefaultIndexedColorMap());
                font.SetColor(xssfColor);
            }
            font.IsBold = true;
            headStyle.SetFont(font);
            #endregion

            #region 列头及样式
            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center; // ------------------
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = excelConfig.HeadPoint;
            cHeadStyle.SetFont(cfont);
            #endregion

            #region 设置内容单元格样式
            int colNum = 0;
            foreach (var columnentity in excelConfig.ColumnEntity)
            {
                //DataColumn item = DataColumnMap[column.Column];



                ICellStyle columnStyle = workbook.CreateCellStyle();
                columnStyle.Alignment = HorizontalAlignment.Center;
                //Encoding.GetEncoding(936)
                arrColWidth[colNum] = Encoding.UTF8.GetBytes(columnentity.Column).Length;
                arrColName[colNum] = columnentity.Column.ToString();
                arrColName[colNum] = columnentity.ExcelColumn;
                if (columnentity.Width != 0)
                {
                    arrColWidth[colNum] = columnentity.Width;
                }
                /*if (columnentity.Background != new Color())
                {
                    if (columnentity.Background != new Color())
                    {
                        columnStyle.FillPattern = FillPattern.SolidForeground;
                        columnStyle.FillForegroundColor = GetXLColour(workbook, columnentity.Background);
                    }
                }*/
                if (columnentity.Font != null || columnentity.Point != 0 || columnentity.ForeColor != new Color())
                {
                    var columnFont = (XSSFFont)workbook.CreateFont();
                    columnFont.FontHeightInPoints = 10;
                    if (columnentity.Font != null)
                    {
                        columnFont.FontName = columnentity.Font;
                    }
                    if (columnentity.Point != 0)
                    {
                        columnFont.FontHeightInPoints = columnentity.Point;
                    }
                    if (columnentity.ForeColor != new Color())
                    {
                        // columnFont.Color = GetXLColour(workbook, columnentity.ForeColor);
                        var xssfColor = new XSSFColor(new byte[] { columnentity.ForeColor.R, columnentity.ForeColor.G, columnentity.ForeColor.B }, new DefaultIndexedColorMap());
                        font.SetColor(xssfColor);
                    }
                    columnStyle.SetFont(font);
                }
                columnStyle.Alignment = getAlignment(columnentity.Alignment);
                arryColumStyle[colNum] = columnStyle;

                colNum++;
            }
            if (excelConfig.IsAllSizeColumn)
            {
                #region 根据列中最长列的长度取得列宽
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < excelConfig.ColumnEntity.Count; j++)
                    {
                        if (arrColWidth[j] != 0)
                        {
                            //Encoding.GetEncoding(936)
                            int intTemp = Encoding.UTF8.GetBytes(dtSource.Rows[i][excelConfig.ColumnEntity[j].Column].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                arrColWidth[j] = intTemp;
                            }
                        }

                    }
                }
                #endregion
            }
            #endregion

            int rowIndex = 0;

            #region 表头及样式
            if (excelConfig.Title != null)
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                if (excelConfig.TitleHeight != 0)
                {
                    headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                }
                headerRow.HeightInPoints = 25;
                headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                headerRow.GetCell(0).CellStyle = headStyle;
                if (excelConfig.ColumnEntity.Count > 1) {
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, excelConfig.ColumnEntity.Count - 1)); // ------------------
                }
            }
            #endregion

            #region 列头及样式
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                colNum = 0;
                #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                foreach (var column in excelConfig.ColumnEntity)
                {
                    headerRow.CreateCell(colNum).SetCellValue(arrColName[colNum]);
                    //列头添加样式
                    if (column.Background != new Color())
                    {
                        ICellStyle cellHeadStyle = workbook.CreateCellStyle();
                        cellHeadStyle.Alignment = HorizontalAlignment.Center;
                        IFont ccfont = workbook.CreateFont();
                        cfont.FontHeightInPoints = excelConfig.HeadPoint;
                        cellHeadStyle.SetFont(ccfont);
                        cellHeadStyle.FillPattern = FillPattern.SolidForeground;
                        // cellHeadStyle.FillForegroundColor = GetXLColour(workbook, column.Background);
                        headerRow.Cells[colNum].CellStyle = cellHeadStyle;
                    }
                    else
                    {
                        headerRow.GetCell(colNum).CellStyle = cHeadStyle;
                    }
                    //设置列宽
                    //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                    int colWidth = (arrColWidth[colNum] + 1) * 256;
                    if (colWidth < 255 * 256)
                    {
                        sheet.SetColumnWidth(colNum, colWidth < 3000 ? 3000 : colWidth);
                    }
                    else
                    {
                        sheet.SetColumnWidth(colNum, 6000);
                    }
                    colNum++;
                }
                #endregion
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            Dictionary<string, ICellStyle> dateStyleMap = new Dictionary<string, ICellStyle>();
            dateStyleMap.Add("yyyy-mm-dd", dateStyle);


          

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535)
                {
                    sheet = workbook.CreateSheet();
                    rowIndex = 0;
                    #region 表头及样式
                    {
                        if (excelConfig.Title != null)
                        {
                            IRow headerRow = sheet.CreateRow(rowIndex);
                            rowIndex++;
                            if (excelConfig.TitleHeight != 0)
                            {
                                headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                            }
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, excelConfig.ColumnEntity.Count - 1)); // ------------------
                        }

                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(rowIndex);
                        rowIndex++;
                        #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                        colNum = 0;
                        foreach (var column in excelConfig.ColumnEntity)
                        {
                            headerRow.CreateCell(colNum).SetCellValue(arrColName[colNum]);
                            headerRow.GetCell(colNum).CellStyle = cHeadStyle;
                            //设置列宽
                            sheet.SetColumnWidth(colNum, (arrColWidth[colNum] + 1) * 256);
                            colNum++;
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

               
                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                colNum = 0;
                foreach (var columnentity in excelConfig.ColumnEntity)
                {
                    ICell newCell = dataRow.CreateCell(colNum);
                    newCell.CellStyle = arryColumStyle[colNum];
                    string drValue = row[columnentity.Column].ToString();

                    //ColumnModel columnentity = excelConfig.ColumnEntity.Find(t => t.Column == column.ColumnName);
                    if (!string.IsNullOrEmpty(columnentity.DateFormat))
                    {
                        if (!dateStyleMap.ContainsKey(columnentity.DateFormat))
                        {
                            ICellStyle dateStyle2 = workbook.CreateCellStyle();
                            IDataFormat format2 = workbook.CreateDataFormat();
                            dateStyle2.DataFormat = format.GetFormat(columnentity.DateFormat);
                            dateStyleMap.Add(columnentity.DateFormat, dateStyle2);
                        }


                        SetCell(newCell, dateStyleMap[columnentity.DateFormat], DataColumnMap[columnentity.Column].DataType, drValue);
                    }
                    else {

                        SetCell(newCell, dateStyle, DataColumnMap[columnentity.Column].DataType, drValue);
                    }
                      


                    colNum++;
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                byte[] bytes = ms.ToArray();
                workbook.Close();
                return new MemoryStream(bytes);
            }
        }
        #endregion

        #region ListExcel导出(加载模板)
        /// <summary>
        /// List根据模板导出ExcelMemoryStream
        /// </summary>
        /// <param name="list"></param>
        /// <param name="templdateName"></param>
        public static MemoryStream ExportListByTempale(List<TemplateDataModel> list, string templdateName)
        {
            try
            {
                using (FileStream fileStream = new FileStream(templdateName, FileMode.Open, FileAccess.Read))
                {
                    if (templdateName.IndexOf(".xlsx") == -1)//2003
                    {
                        HSSFWorkbook hssfworkbook = new HSSFWorkbook(fileStream);
                        ISheet sheet = hssfworkbook.GetSheetAt(0);
                        SetPurchaseOrder(sheet, list);
                        sheet.ForceFormulaRecalculation = true;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            hssfworkbook.Write(ms);
                            byte[] bytes = ms.ToArray();
                            hssfworkbook.Close();
                            return new MemoryStream(bytes);
                        }
                    }
                    else//2007
                    {
                        XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
                        ISheet sheet = xssfworkbook.GetSheetAt(0);
                        SetPurchaseOrder(sheet, list);
                        sheet.ForceFormulaRecalculation = true;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            xssfworkbook.Write(ms);
                            byte[] bytes = ms.ToArray();
                            xssfworkbook.Close();
                            return new MemoryStream(bytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 赋值单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="list"></param>
        private static void SetPurchaseOrder(ISheet sheet, List<TemplateDataModel> list)
        {
            try
            {
                foreach (var item in list)
                {
                    IRow row = null;
                    ICell cell = null;
                    row = sheet.GetRow(item.row);
                    if (row == null)
                    {
                        row = sheet.CreateRow(item.row);
                    }
                    cell = row.GetCell(item.cell);
                    if (cell == null)
                    {
                        cell = row.CreateCell(item.cell);
                    }
                    cell.SetCellValue(item.value);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 设置表格内容
        private static void SetCell(ICell newCell, ICellStyle dateStyle, Type dataType, string drValue)
        {
            switch (dataType.ToString())
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型
                    System.DateTime dateV;
                    if (System.DateTime.TryParse(drValue, out dateV))
                    {
                        newCell.SetCellValue(dateV);
                    }
                    else
                    {
                        newCell.SetCellValue("");
                    }
                    newCell.CellStyle = dateStyle;//格式化显示
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }
        #endregion

        #region 从Excel导入
        // 通用方法：获取单元格值
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static object GetCellValue(ICell cell)
        {
            if (cell == null) return null;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue; // 日期类型
                    }
                    else
                    {
                        return cell.NumericCellValue; // 数字类型
                    }
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Formula:
                    // 对于公式单元格，需要获取其计算后的结果
                    switch (cell.CachedFormulaResultType)
                    {
                        case CellType.String:
                            return cell.StringCellValue;
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                return cell.DateCellValue;
                            }
                            else
                            {
                                return cell.NumericCellValue;
                            }
                        case CellType.Boolean:
                            return cell.BooleanCellValue;
                        case CellType.Error:
                            return cell.ErrorCellValue;
                        default:
                            return null;
                    }
                case CellType.Blank:
                    return null;
                case CellType.Error:
                    return cell.ErrorCellValue;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ExcelImport(string strFileName)
        {
            DataTable dt = new DataTable();

            ISheet sheet = null;
            try
            {
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
                    sheet = xssfworkbook.GetSheetAt(0);
                    
                
                    /*if (strFileName.IndexOf(".xlsx") == -1)//2003
                    {
                       
                    }
                    else//2007
                    {
                       
                    }*/
                }
            }
            catch (Exception)
            {
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                    sheet = hssfworkbook.GetSheetAt(0);
                }
            }
            



            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    var cell = row.GetCell(j);
                    var value = GetCellValue(cell);
                    if (value != null)
                    {
                        dataRow[j] = value.ToString();
                    }

                    /*if (row.GetCell(j) != null)
                    {
                        //如果是公式Cell 
                        //则仅读取其Cell单元格的显示值 而不是读取公式
                        if (row.GetCell(j).CellType == CellType.Formula)
                        {
                            row.GetCell(j).SetCellType(CellType.String);
                            dataRow[j] = row.GetCell(j).StringCellValue;
                        }
                        else
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }*/
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        /// <summary>
        /// 读取excel ,默认第一行为标头 
        /// </summary>
        /// <param name="fileStream">文件数据流</param>
        /// <param name="flieType">文件类型</param>
        /// <returns></returns>
        public static DataTable ExcelImport(Stream fileStream, string flieType)
        {
            DataTable dt = new DataTable();
            ISheet sheet = null;
            if (flieType == ".xls")
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(fileStream);
                sheet = hssfworkbook.GetSheetAt(0);
            }
            else
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
                sheet = xssfworkbook.GetSheetAt(0);
            }
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        //如果是公式Cell 
                        //则仅读取其Cell单元格的显示值 而不是读取公式
                        if (row.GetCell(j).CellType == CellType.Formula)
                        {
                            row.GetCell(j).SetCellType(CellType.String);
                            dataRow[j] = row.GetCell(j).StringCellValue;
                        }
                        else
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        #endregion

        #region RGB颜色转NPOI颜色
        private static short GetXLColour(HSSFWorkbook workbook, Color SystemColour)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    s = XlColour.Indexed;
                }

            }
            else
                s = XlColour.Indexed;
            return s;
        }
        #endregion

        #region 设置列的对齐方式
        /// <summary>
        /// 设置对齐方式
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private static HorizontalAlignment getAlignment(string style)
        {
            switch (style)
            {
                case "center":
                    return HorizontalAlignment.Center;
                case "left":
                    return HorizontalAlignment.Left;
                case "right":
                    return HorizontalAlignment.Right;
                case "fill":
                    return HorizontalAlignment.Fill;
                case "justify":
                    return HorizontalAlignment.Justify;
                case "centerselection":
                    return HorizontalAlignment.CenterSelection;
                case "distributed":
                    return HorizontalAlignment.Distributed;
            }
            return NPOI.SS.UserModel.HorizontalAlignment.General;
        }

        #endregion
    }
}
