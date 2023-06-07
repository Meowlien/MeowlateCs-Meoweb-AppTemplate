#define DEBUG_UseFakeData // 定義使用模擬資料(假資料)

using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Meoweb.Commons;
using AppTemplate.Models;

namespace AppTemplate.Databases.Npgsql {

    // For 構建
    /// <summary>
    /// Npgsql 資料庫上下文 (範本)
    /// </summary>
    /// <remarks>
    /// 
    /// <br>説明：</br>
    ///     <br> - 資料庫上下文是作爲資料庫連綫的輕量級操作單元，用以資料庫相關操作如：查詢、刪除、添加及修改等</br>
    /// <br></br>
    /// 
    /// <br>注意事項：</br>
    ///     <br> - 請將相關業務邏輯統整到同一個上下文</br>
    /// <br></br>
    /// 
    /// <br>繼承關係：</br>
    ///     <br> - <see cref="MyDbCtx" /> ( 此類別：僅支援 Npgsql 資料庫的上下文 ) </br>
    ///     <br> - <see cref="NpgsqlDbCtxTemplate" /> ( Npgsql 資料庫上下文-模板 ) </br>
    ///     <br> - <see cref="DbCtxTemplate" /> ( 資料庫上下文通用型-模板 ) </br>
    ///     <br> - <see cref="DbContext" /> ( Supporter: Microsoft's Entity Framework Core 微軟支援的資料庫實體框架核心 ) </br>
    /// <br></br>
    /// 
    /// </remarks>
    public partial class MyDbCtx : NpgsqlDbCtxTemplate {
        public MyDbCtx(ILogger<MyDbCtx> logger)
            : base(logger) {
        }
        public MyDbCtx(DbContextOptions<MyDbCtx> options, ILogger<MyDbCtx> logger)
            : base(options, logger) {
        }
    }

    // For 結果資料模型  (承載體)
    public partial class MyDbCtx : NpgsqlDbCtxTemplate {
        public DbSet<MyDataModels.Demo.Result> DemoResultModel { get; set; } // 命名規範：<後綴> ResultModel
        // More...
    }

    // For 服務  (業務邏輯)
    public partial class MyDbCtx : NpgsqlDbCtxTemplate {

        public bool GetUserById(                        // 回傳-是否成功
                string id,                              // 接收-參數 >> id
            out MyDataModels.User? result               // 結果-資料
        ) {
#if DEBUG_UseFakeData // 使用模擬資料
            // 創建結果
            result = new() {
                Id = "FakeData_" + id,
                Name = "FakeData_Alan",
                Gender = "FakeData_Male",
                Email = "FakeData_abc123@gmail.com",
            };
            return true; // 回傳-成功
#else
            /* 建立-查詢指令
            *   - SQL 範例：SELECT 結果欄位 FROM 資料庫名稱 WHERE 條件
            */
            string cmd = "SELECT * FROM User WHERE id=@id, password=@password";

            // 建立-查詢參數
            NpgsqlParameter[] param = new NpgsqlParameter[] {
                new NpgsqlParameter("@id", parameter.Id),
                new NpgsqlParameter("@password", parameter.Password),
            };

            // 呼叫-資料庫
            var resultList = LoginResultModel.FromSqlRaw(cmd, param).ToList();

            // 獲取一筆資料
            result = resultList.FirstOrDefault(); // 多筆資料需注解此行

            /* 獲取多筆資料
            *   - 1. 請注解掉該行内容: result = resultList.FirstOrDefault();
            *   - 2. 將 result 參數改爲: out List<SampleDataModel.Login.Result>? result
            */

            // 返回執行判斷結果
            return resultList.Count > 0; // 大於 0 筆資料即為查詢到結果，返回 true; 反之亦然。
#endif
        }

        public bool GetUserByAny(                           // 回傳-是否成功
                string any,                                 // 接收-參數 >> any
            out List<MyDataModels.User>? resultList         // 結果-資料
        ) {
#if DEBUG_UseFakeData
            resultList = Enumerable.Range(1, 5).Select(index => new MyDataModels.User {
                Id = "FakeData_123456",
                Name = "FakeData_" + any,
                Gender = "FakeData_Male",
                Email = "FakeData_abc123@gmail.com",
            })
            .ToList();
            return true;
#else
            /* 建立-查詢指令
            *   - SQL 範例：SELECT 結果欄位 FROM 資料庫名稱 WHERE 條件
            */
            string cmd = "SELECT * FROM User WHERE id=@id, password=@password";

            // 建立-查詢參數
            NpgsqlParameter[] param = new NpgsqlParameter[] {
                new NpgsqlParameter("@id", parameter.Id),
                new NpgsqlParameter("@password", parameter.Password),
            };

            // 呼叫-資料庫
            var resultList = LoginResultModel.FromSqlRaw(cmd, param).ToList();

            // 獲取一筆資料
            result = resultList.FirstOrDefault(); // 多筆資料需注解此行

            /* 獲取多筆資料
            *   - 1. 請注解掉該行内容: result = resultList.FirstOrDefault();
            *   - 2. 將 result 參數改爲: out List<SampleDataModel.Login.Result>? result
            */

            // 返回執行判斷結果
            return resultList.Count > 0; // 大於 0 筆資料即為查詢到結果，返回 true; 反之亦然。
#endif
        }

        public bool GetDataByAny(                         // 回傳-是否成功
                MyDataModels.Demo.Linq parameter,         // 接收-參數
            out MyDataModels.Demo.Result? result          // 結果-資料
        ) {

#if DEBUG_UseFakeData // 使用模擬資料

            // 創建結果
            result = new() {
                ResultData_A = "FakeData_A", // 寫入資料
                ResultData_B = "FakeData_B", // 寫入資料
            };
            return true; // 回傳-成功
#else
            /* 建立-查詢指令
            *   - SQL 範例：SELECT 結果欄位 FROM 資料庫名稱 WHERE 條件
            */
            string cmd = "SELECT * FROM User WHERE id=@id, password=@password";

            // 建立-查詢參數
            NpgsqlParameter[] param = new NpgsqlParameter[] {
                new NpgsqlParameter("@id", parameter.Id),
                new NpgsqlParameter("@password", parameter.Password),
            };

            // 呼叫-資料庫
            var resultList = LoginResultModel.FromSqlRaw(cmd, param).ToList();

            // 獲取一筆資料
            result = resultList.FirstOrDefault(); // 多筆資料需注解此行

            /* 獲取多筆資料
            *   - 1. 請注解掉該行内容: result = resultList.FirstOrDefault();
            *   - 2. 將 result 參數改爲: out List<SampleDataModel.Login.Result>? result
            */

            // 返回執行判斷結果
            return resultList.Count > 0; // 大於 0 筆資料即為查詢到結果，返回 true; 反之亦然。
#endif
        }

    }

}





