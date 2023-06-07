#define LOCAL_DEBUG_API_OFF // API 開關

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

using Meoweb.Commons;
using AppTemplate.Databases.Npgsql;

namespace AppTemplate.Controllers {
    using IResult = Meoweb.Commons.Data.IResult;

    // For 資料模型
    public partial class My_AllController : WebApiTemplate
        <My_AllController.RequestDataModel, My_AllController.ResponseDataModel> {

        // 請求時-資料模型
        public struct RequestDataModel {
            public string Data { get; set; } // 僅作參考
            // More... (自行添加: 請求時需要的資料欄位)

        }
        
        // 回復時-資料模型
        public struct ResponseDataModel : IResult {
            public string Data { get; set; } // 僅作參考
            // More... (自行添加: 回復時需要的資料欄位)



            /* ============================================
            *   以下屬性慎改
            */
            public int ResultCode { get; set; }     // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }   // 響應資訊 ( 錯誤時應記錄錯誤資訊 )
            /* ============================================
            *   For Debug
            */
            public override string ToString() {
                return "\n" + base.ToString() + " {\n"
                    + $"  >> ResultCode: {ResultCode}\n"
                    + $"  >> ResultMsg: {ResultMsg}\n"
                    + "}\n";
            }
        }

    }



    // For 請求  (前置檢查 & 響應)
#if !LOCAL_DEBUG_API_OFF            // 如果：本頁最上方定義了 LOCAL_DEBUG_API_OFF 則關閉此 API
    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況來指定策略，請遵循安全策略)
    //[Authorize]                     // 啓用-身份驗證 (驗證通過才能夠訪問此資源)
#endif
    public partial class My_AllController {

#pragma warning disable CS1998 // Async 方法缺乏 'await' 運算子，將同步執行 

        /// <summary>
        /// >> Post 請求，數據在 FromBody (數據體)
        /// </summary>
        /// <param name="value">Post 請求時，需提供的資料</param>
        /// <returns>響應結果</returns>
        [HttpPost]
        //[Produces("application/json")]
        //[ProducesResponseType(typeof(RequestDataModel), 200)]
        public virtual async Task<object> Post([FromBody] RequestDataModel value) {
            // todo: 處理 Post 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        /// <summary>
        /// >> Get 請求，數據附加在 URL (路由連結)
        /// </summary>
        /// <param name="value">Get 請求時，需提供附加在 URL 的資料</param>
        /// <returns>響應結果</returns>
        [HttpGet]
        public virtual async Task<object> Get([FromBody] RequestDataModel value) {
            // todo: 處理 Get 請求的邏輯
            return ResponseData; // 回復請求結果
        }
        
        [HttpDelete]
        public virtual async Task<object> Delete([FromBody] RequestDataModel value) {
            // todo: 處理 Delete 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpPut]
        public virtual async Task<object> Put([FromBody] RequestDataModel value) {
            // todo: 處理 Put 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpPatch]
        public virtual async Task<object> Patch([FromBody] RequestDataModel value) {
            // todo: 處理 Patch 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpHead]
        public virtual async Task<object> Head([FromBody] RequestDataModel value) {
            // todo: 處理 Head 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpOptions]
        public virtual async Task<object> Options([FromBody] RequestDataModel value) {
            // todo: 處理 Options 請求的邏輯
            return ResponseData; // 回復請求結果
        }

#pragma warning restore CS1998

    }

    // For 構建式 (依賴注入 >> 注入資料庫)
    public partial class My_AllController {

        protected MyDbCtx MyDbCtx { get; set; }

        /// <summary>
        /// Constructor 構建式
        /// </summary>
        /// <param name="logger">依賴注入: 日志</param>
        /// <param name="dbCtx"></param>
        public My_AllController(ILogger<My_AllController> logger, MyDbCtx dbCtx)
            :base(logger, new RequestDataModel(), new ResponseDataModel()) {
            MyDbCtx = dbCtx;
        }

        /// <summary>
        /// **建立-響應**
        /// </summary>
        protected override void BuildResponse() {
            base.BuildResponse();
            Logger.LogInformation($"{ResponseData}\n");
        }

    }

    // For 處理  (資料庫查詢 & 處理業務邏輯)
    public partial class My_AllController {

        protected override bool ProcessData() {
            // 如果僅單一職責，請使用此方法
            return false;
        }

    }

}
