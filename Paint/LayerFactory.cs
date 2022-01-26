using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    class LayerFactory
    {
        static public Layer Create(LayerData data)
        {
            Layer temptLayer = new Layer()
            {
                isVisible = data.isVivible,
                name = data.Name,
                UserShapes = new List<IShape>()
            };

            foreach (var shape in data.Data)
            {
                temptLayer.UserShapes.Add(ShapeFactory.GetInstance().Create(shape));
            }

            return temptLayer;
        }

        static public Layer GroupLayer(List<Layer> data)
        {
            Layer temptLayer = new Layer()
            {
                isVisible = true,
                name = "group layer",
                UserShapes = new List<IShape>()
            };

            foreach (var layer in data)
            {
                foreach (var shape in layer.UserShapes)
                {
                    temptLayer.UserShapes.Add(shape);
                }
            }

            return temptLayer;
        }

    }
}
