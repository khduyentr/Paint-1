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
                if (layer.isVisible)
                {
                    foreach (var shape in layer.UserShapes)
                    {
                        temptLayer.UserShapes.Add(shape);
                    }
                } 
            }

            return temptLayer;
        }

        static public List<Layer> UngroupLayer(Layer data)
        {
            List<Layer> result = new List<Layer>();
         

            foreach (var shape in data.UserShapes)
            {
                Layer temptLayer = new Layer()
                {
                    isVisible = true,
                    name = "tempt layer",
                    UserShapes = new List<IShape>()
                };
                temptLayer.isVisible = data.isVisible;
                temptLayer.name = shape.Name;
                temptLayer.UserShapes.Add(shape);
                result.Add(temptLayer);
            }

            return result;
        }

    }
}
