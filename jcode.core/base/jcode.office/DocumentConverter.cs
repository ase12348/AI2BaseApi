using ImageMagick;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using jcode.util;

namespace jcode.office
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.12.01
    /// 描 述：文档转化类 doc,docx,xls,xlsx,ppt,pptx,pdf,txt
    /// </summary>
    public class DocumentConverter
    {
        /// <summary>
        /// 获取LibreOffice程序的路径
        /// </summary>
        /// <returns></returns>
        private static string GetLibreOfficePath() {
            var configured = ConfigHelper.GetConfig().OfficeToolPath;
            if (!string.IsNullOrWhiteSpace(configured)) {
                return configured;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                return "/Applications/LibreOffice.app/Contents/MacOS/soffice";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return @"C:\Program Files\LibreOffice\program\soffice.exe";
            }
            // Linux 常见发行版/镜像路径
            string[] candidates = {
                "/opt/libreoffice24.2/program/soffice",
                "/opt/libreoffice/program/soffice",
                "/usr/bin/libreoffice",
                "/usr/bin/soffice"
            };
            foreach (var item in candidates) {
                if (File.Exists(item)) {
                    return item;
                }
            }
            return "/usr/bin/soffice";
        }

        /// <summary>
        /// LibreOffice 是否可用（未安装或未配置时，调用方应直接提示下载）
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            var options = GetPreviewOptions();
            if (!options.EnableLibreOffice) {
                return false;
            }
            try {
                var path = GetLibreOfficePath();
                return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
            } catch {
                return false;
            }
        }

        private static PreviewOptions GetPreviewOptions()
        {
            return ConfigHelper.GetConfig().Preview ?? new PreviewOptions();
        }

        /// <summary>
        /// 转化成文件转化成PDF
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="targetPath">目标文件目录</param>
        /// <exception cref="PreviewConvertException"></exception>
        public static void ToPdf(string filePath, string targetPath)
        {
            RunConvert(filePath, targetPath, "pdf", ".pdf");
        }

        /// <summary>
        /// 转化成文件转化成PDF
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="targetPath">目标文件目录</param>
        /// <exception cref="PreviewConvertException"></exception>
        public static void ToHtml(string filePath, string targetPath)
        {
            RunConvert(filePath, targetPath, "html", ".html");

            // 对图片进行转化
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string htmlFile = $"{targetPath}/{fileName}.html";
            if (!File.Exists(htmlFile)) {
                return;
            }
            string[] files = Directory.GetFiles(targetPath, fileName + "_html*");
            if (files.Length == 0) {
                return;
            }
            var content = File.ReadAllText(htmlFile);
            foreach (var fileItem in files)
            {
                var res = ConvertImageToBase64(fileItem);
                var imgName = Path.GetFileName(fileItem);
                content = content.Replace(imgName, $"data:image/png;base64,{res}");

                File.Delete(fileItem);// 删除文件
            }
            File.WriteAllText(htmlFile, content);
        }

        /// <summary>
        /// 调用 LibreOffice 无头模式转换文件
        /// </summary>
        /// <param name="filePath">源文件</param>
        /// <param name="outDir">输出目录</param>
        /// <param name="targetFormat">--convert-to 的参数，html/pdf</param>
        /// <param name="outputExtension">期望产物后缀，含点</param>
        private static void RunConvert(string filePath, string outDir, string targetFormat, string outputExtension)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
                throw new PreviewConvertException(PreviewErrorCode.FileMissing, "文件已丢失，无法预览");
            }
            var options = GetPreviewOptions();
            if (!options.EnableLibreOffice) {
                throw new PreviewConvertException(PreviewErrorCode.LibreOfficeMissing, "服务端未开启 LibreOffice 转换");
            }
            string libreOfficePath = GetLibreOfficePath();
            if (string.IsNullOrWhiteSpace(libreOfficePath) || !File.Exists(libreOfficePath)) {
                throw new PreviewConvertException(PreviewErrorCode.LibreOfficeMissing, "服务器未安装 LibreOffice，无法转换该格式");
            }

            if (!Directory.Exists(outDir)) {
                Directory.CreateDirectory(outDir);
            }

            // 每次转换使用独立 profile，避免并发调用争抢同一个用户配置锁
            string profileDir = Path.Combine(Path.GetTempPath(), "jcode_lo_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(profileDir);
            int timeoutSeconds = Math.Max(5, options.OfficeConvertTimeoutSeconds);
            try {
                var processStartInfo = new ProcessStartInfo(libreOfficePath);
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
                processStartInfo.ArgumentList.Add($"-env:UserInstallation={new Uri(profileDir).AbsoluteUri}");
                processStartInfo.ArgumentList.Add("--headless");
                processStartInfo.ArgumentList.Add("--nologo");
                processStartInfo.ArgumentList.Add("--norestore");
                processStartInfo.ArgumentList.Add("--convert-to");
                processStartInfo.ArgumentList.Add(targetFormat);
                processStartInfo.ArgumentList.Add("--outdir");
                processStartInfo.ArgumentList.Add(outDir);
                processStartInfo.ArgumentList.Add(filePath);

                string standardError;
                using (var process = new Process() { StartInfo = processStartInfo })
                {
                    process.Start();
                    // soffice 会拉起 soffice.bin，子进程可能继承输出管道，不能无限等待读取完成
                    var standardOutputTask = process.StandardOutput.ReadToEndAsync();
                    var standardErrorTask = process.StandardError.ReadToEndAsync();

                    if (!process.WaitForExit(timeoutSeconds * 1000))
                    {
                        KillProcessTree(process);
                        throw new PreviewConvertException(PreviewErrorCode.LibreOfficeTimeout,
                            $"文件转换超时（超过 {timeoutSeconds} 秒），请下载后查看");
                    }

                    standardError = WaitForText(standardErrorTask);
                    WaitForText(standardOutputTask);

                    if (process.ExitCode != 0)
                    {
                        throw new PreviewConvertException(PreviewErrorCode.LibreOfficeFailed,
                            $"文件转换失败（退出码 {process.ExitCode}）{(string.IsNullOrWhiteSpace(standardError) ? "" : "：" + TrimLog(standardError))}");
                    }
                }

                string output = Path.Combine(outDir, Path.GetFileNameWithoutExtension(filePath) + outputExtension);
                if (!File.Exists(output))
                {
                    throw new PreviewConvertException(PreviewErrorCode.LibreOfficeFailed,
                        $"文件转换失败，未生成预览文件{(string.IsNullOrWhiteSpace(standardError) ? "" : "：" + TrimLog(standardError))}");
                }
            } finally {
                try {
                    if (Directory.Exists(profileDir)) {
                        Directory.Delete(profileDir, true);
                    }
                } catch {
                    // 临时 profile 清理失败不影响预览结果
                }
            }
        }

        /// <summary>
        /// 结束进程及其子进程。jcode.office 以 netstandard2.1 编译，Process.Kill(bool) 只能在运行时通过反射取得
        /// </summary>
        private static void KillProcessTree(Process process)
        {
            try {
                var killTree = typeof(Process).GetMethod("Kill", new[] { typeof(bool) });
                if (killTree != null) {
                    killTree.Invoke(process, new object[] { true });
                    return;
                }
            } catch {
                // 进程已退出等情况直接忽略
            }
            try {
                process.Kill();
            } catch {
                // 进程已退出等情况直接忽略
            }
        }

        private static string WaitForText(Task<string> task)        {
            if (!task.Wait(2000)) {
                return string.Empty;
            }
            try {
                return task.Result ?? string.Empty;
            } catch {
                return string.Empty;
            }
        }

        private static string TrimLog(string text)
        {
            text = text.Trim();
            return text.Length > 300 ? text.Substring(0, 300) : text;
        }

        /// <summary>
        /// PDF转成图片
        /// </summary>
        /// <param name="fileName">图片文件名</param>
        /// <param name="filePath">pdf文件地址</param>
        /// <param name="outDir">输出目录</param>
        /// <returns></returns>
        public static async Task PdfToImage(string fileName, string filePath, string outDir)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            
            var settings = new MagickReadSettings();
            using (var images = new MagickImageCollection())
            {
                await images.ReadAsync(filePath, settings);
                int pageCount = images.Count;
                for (int i = 0; i < pageCount; i++)
                {
                    var image = images[i];
                    image.Format = MagickFormat.Png;
                    //image.Density = new Density(1000);
                    image.Quality = 100;
                    string imgPath = $"{fileName}_{i}.png"; //相对路径   
                    string filename = Path.Combine(outDir, imgPath);
                    await image.WriteAsync(filename);
                }
            }
        }
        
        
        public static string ConvertImageToBase64(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] imageBytes = new byte[fs.Length];
                fs.Read(imageBytes, 0, imageBytes.Length);
                fs.Close();

                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }

    /// <summary>
    /// 异常类
    /// </summary>
    public class LibreOfficeFailedException : Exception {
        public LibreOfficeFailedException(int exitCode) :base($"Office错误 {exitCode}") { }
    }
}
