using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MPTKDDataEntry.Models
{
    public class BotInfo
    {
        public int ID { get; set; }
        [DataType(DataType.MultilineText)]
        public string Monday { get; set; }
        [DataType(DataType.MultilineText)]
        public string Tuesday { get; set; }
        [DataType(DataType.MultilineText)]
        public string Wedensday { get; set; }
        [DataType(DataType.MultilineText)]
        public string Thursday { get; set; }
        [DataType(DataType.MultilineText)]
        public string Friday { get; set; }
        [DataType(DataType.MultilineText)]
        public string Saturday { get; set; }
        public Boolean IsTesting { get; set; }
        [DataType(DataType.MultilineText)]
        public string Testing { get; set; }
        [DataType(DataType.MultilineText)]
        public string Promotion { get; set; }
        [DataType(DataType.MultilineText)]
        public string Holidays { get; set; }

    }
}