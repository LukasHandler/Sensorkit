namespace Sensorkit.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LessonModel
    {
        public int Id { get; set; }

        public bool RunAble { get; set; }

        public string Name { get; set; }
        public string Content { get; set; }

        public string[] Arguments { get; set; }
    }
}
