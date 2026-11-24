using AlibabaCloud.SDK.Ocr_api20210707;
using Aliyun.OSS;
using System.Collections.Generic;
using System;
using Tea;
using TencentCloud.Cam.V20190116.Models;
using System.IO;
using AlibabaCloud.SDK.Ocr_api20210707.Models;
using AlibabaCloud.TeaUtil.Models;
using AlibabaCloud.TeaUtil;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2023.09.12
    /// 描 述：阿里云OCR文字识别帮助类
    /// </summary>
    public class AliyunOcrHelper
    {
        private readonly string _endpoint, _accessId, _accessSecret;
        private readonly Client client;

        /// <summary>
        /// 初始化
        /// </summary>
        public AliyunOcrHelper()
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            _accessId = setting.OcrAccessKeyId;
            _accessSecret = setting.OcrAccessKeySecret;
            _endpoint= setting.OcrEndpoint;
            client = CreateClient(_accessId, _accessSecret, _endpoint);
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="accessSecret"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static Client CreateClient(string accessId, string accessSecret, string endpoint)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = accessId,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = accessSecret,
            };
            config.Endpoint = endpoint;
            return new Client(config);
        }

        #region 通用文字识别
        /// <summary>
        /// 全文识别高精版
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeAdvancedResponse RecognizeAdvanced(Stream stream)
        {
            RecognizeAdvancedRequest recognizeAdvancedRequest = new RecognizeAdvancedRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeAdvancedWithOptions(recognizeAdvancedRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 通用手写体识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeHandwritingResponse RecognizeHandwriting(Stream stream)
        {
            RecognizeHandwritingRequest recognizeHandwritingRequest = new RecognizeHandwritingRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeHandwritingWithOptions(recognizeHandwritingRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 电商图片文字识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeBasicResponse RecognizeBasic(Stream stream)
        {
            RecognizeBasicRequest recognizeBasicRequest = new RecognizeBasicRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeBasicWithOptions(recognizeBasicRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 通用文字识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeGeneralResponse RecognizeGeneral(Stream stream)
        {
            RecognizeGeneralRequest recognizeGeneralRequest = new RecognizeGeneralRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeGeneralWithOptions(recognizeGeneralRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 表格识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeTableOcrResponse RecognizeTableOcr(Stream stream)
        {
            RecognizeTableOcrRequest recognizeTableOcrRequest = new RecognizeTableOcrRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeTableOcrWithOptions(recognizeTableOcrRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 文档结构化识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeDocumentStructureResponse RecognizeDocumentStructure(Stream stream)
        {
            RecognizeDocumentStructureRequest recognizeDocumentStructureRequest = new RecognizeDocumentStructureRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeDocumentStructureWithOptions(recognizeDocumentStructureRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        #endregion

        #region 个人证照识别
        /// <summary>
        /// 身份证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeIdcardResponse RecognizeIdcard(Stream stream)
        {
            RecognizeIdcardRequest recognizeIdcardRequest = new RecognizeIdcardRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeIdcardWithOptions(recognizeIdcardRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 护照识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizePassportResponse RecognizePassport(Stream stream)
        {
            RecognizePassportRequest recognizePassportRequest = new RecognizePassportRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizePassportWithOptions(recognizePassportRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 户口本识别，支持户口本户主页和常住人口页识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeHouseholdResponse RecognizeHousehold(Stream stream)
        {
            RecognizeHouseholdRequest recognizeHouseholdRequest = new RecognizeHouseholdRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeHouseholdWithOptions(recognizeHouseholdRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 不动产权证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeEstateCertificationResponse RecognizeEstateCertification(Stream stream)
        {
            RecognizeEstateCertificationRequest recognizeEstateCertificationRequest = new RecognizeEstateCertificationRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeEstateCertificationWithOptions(recognizeEstateCertificationRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 银行卡识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeBankCardResponse RecognizeBankCard(Stream stream)
        {
            RecognizeBankCardRequest recognizeBankCardRequest = new RecognizeBankCardRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeBankCardWithOptions(recognizeBankCardRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 出生证明识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeBirthCertificationResponse RecognizeBirthCertification(Stream stream)
        {
            RecognizeBirthCertificationRequest recognizeBirthCertificationRequest = new RecognizeBirthCertificationRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeBirthCertificationWithOptions(recognizeBirthCertificationRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 中国护照识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeChinesePassportResponse RecognizeChinesePassport(Stream stream)
        {
            RecognizeChinesePassportRequest recognizeChinesePassportRequest = new RecognizeChinesePassportRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeChinesePassportWithOptions(recognizeChinesePassportRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 来往大陆（内地）通行证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeExitEntryPermitToMainlandResponse RecognizeExitEntryPermitToMainland(Stream stream)
        {
            RecognizeExitEntryPermitToMainlandRequest recognizeExitEntryPermitToMainlandRequest = new RecognizeExitEntryPermitToMainlandRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeExitEntryPermitToMainlandWithOptions(recognizeExitEntryPermitToMainlandRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 往来港澳台通行证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeExitEntryPermitToHKResponse RecognizeExitEntryPermitToHK(Stream stream)
        {
            RecognizeExitEntryPermitToHKRequest recognizeExitEntryPermitToHKRequest = new RecognizeExitEntryPermitToHKRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeExitEntryPermitToHKWithOptions(recognizeExitEntryPermitToHKRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 中国香港身份证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeHKIdcardResponse RecognizeHKIdcard(Stream stream)
        {
            RecognizeHKIdcardRequest recognizeHKIdcardRequest = new RecognizeHKIdcardRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeHKIdcardWithOptions(recognizeHKIdcardRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 社保卡识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeSocialSecurityCardVersionIIResponse RecognizeSocialSecurityCardVersionII(Stream stream)
        {
            RecognizeSocialSecurityCardVersionIIRequest recognizeSocialSecurityCardVersionIIRequest = new RecognizeSocialSecurityCardVersionIIRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeSocialSecurityCardVersionIIWithOptions(recognizeSocialSecurityCardVersionIIRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 国际身份证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeInternationalIdcardResponse RecognizeInternationalIdcard(Stream stream)
        {
            RecognizeInternationalIdcardRequest recognizeInternationalIdcardRequest = new RecognizeInternationalIdcardRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeInternationalIdcardWithOptions(recognizeInternationalIdcardRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        #endregion

        #region 票据凭证识别
        /// <summary>
        /// 混贴发票识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeMixedInvoicesResponse RecognizeMixedInvoices(Stream stream)
        {
            RecognizeMixedInvoicesRequest recognizeMixedInvoicesRequest = new RecognizeMixedInvoicesRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeMixedInvoicesWithOptions(recognizeMixedInvoicesRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 增值税发票识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeInvoiceResponse RecognizeInvoice(Stream stream)
        {
            RecognizeInvoiceRequest recognizeInvoiceRequest = new RecognizeInvoiceRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeInvoiceWithOptions(recognizeInvoiceRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        #endregion

        #region 企业资质识别
        /// <summary>
        /// 营业执照识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeBusinessLicenseResponse RecognizeBusinessLicense(Stream stream)
        {
            RecognizeBusinessLicenseRequest recognizeBusinessLicenseRequest = new RecognizeBusinessLicenseRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeBusinessLicenseWithOptions(recognizeBusinessLicenseRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 银行开户许可证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeBankAccountLicenseResponse RecognizeBankAccountLicense(Stream stream)
        {
            RecognizeBankAccountLicenseRequest recognizeBankAccountLicenseRequest = new RecognizeBankAccountLicenseRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeBankAccountLicenseWithOptions(recognizeBankAccountLicenseRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        #endregion

        #region 车辆物流识别
        /// <summary>
        /// 行驶证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeVehicleLicenseResponse RecognizeVehicleLicense(Stream stream)
        {
            RecognizeVehicleLicenseRequest recognizeVehicleLicenseRequest = new RecognizeVehicleLicenseRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeVehicleLicenseWithOptions(recognizeVehicleLicenseRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 驾驶证识别
        /// </summary>
        /// <param name="stream"></param>
        public RecognizeDrivingLicenseResponse RecognizeDrivingLicense(Stream stream)
        {
            RecognizeDrivingLicenseRequest recognizeDrivingLicenseRequest = new RecognizeDrivingLicenseRequest
            {
                Body = stream,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var res = client.RecognizeDrivingLicenseWithOptions(recognizeDrivingLicenseRequest, new RuntimeOptions());
                return res;
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                Common.AssertAsString(error.Message);
            }
            return null;
        }
        #endregion
    }
}
