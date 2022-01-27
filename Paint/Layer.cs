using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
  

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

        public Layer()
        {
        }

        public Layer Clone()
        {
            var temptShapeList = new List<IShape>();

            foreach (var shape in UserShapes)
            {
                temptShapeList.Add(shape.Clone());
            }

            return new Layer()
            {
                name = this.name,
                isVisible = this.isVisible,
                UserShapes = temptShapeList
            };
        }



    }
}
