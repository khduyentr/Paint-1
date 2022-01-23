using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class LayerData
    {
        public List<ShapeData> Data { get; set; }

        
    }

    public class Layer 
    {
      
        public string name { get; set; }
        public List<IShape> UserShapes { get; set; }

        public bool isVisible { get; set; }

        public Layer (string name, List<IShape> UserShapes, bool isVisible)
        {
            this.name = name;
            this.UserShapes = UserShapes;
            this.isVisible = isVisible;
        }

    }
}
