using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Identity.Client;

using SqlSugar;

namespace AvaloniaApplication1
{
    public static class SqlSugarHelpers
    {
        private static SqlSugarClient? _DbMaintenance;
        public static string? ConnectionString;

        public static SqlSugarClient DbMaintenance()
        {
            if (_DbMaintenance == null)
            {
                _DbMaintenance = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = DbType.Sqlite,
                    ConnectionString = @"datasource=data.db",
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true                  
                });

                _DbMaintenance.DbMaintenance.CreateDatabase();
                _DbMaintenance.CodeFirst.InitTables(typeof(LocationInfo),typeof(AppConfig));
            }
            return _DbMaintenance;
        }
    }
    public record PhoneListItemTemplate(string name, string phoneID);
}
