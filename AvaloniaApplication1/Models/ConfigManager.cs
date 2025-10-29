using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1
{
    using SqlSugar;

    using System;

    public class AppConfig
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }

    public static class ConfigManager
    {
        public static T GetConfig<T>(string key, T defaultValue)
        {
            var config = SqlSugarHelpers.DbMaintenance().Queryable<AppConfig>()
                .Where(c => c.Key == key)
                .First();

            if (config == null)
                return defaultValue;

            try
            {
                // 尝试类型转换（支持基础类型和字符串转换）
                return (T)Convert.ChangeType(config.Value, typeof(T));
            }
            catch
            {
                // 转换失败返回默认值
                return defaultValue;
            }
        }

        public static void SaveConfig(string key, object value)
        {
            if (value == null)
                return;

            var valueStr = value.ToString() ?? string.Empty;
            var existingConfig = SqlSugarHelpers.DbMaintenance().Queryable<AppConfig>()
                .Where(c => c.Key == key)
                .First();

            if (existingConfig == null)
            {
                SqlSugarHelpers.DbMaintenance().Insertable(new AppConfig
                {
                    Key = key,
                    Value = valueStr
                }).ExecuteCommand();
            }
            else
            {
                existingConfig.Value = valueStr;
                SqlSugarHelpers.DbMaintenance().Updateable(existingConfig).ExecuteCommand();
            }
        }

        public static int GetMapAccuracy() => GetConfig("MapAccuracy", 17);
        public static void SaveMapAccuracy(int v) => SaveConfig("MapAccuracy", v);

        public static int GetMapHoursLen() => GetConfig("MapHoursLen", 3);
        public static void SaveMapHoursLen(int v) => SaveConfig("MapHoursLen", v);

        public static int GetSelectAccuracy() => GetConfig("SelectAccuracy", 30);
        public static void SaveSelectAccuracy(int v) => SaveConfig("SelectAccuracy", v);
    }
}
