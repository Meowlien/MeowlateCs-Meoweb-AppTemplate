// #define LOCAL_DEBUG_API_OFF // API 開關

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

using Meoweb.Commons;
using AppTemplate.Models;
using AppTemplate.Databases.Npgsql;

namespace AppTemplate.Controllers {
    using IResult = Meoweb.Commons.Data.IResult;

    // For 資料模型
    public partial class My_PostController : WebApiTemplate
        <My_PostController.RequestDataModel, My_PostController.ResponseDataModel> {

        // 請求時-資料模型
        public struct RequestDataModel {
            public string Data_A { get; set; } // 僅作參考
            public string Data_B { get; set; } // 僅作參考
            // More... (自行添加: 請求時需要的資料欄位)

        }
        
        // 回復時-資料模型
        public struct ResponseDataModel : IResult {
            public string ResultData_A { get; set; } // 僅作參考
            public string ResultData_B { get; set; } // 僅作參考
            // More... (自行添加: 回復時需要的資料欄位)



            /*  以下屬性慎改 *********************************/
            public int ResultCode { get; set; }
            public string ResultMsg { get; set; }
            /***********************************************/
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
    //[EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況來指定策略，請遵循安全策略)
    //[Authorize]                     // 啓用-身份驗證 (驗證通過才能夠訪問此資源)
#endif
    public partial class My_PostController {

        /// <summary>
        /// >> Post 請求，數據在 FromBody (數據體)
        /// </summary>
        /// <param name="value">Post 請求時，需提供的資料</param>
        /// <returns>響應結果</returns>
        [HttpPost]
        public virtual async Task<object> Post([FromBody] RequestDataModel value) {
            // 0. 注意 >> 所有錯誤資訊由方法内部建立

            // 1. 緩存-請求内容
            RequestData = value;

            // 2. 解析-請求内容
            if(await BuildRequest() == false) {
                BuildResponse();        // 建立-響應(打包響應資訊)
                return ResponseData;    // 回復結果 (成功/失敗)
            }

            // 3. 前置檢查
            if (
                #region 檢查條件
                /*  前置檢查條件-説明 *************************************************************
                *   - 1：檢查請求所携帶的請求正文是否符合此<資料模型(RequestDataModel)>
                *   - 2：檢查請求正文是否携帶必要欄位資訊以做校驗等
                *   - 原因：排除 | 過濾不符條件的請求，以提高整體系統效率
                *   - 注意：過多的檢查會降低整體效率!!! 如非必要請不要涵蓋所有欄位進行檢查，可空欄不予以檢查
                *   - 以下為前置檢查流程，檢查條件、流程如需調整可於此調整
                ********************************************************************************/
                // 資料頭(Head)
                CheckValidDataHead(@"") == true
                // 資料體(Body)
                && CheckValidDataBody(@"") == true
                // 跨域資訊(CorsPolicy)
                && CheckHasCorsPolicy() == true
                // 高速資料庫比對資料(Redis)
                && ComparisonData() == true
                // 數據解密(Decription)
                && DataDecription() == true
                #endregion
            ) {
                // 呼叫資料庫
                if (ProcessData() == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
            }

            // 4. 響應結果
            BuildResponse();        // 建立-響應(打包響應資訊)
            return ResponseData;    // 回復結果 (成功/失敗)
        }
    }

    // For 構建式 (依賴注入 >> 注入資料庫)
    public partial class My_PostController {

        protected MyDbCtx MyDbCtx { get; set; }

        /// <summary>
        /// Constructor 構建式
        /// </summary>
        /// <param name="logger">依賴注入: 日志</param>
        /// <param name="dbCtx"></param>
        public My_PostController(ILogger<My_PostController> logger, MyDbCtx dbCtx)
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
    public partial class My_PostController {

        // 處理資料：請命名與該請求相關的名稱
        protected override bool ProcessData() {
            // 嘗試執行以下
            try {
                // 呼叫資料庫
                if(MyDbCtx.GetDataByAny(new MyDataModels.Demo.Linq() {
                    // 資料庫查詢所需參數
                    Data_A = RequestData.Data_A,
                    Data_B = RequestData.Data_B,

                // 查詢成功
                }, out MyDataModels.Demo.Result? data) == true) {

                    // 檢查：資料是否存在?
                    if (data != null) {
                        // 寫入-響應正文
                        ResponseData.ResultData_A = data.ResultData_A;
                        ResponseData.ResultData_B = data.ResultData_B;
                        return true; // 返回成功
                    }

                    // 資料不存在
                    Logger.LogInformation("Linq result data is null!");
                    BuildResult(WebApiResult.Code.Fail, "No Data");
                    return false; // 返回失敗
                }

                // 查詢失敗：沒有相關 or 符合條件的資料
                BuildResult(WebApiResult.Code.Fail, "Cannot Found.");
                return false; // 返回失敗
            }

            // 捕獲：例外狀況
            catch (Exception ex) {

#pragma warning disable CA2254
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
#pragma warning restore CA2254

#if DEBUG
                // 暴露例外訊息不安全
                BuildResult(WebApiResult.Code.Exception, ex.Message);
#else
                BuildResult(WebApiResult.Code.Exception);
#endif
                return false;
            }
        }

    }

}


