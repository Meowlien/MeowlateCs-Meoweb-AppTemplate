// #define LOCAL_DEBUG_API_OFF // API 開關

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using Meoweb.Commons;
using AppTemplate.Models;
using AppTemplate.Databases.Npgsql;

namespace AppTemplate.Controllers {
    using IResult = Meoweb.Commons.Data.IResult;

    // For 資料模型
    public partial class My_GetController : WebApiTemplate
        <My_GetController.RequestDataModel, My_GetController.ResponseDataModel> {

        // 請求時-資料模型
        public struct RequestDataModel {
            public string Id { get; set; } // 僅作參考
            // More... (自行添加: 請求時需要的資料欄位)
        }

        // 回復時-資料模型
        public struct ResponseDataModel : IResult {
            public List<MyDataModels.User> UserDataList { get; set; } // 僅作參考
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
    public partial class My_GetController {

        [HttpGet("id/{id}")] // 路由範例 >> 呼叫：GET 'api/SampleUserData/id/123' 即： id = 123
        public virtual async Task<object> GetDataById(
            [FromRoute] string id // 變數名稱 id 必須和 HttpGet 標簽中的名稱 {id} 一致
        /*  [FromRoute] string Data_A, */
        /*  [FromRoute] string Data_B */
        /*  [FromRoute] 更多... */) {

            // 0. 注意 >> 所有錯誤資訊由方法内部建立

            // 1. 緩存-請求内容
            RequestData.Id = id;

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
                // 檢查：id 是否有内容
                id.Length > 0 == true
                #endregion
            ) {
                // 透過 id 查找資料
                if (TryGetDataById(id) == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
            }

            // 4. 響應結果
            BuildResponse();        // 建立-響應(打包響應資訊)
            return ResponseData;    // 回復結果 (成功/失敗)
        }

        [HttpGet("any/{any}")]
        public virtual async Task<object> GetDataByAny(
            [FromRoute] string any) {

            // 0. 注意 >> 所有錯誤資訊由方法内部建立

            // 1. 緩存-請求内容
            RequestData.Id = any;

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
                // 檢查：id 是否有内容
                any.Length > 0 == true
                #endregion
            ) {
                // 透過 id 查找資料
                if (TryGetDataByAny(any) == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
            }

            // 4. 響應結果
            BuildResponse();        // 建立-響應(打包響應資訊)
            return ResponseData;    // 回復結果 (成功/失敗)
        }


    }

    // For 構建式 (依賴注入 >> 注入資料庫)
    public partial class My_GetController {

        protected MyDbCtx MyDbCtx { get; set; }

        /// <summary>
        /// Constructor 構建式
        /// </summary>
        /// <param name="logger">依賴注入: 日志</param>
        /// <param name="dbCtx"></param>
        public My_GetController(ILogger<My_GetController> logger, MyDbCtx dbCtx)
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
    public partial class My_GetController {

        protected bool TryGetDataById(string id) {
            // 嘗試呼叫-資料庫
            try {

                // 呼叫資料庫 & 綜合查詢成功
                if(MyDbCtx.GetUserById(id, out MyDataModels.User? record) == true) {
                    // 檢查：資料是否存在?
                    if (record != null) {
                        // 寫入-響應正文
                        ResponseData.UserDataList = new(); // 創建一筆資料集
                        var data = new MyDataModels.User() { // 創建一筆記錄
                            Id = record.Id,
                            Name = record.Name,
                            Gender = record.Gender,
                            Email = record.Email
                        };
                        ResponseData.UserDataList.Add(data); // 添加記錄到清單中
                        return true;
                    }
                    else {
                        Logger.LogInformation("Linq result data is null!");
                        BuildResult(WebApiResult.Code.Fail, "No Data");
                        return false;
                    }
                }

                // 查詢失敗，沒有相關 or 符合條件的資料
                BuildResult(WebApiResult.Code.Fail, "Cannot Found.");
                return false;
            }

            // 捕獲例外狀況
            catch (Exception ex) {
#pragma warning disable CA2254
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
#pragma warning restore CA2254

#if DEBUG
                // 暴露例外訊息不安全
                BuildResult(WebApiResult.Code.CheckFailed_ValidData);
#else
                // undone
                //BuildResult(WebApiResult.Code.CheckFailed_ValidData);
#endif
                return false;
            }
        }

        protected bool TryGetDataByAny(string any) {
            // 嘗試呼叫-資料庫
            try {

                // 呼叫資料庫 & 綜合查詢成功
                if(MyDbCtx.GetUserByAny(any, out List<MyDataModels.User>? dataList) == true) {
                    // 檢查：資料是否存在?
                    if (dataList != null) {
                        // 寫入-響應正文
                        ResponseData.UserDataList = new(); // 創建一筆資料集
                        foreach (var record in dataList) {
                            var data = new MyDataModels.User() { // 創建一筆記錄
                                Id = record.Id,
                                Name = record.Name,
                                Gender = record.Gender,
                                Email = record.Email
                            };
                            ResponseData.UserDataList.Add(data); // 添加記錄到清單中
                        }
                        return true;
                    }
                    else {
                        Logger.LogInformation("Linq result data is null!");
                        BuildResult(WebApiResult.Code.Fail, "No Data");
                        return false;
                    }
                }

                // 查詢失敗，沒有相關 or 符合條件的資料
                BuildResult(WebApiResult.Code.Fail, "Cannot Found.");
                return false;
            }

            // 捕獲例外狀況
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



