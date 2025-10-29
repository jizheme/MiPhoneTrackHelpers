using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SqlSugar;

namespace AvaloniaApplication1
{
    public class LocationInfo
    {
        public string area { get; set; }
        public string address { get; set; }
        public string sourceType { get; set; }
        public double latitude { get; set; }
        public bool inChinaMainLand { get; set; }
        public double accuracy { get; set; }
        public string coordinateType { get; set; }
        public string addressComponent { get; set; }
        public double longitude { get; set; }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        public long clientUpdateTime { get; set; }

        public string phone { get; set; }
        public DateTime CreateTime { get; set; }
        public string phoneID { get; set; }

        public string WebSource { get; set; }
    }
}
