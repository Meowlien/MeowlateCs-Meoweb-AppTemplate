using  MeowebSettings= Meoweb.AppLibs.AppSettings;

namespace AppTemplate.AppLibs {

    /// <summary>
    /// 設置請求正文緩衝區大小為 1MB
    /// </summary>
    public static class AppSettings {

        /// <summary>
        /// 設置請求正文緩衝區大小為 1MB
        /// </summary>
        public static int MaxRequestBodyBufferSize {
            get => MeowebSettings.MaxRequestBodyBufferSize;
            set => MeowebSettings.MaxRequestBodyBufferSize = value;
        } // default = 1 * (1024 * 1024);

        /// <summary>
        /// 資料庫-連綫口號
        /// </summary>
        public static string DbConnectionStr {
            get => MeowebSettings.DbConnectionStr;
            set => MeowebSettings.DbConnectionStr = value;
        } // default = "";

        /// <summary>
        /// 跨域-許可清單
        /// </summary>
        public static List<string> Origins { get; set; } = new() {
            "https://localhost:80",
        };

    }

}
