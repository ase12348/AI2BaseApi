using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.OOXML.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using DataTable = System.Data.DataTable;

namespace jcode.office
{
    /// <summary>
    /// Excel新的帮助类
    /// </summary>
    public class ExcelNewHelper
    {
        /// <summary>
        /// 导出excel到文件
        /// </summary>
        /// <param name="sheets">工作表集合</param>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static void ExcelExport(List<ExcelSheet> sheets, string name)
        {
            using (MemoryStream ms = ExportMemoryStream(sheets, name))
            {
                using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        /// <summary>
        /// 导出excel到内存
        /// </summary>
        /// <param name="sheets">工作表集合</param>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static MemoryStream ExportMemoryStream(List<ExcelSheet> sheets, string name)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd HH:mm:ss");
            dateStyle.WrapText = true;
            
            foreach (var sheet in sheets)
            {
                Dictionary<int, IRow> rows = new Dictionary<int, IRow>();
                ISheet isheet = workbook.CreateSheet(sheet.Name);
                foreach (var area in sheet.Data)
                {
                    if (!rows.ContainsKey(area.Y))
                    {
                        rows.Add(area.Y, isheet.CreateRow(area.Y));
                    }
                    IRow row = rows[area.Y];
                    if (area.Type == 1)
                    {
                        // 行数据
                        SetRow(row, area.Row, area.X,isheet, workbook);
                    }
                    else
                    {
                        int rowIndex= 0;
                        // 表格数据
                        // 设置表头
                        if (area.Table.IsHeaderMerged)
                        {
                            rowIndex = SetRowByCell(rows,area.Table.Header,isheet,workbook) + 1;
                        }
                        else
                        {
                            SetRow(row, area.Table.Header, area.X,isheet, workbook);
                            rowIndex = area.Y + 1;
                        }



                        // 设置表数据
                        Dictionary<string, DataColumn> dataColumnMap = new Dictionary<string, DataColumn>();
                        foreach (DataColumn column in area.Table.Data.Columns)
                        {
                            dataColumnMap.Add(column.ColumnName, column);
                        }


                        Dictionary<int, int> widthDic = new Dictionary<int, int>();
                        foreach (DataRow tableRow in area.Table.Data.Rows)
                        {
                            if (!rows.ContainsKey(rowIndex))
                            {
                                rows.Add(rowIndex, isheet.CreateRow(rowIndex));
                            }
                            row = rows[rowIndex];
                            SetTableRow(row, tableRow, dataColumnMap, dateStyle, area.Table.Header, area.X, workbook,widthDic);
                            rowIndex++;
                        }

           
                        foreach (int key in widthDic.Keys)
                        {
                            SetColumnWidth(widthDic[key], key, isheet);
                        }

                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                byte[] bytes = ms.ToArray();
                workbook.Close();
                return new MemoryStream(bytes);
            }
        }

        /// <summary>
        /// 设置单元格
        /// </summary>
        /// <param name="rows">excel行</param>
        /// <param name="cells">单元格信息</param>
        /// <param name="isheet">页</param>
        /// <param name="workbook">excel文档</param>
        private static int SetRowByCell(Dictionary<int, IRow> rows, List<ExcelCell> cells, ISheet isheet, XSSFWorkbook workbook)
        {
            int maxRow = 0;
            foreach (var item in cells)
            {
                if (!rows.ContainsKey(item.Y))
                {
                    rows.Add(item.Y, isheet.CreateRow(item.Y));
                }
                IRow row = rows[item.Y];
                ICell cell = row.GetCell(item.X);
                if (cell == null)
                {
                    cell = row.CreateCell(item.X);
                }
                cell.SetCellValue(item.Text);
                cell.CellStyle = GetCellStyle(item, workbook);

                SetColumnWidth(item.Width,item.X,isheet);
               

                if (item.ColSpan > 1 || item.RowSpan > 1)
                {
                    isheet.AddMergedRegion(new CellRangeAddress(item.Y, item.Y + item.RowSpan - 1, item.X,
                        item.X + item.ColSpan - 1));
                }

                if (item.Y > maxRow)
                {
                    maxRow = item.Y;
                }
            }

            return maxRow;
        }
        
        
        /// <summary>
        /// 设置行
        /// </summary>
        /// <param name="row">excel行</param>
        /// <param name="cells">单元格信息</param>
        /// <param name="colIndex">开始列</param>
        /// <param name="workbook">excel文档</param>
        private static void SetRow(IRow row, List<ExcelCell> cells, int colIndex,ISheet isheet, XSSFWorkbook workbook)
        {
            foreach (var item in cells)
            {
                ICell cell = row.GetCell(colIndex);
                if (cell == null)
                {
                    cell = row.CreateCell(colIndex);
                }
                cell.SetCellValue(item.Text);
                cell.CellStyle = GetCellStyle(item, workbook);
                
                if (item.ColSpan > 1)
                {
                    var lastCol = colIndex + item.ColSpan - 1;
                    
                    
                    if (lastCol < colIndex)
                    {
                        lastCol = colIndex;
                    }

                    isheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex, lastCol));
                }
                
                SetColumnWidth(item.Width,colIndex,isheet);
                SetColumnHeight(item.Height,row.RowNum,isheet);
                colIndex++;
                
                
            }
        }

        /// <summary>
        /// 设置行
        /// </summary>
        /// <param name="row">excel行</param>
        /// <param name="cells">单元格信息</param>
        /// <param name="colIndex">开始列</param>
        /// <param name="workbook">excel文档</param>
        private static void SetTableRow(IRow row, DataRow tableRow, Dictionary<string, DataColumn> dataColumnMap, ICellStyle dateStyle, List<ExcelCell> cells, int colIndex, XSSFWorkbook workbook,Dictionary<int, int> widthDic)
        {

            foreach (var item in cells)
            {
                if (dataColumnMap.ContainsKey(item.Prop))
                {
                    ICell cell = row.GetCell(colIndex);
                    if (cell == null)
                    {
                        cell = row.CreateCell(colIndex);
                    }
                    //cell.CellStyle = GetCellStyle(item, workbook);
                    SetCell(cell, dateStyle, dataColumnMap[item.Prop].DataType, tableRow[item.Prop].ToString());
                    int length = Encoding.Default.GetBytes(cell.StringCellValue).Length * 256 + 300;
                    length = length > 15000 ? 15000 : length;

                    if (widthDic.ContainsKey(colIndex))
                    {
                        // 若比已存在列宽更宽则替换，Excel限制最大宽度为15000
                        if (widthDic[colIndex] < length)
                        {
                            widthDic[colIndex] = length;
                        }
                    }
                    else
                    {
                        widthDic.Add(colIndex,length);
                    }



                    //cell.SetCellValue(tableRow[item.Prop].ToString());
                    
                    
                    
                    colIndex++;
                }
            }
        }

        /// <summary>
        /// 获取单元格样式
        /// </summary>
        /// <param name="cell">单元格配置</param>
        /// <param name="workbook">excel文档</param>
        /// <returns></returns>
        private static ICellStyle GetCellStyle(ExcelCell cell, XSSFWorkbook workbook)
        {
            if (cell.FontSize == 0)
            {
                cell.FontSize = 10;
            }
            ICellStyle cellStyle = workbook.CreateCellStyle();
            var font = (XSSFFont)workbook.CreateFont();
            font.IsBold = cell.IsBold;
            font.FontHeightInPoints = cell.FontSize;

            var fontRgb = Color.Black;
            var xssfColor = new XSSFColor(new byte[] { fontRgb.R, fontRgb.G, fontRgb.B }, new DefaultIndexedColorMap());
            font.SetColor(xssfColor);

            switch (cell.Align)
            {
                case  "left":
                    cellStyle.Alignment = HorizontalAlignment.Left;
                    break;
                case  "center":
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    break;
                case  "right":
                    cellStyle.Alignment = HorizontalAlignment.Right;
                    break;
            }
            
            
            cellStyle.SetFont(font);
            return cellStyle;
        }

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


        private static void SetColumnWidth(int width,int colIndex,ISheet isheet)
        {
            if (width > 0)
            {
                int colWidth = width;
                var oldColWidth  = isheet.GetColumnWidth(colIndex);
                if (width > oldColWidth)
                {
                    isheet.SetColumnWidth(colIndex, colWidth);
                }
            }
        }
        private static void SetColumnHeight(int height,int rowIndex,ISheet isheet)
        {
            if (height > 0)
            {
                // int rowHeight = height;
                var oldColHeight  = isheet.GetRow(rowIndex).Height;
                if (height > oldColHeight)
                {
                    isheet.GetRow(rowIndex).Height = (short)height;
                }
            }
        }


        #region 导入
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static List<ExcelSheet> ExcelImport(string strFileName, List<ExcelArea> areas)
        {
            List<ExcelSheet> sheets = new List<ExcelSheet>();

            DataTable dt = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (strFileName.IndexOf(".xlsx") == -1)//2003
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                    for (int i = 0; i < hssfworkbook.Count; i++)
                    {
                        ISheet sheet = hssfworkbook.GetSheetAt(i);
                        sheets.Add(SetSheet(sheet, areas));
                    }

                }
                else//2007
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
                    for (int i = 0; i < xssfworkbook.Count; i++)
                    {
                        ISheet sheet = xssfworkbook.GetSheetAt(0);
                        sheets.Add(SetSheet(sheet, areas));
                    }

                }
            }
            return sheets;
        }


        private static ExcelSheet SetSheet(ISheet sheet, List<ExcelArea> areas)
        {
            ExcelSheet excelSheet = new ExcelSheet();
            excelSheet.Data = new List<ExcelArea>();
            foreach (var area in areas)
            {
                ExcelArea newExcelArea = new ExcelArea();
                newExcelArea.Type = area.Type;

                // 行数据
                IRow row = sheet.GetRow(area.Y);
                if (row != null)
                {
                    if (area.Type == 1)
                    {
                        newExcelArea.Row = GetRowCell(row, area.Row, area.X);
                    }
                    else
                    {
                        newExcelArea.Table = new ExcelTable();
                        newExcelArea.Table.Header = area.Table.Header;
                        newExcelArea.Table.Data = new DataTable();
                        foreach (var cell in newExcelArea.Table.Header)
                        {
                            newExcelArea.Table.Data.Columns.Add(cell.Prop);
                        }
                        for (int i = (area.Y + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow sheetRow = sheet.GetRow(i);
                            DataRow dataRow = newExcelArea.Table.Data.NewRow();
                            int colindex = 0;
                            foreach (var cell in newExcelArea.Table.Header)
                            {
                                ICell _cell = sheetRow.GetCell(colindex);
                                if (_cell != null)
                                {
                                    //如果是公式Cell 
                                    //则仅读取其Cell单元格的显示值 而不是读取公式
                                    if (_cell.CellType == CellType.Formula)
                                    {
                                        _cell.SetCellType(CellType.String);
                                        dataRow[cell.Prop] = _cell.StringCellValue;
                                    }
                                    else
                                    {
                                        dataRow[cell.Prop] = _cell.ToString();
                                    }
                                }
                                colindex++;
                            }
                            newExcelArea.Table.Data.Rows.Add(dataRow);

                        }
                    }
                }

                excelSheet.Name = sheet.SheetName;
                excelSheet.Data.Add(newExcelArea);

            }

            return excelSheet;
        }

        private static List<ExcelCell> GetRowCell(IRow row, List<ExcelCell> cells, int colIndex)
        {
            var list = new List<ExcelCell>();
            foreach (var cell in cells)
            {
                ExcelCell newCell = new ExcelCell();
                var _cell = row.GetCell(colIndex);
                if (_cell != null)
                {
                    if (_cell.CellType == CellType.Formula)
                    {
                        _cell.SetCellType(CellType.String);
                        newCell.Text = _cell.StringCellValue;
                    }
                    else
                    {
                        newCell.Text = _cell.ToString();
                    }
                }
                colIndex++;
                list.Add(newCell);
            }
            return list;
        }
        #endregion
    }
}