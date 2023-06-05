using System.ComponentModel.DataAnnotations;

namespace AppTemplate.Models {

    // 範本資料模型
    public class MyDataModels {
        
        // 一般資料模型()
        public class Demo {
            // 綜合查詢 (查詢時需要的必備資料)
            public struct Linq { // Linq >> Language Integrated Query
                public string Data_A { get; set; }
                public string Data_B { get; set; }
            }
            // 查詢結果 (查詢結果資料載體)
            public class Result {
                [Key]
                public string ResultData_A { get; set; } = "";
                public string ResultData_B { get; set; } = "";
            }
        }

        public class User {
            [Key]
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Email { get; set; } = "";
        }

        // More...
    }

}
