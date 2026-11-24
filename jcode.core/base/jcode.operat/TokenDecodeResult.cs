namespace jcode.operat
{
    /// <summary>
    /// JWT 解析结果
    /// </summary>
    public class TokenDecodeResult
    {
        /// <summary>
        /// 是否解析成功（签名通过、未过期、类型匹配）
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 失败原因；成功时为空
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 原始载荷 JSON
        /// </summary>
        public string Json { get; set; }
        /// <summary>
        /// 载荷对象
        /// </summary>
        public Payload Payload { get; set; }
    }
}
