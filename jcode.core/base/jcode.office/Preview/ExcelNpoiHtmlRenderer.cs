using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using jcode.util;

namespace jcode.office
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.10.26
    /// 描 述：用 NPOI 直接把 Excel（xls/xlsx）渲染成 HTML，避免依赖 LibreOffice
    /// </summary>
    public static class ExcelNpoiHtmlRenderer
    {
        private const int MaxSheets = 10;

        /// <summary>
        /// 渲染 Excel 为完整 HTML 文档
        /// </summary>
        /// <param name="filePath">Excel 文件路径</param>
        /// <param name="options">预览配置</param>
        /// <returns></returns>
        /// <exception cref="PreviewConvertException"></exception>
        public static string Render(string filePath, PreviewOptions options)
        {
            options = options ?? new PreviewOptions();
            int maxRows = Math.Max(1, options.MaxRows);
            int maxCols = Math.Max(1, options.MaxCols);

            try {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook workbook = IsXlsx(stream) ? new XSSFWorkbook(stream) : (IWorkbook)new HSSFWorkbook(stream);
                    var formatter = new DataFormatter();
                    IFormulaEvaluator evaluator = null;
                    try {
                        evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
                    } catch {
                        // 部分文件没有公式计算环境，退化为读取公式文本
                    }
                    try {
                        return RenderWorkbook(workbook, formatter, evaluator, maxRows, maxCols);
                    } finally {
                        workbook.Close();
                    }
                }
            } catch (PreviewConvertException) {
                throw;
            } catch (Exception ex) {
                throw new PreviewConvertException(PreviewErrorCode.NpoiFailed, "表格解析失败，请下载后查看", ex);
            }
        }

        private static bool IsXlsx(Stream stream)
        {
            long position = stream.Position;
            var header = new byte[4];
            int read = stream.Read(header, 0, header.Length);
            stream.Position = position;
            // xlsx 是 zip 包（PK\x03\x04），xls 是 OLE 复合文档
            return read == 4 && header[0] == 0x50 && header[1] == 0x4B;
        }

        private static string RenderWorkbook(IWorkbook workbook, DataFormatter formatter, IFormulaEvaluator evaluator, int maxRows, int maxCols)
        {
            var sheets = new List<(string Name, string Html)>();
            int limit = Math.Min(workbook.NumberOfSheets, MaxSheets);
            for (int i = 0; i < limit; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                if (sheet == null) {
                    continue;
                }
                var sheetHtml = new StringBuilder();
                RenderSheet(sheetHtml, workbook, sheet, formatter, evaluator, maxRows, maxCols);
                string name = string.IsNullOrWhiteSpace(sheet.SheetName) ? $"工作表{i + 1}" : sheet.SheetName;
                sheets.Add((name, sheetHtml.ToString()));
            }

            bool multi = sheets.Count > 1;
            var html = new StringBuilder();
            html.Append("<!DOCTYPE html><html><head><meta charset=\"utf-8\"/>");
            html.Append("<style>");
            html.Append("html,body{height:100%;}");
            html.Append("body{margin:0;font:13px/1.5 -apple-system,'PingFang SC','Microsoft YaHei',sans-serif;color:#1f2329;background:#fff;display:flex;flex-direction:column;}");
            html.Append(".jcode-notice{flex:none;margin:8px 12px;padding:6px 10px;background:#fff7e6;border:1px solid #ffe7ba;color:#ad6800;border-radius:4px;font-size:12px;}");
            html.Append(".jcode-wb{flex:1;min-height:0;display:flex;}");
            html.Append(".jcode-radio{position:absolute;left:-9999px;width:1px;height:1px;opacity:0;}");
            html.Append(".jcode-tabs{flex:none;width:168px;overflow:auto;background:#f7f8fa;border-right:1px solid #e5e6eb;padding:6px 0;}");
            html.Append(".jcode-tab{display:block;padding:8px 14px;color:#4e5969;cursor:pointer;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;border-left:2px solid transparent;}");
            html.Append(".jcode-tab:hover{background:#eef0f3;}");
            html.Append(".jcode-panes{flex:1;min-width:0;min-height:0;display:flex;flex-direction:column;}");
            html.Append(".jcode-pane{display:none;flex:1;min-height:0;overflow:auto;padding:12px;}");
            html.Append(".jcode-pane.is-only{display:block;}");
            html.Append(".jcode-sheet{overflow:visible;}");
            html.Append("table{border-collapse:collapse;table-layout:fixed;}");
            html.Append("td,th{border:1px solid #d9d9d9;padding:2px 6px;min-width:48px;max-width:320px;white-space:pre-wrap;word-break:break-all;vertical-align:top;}");
            html.Append("tr:first-child td{background:#fafafa;}");
            html.Append(".jcode-tip{margin-top:8px;padding:8px 12px;background:#fff7e6;border:1px solid #ffe7ba;color:#ad6800;border-radius:4px;font-size:12px;}");
            html.Append(".jcode-empty{padding:24px;text-align:center;color:#8f959e;}");
            if (multi)
            {
                for (int i = 0; i < sheets.Count; i++)
                {
                    html.Append("#jcode-sheet-").Append(i).Append(":checked~.jcode-tabs .jcode-tab-").Append(i)
                        .Append("{background:#fff;color:#3370ff;border-left-color:#3370ff;font-weight:600;}");
                    html.Append("#jcode-sheet-").Append(i).Append(":checked~.jcode-panes .jcode-pane-").Append(i)
                        .Append("{display:block;}");
                }
            }
            html.Append("</style></head><body>");

            if (workbook.NumberOfSheets > MaxSheets)
            {
                html.Append("<div class=\"jcode-notice\">该文件包含 ").Append(workbook.NumberOfSheets)
                    .Append(" 个工作表，仅显示前 ").Append(MaxSheets).Append(" 个，完整内容请下载查看。</div>");
            }
            if (sheets.Count == 0)
            {
                html.Append("<div class=\"jcode-empty\">该表格没有可显示的内容</div></body></html>");
                return html.ToString();
            }

            html.Append("<div class=\"jcode-wb\">");
            if (multi)
            {
                for (int i = 0; i < sheets.Count; i++)
                {
                    html.Append("<input class=\"jcode-radio\" type=\"radio\" name=\"jcode-sheet\" id=\"jcode-sheet-")
                        .Append(i).Append('"').Append(i == 0 ? " checked" : string.Empty).Append("/>");
                }
                html.Append("<div class=\"jcode-tabs\">");
                for (int i = 0; i < sheets.Count; i++)
                {
                    html.Append("<label class=\"jcode-tab jcode-tab-").Append(i).Append("\" for=\"jcode-sheet-").Append(i).Append("\">")
                        .Append(WebUtility.HtmlEncode(sheets[i].Name)).Append("</label>");
                }
                html.Append("</div>");
            }
            html.Append("<div class=\"jcode-panes\">");
            for (int i = 0; i < sheets.Count; i++)
            {
                html.Append("<div class=\"jcode-pane jcode-pane-").Append(i).Append(!multi && i == 0 ? " is-only" : string.Empty).Append("\">")
                    .Append(sheets[i].Html).Append("</div>");
            }
            html.Append("</div></div></body></html>");
            return html.ToString();
        }

        private static void RenderSheet(StringBuilder html, IWorkbook workbook, ISheet sheet, DataFormatter formatter,
            IFormulaEvaluator evaluator, int maxRows, int maxCols)
        {
            if (sheet.LastRowNum < 0)
            {
                html.Append("<div class=\"jcode-empty\">该工作表没有可显示的内容</div>");
                return;
            }

            int firstRow = Math.Max(0, sheet.FirstRowNum);
            int lastRow = Math.Min(sheet.LastRowNum, firstRow + maxRows - 1);
            bool rowTruncated = sheet.LastRowNum > lastRow;
            if (lastRow < firstRow)
            {
                html.Append("<div class=\"jcode-empty\">该工作表没有可显示的内容</div>");
                return;
            }

            int lastCol = -1;
            for (int r = firstRow; r <= lastRow; r++)
            {
                var row = sheet.GetRow(r);
                if (row != null) {
                    lastCol = Math.Max(lastCol, row.LastCellNum - 1);
                }
            }
            if (lastCol < 0)
            {
                html.Append("<div class=\"jcode-empty\">该工作表没有可显示的内容</div>");
                return;
            }
            int colCount = lastCol + 1;
            bool colTruncated = colCount > maxCols;
            if (colTruncated) {
                colCount = maxCols;
            }

            // 合并单元格：左上角记录跨度，其余被覆盖的单元格跳过
            var spanAt = new Dictionary<long, (int RowSpan, int ColSpan)>();
            var covered = new HashSet<long>();
            try {
                for (int i = 0; i < sheet.NumMergedRegions; i++)
                {
                    var region = sheet.GetMergedRegion(i);
                    if (region == null || region.FirstRow < firstRow || region.FirstRow > lastRow
                        || region.FirstColumn < 0 || region.FirstColumn >= colCount)
                    {
                        continue;
                    }
                    int rowSpan = Math.Min(region.LastRow, lastRow) - region.FirstRow + 1;
                    int colSpan = Math.Min(region.LastColumn, colCount - 1) - region.FirstColumn + 1;
                    if (rowSpan <= 0 || colSpan <= 0) {
                        continue;
                    }
                    spanAt[Key(region.FirstRow, region.FirstColumn)] = (rowSpan, colSpan);
                    for (int r = region.FirstRow; r < region.FirstRow + rowSpan; r++)
                    {
                        for (int c = region.FirstColumn; c < region.FirstColumn + colSpan; c++)
                        {
                            if (r != region.FirstRow || c != region.FirstColumn) {
                                covered.Add(Key(r, c));
                            }
                        }
                    }
                }
            } catch {
                // 合并区域信息异常时按普通表格渲染
                spanAt.Clear();
                covered.Clear();
            }

            html.Append("<div class=\"jcode-sheet\"><table>");
            for (int r = firstRow; r <= lastRow; r++)
            {
                html.Append("<tr>");
                var row = sheet.GetRow(r);
                for (int c = 0; c < colCount; c++)
                {
                    if (covered.Contains(Key(r, c))) {
                        continue;
                    }
                    ICell cell = row?.GetCell(c);
                    var attributes = GetCellAttributes(workbook, cell);
                    spanAt.TryGetValue(Key(r, c), out var span);
                    html.Append("<td").Append(attributes);
                    if (span.RowSpan > 1) { html.Append(" rowspan=\"").Append(span.RowSpan).Append("\""); }
                    if (span.ColSpan > 1) { html.Append(" colspan=\"").Append(span.ColSpan).Append("\""); }
                    html.Append(">");
                    string text = GetCellText(cell, formatter, evaluator);
                    if (!string.IsNullOrEmpty(text)) {
                        html.Append(WebUtility.HtmlEncode(text).Replace("\r\n", "\n").Replace("\n", "<br/>").Replace("\r", "<br/>"));
                    }
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }
            html.Append("</table></div>");

            if (rowTruncated || colTruncated)
            {
                html.Append("<div class=\"jcode-tip\">内容较多，仅显示前 ").Append(maxRows).Append(" 行、前 ")
                    .Append(maxCols).Append(" 列，完整内容请下载查看。</div>");
            }
        }

        private static string GetCellAttributes(IWorkbook workbook, ICell cell)
        {
            if (cell == null) {
                return string.Empty;
            }
            var style = new StringBuilder();
            try {
                var cellStyle = cell.CellStyle;
                if (cellStyle != null)
                {
                    var font = cellStyle.GetFont(workbook);
                    if (font != null && font.IsBold) {
                        style.Append("font-weight:600;");
                    }
                    switch (cellStyle.Alignment)
                    {
                        case HorizontalAlignment.Center:
                            style.Append("text-align:center;");
                            break;
                        case HorizontalAlignment.Right:
                            style.Append("text-align:right;");
                            break;
                    }
                }
            } catch {
                // 样式读取失败不影响内容展示
            }
            return style.Length == 0 ? string.Empty : " style=\"" + style + "\"";
        }

        private static string GetCellText(ICell cell, DataFormatter formatter, IFormulaEvaluator evaluator)
        {
            if (cell == null) {
                return string.Empty;
            }
            try {
                if (cell.CellType == CellType.Formula && evaluator != null) {
                    return formatter.FormatCellValue(cell, evaluator) ?? string.Empty;
                }
                return formatter.FormatCellValue(cell) ?? string.Empty;
            } catch {
                try {
                    return cell.ToString() ?? string.Empty;
                } catch {
                    return string.Empty;
                }
            }
        }

        private static long Key(int row, int col)
        {
            return ((long)row << 20) + col;
        }
    }
}
