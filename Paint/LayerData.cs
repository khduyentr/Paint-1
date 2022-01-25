using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class LayerData
    {
        public String Name { get; set; }
        public bool isVivible { get; set; }
        public List<ShapeData> Data { get; set; }
    }
}
