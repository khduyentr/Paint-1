using Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Paint
{
    public class ProjectData
    {
        public List<LayerData> Data { get; set; }
        
        public string Address { get; set; }
    }
    
    public class Project
    {

        public List<Layer> UserLayer { get; set; }
       
        public string Address { get; set; }

        public bool IsSaved { get; set; }

    
        public Project()
        {
           
            UserLayer = new List<Layer>();
            Address = "";
            IsSaved = true;
        }

        public string addNewLayer()
        {
            int current = UserLayer.Count;

            string layerName = "Layer " + current.ToString();
            List<IShape> temptList = new List<IShape>();
            Layer temptLayer = new Layer(layerName, temptList, true);
            this.UserLayer.Add(temptLayer);
            return layerName;
        }

        public void SaveToFile()
        {
            ProjectData projectData = new ProjectData() { 
                Address = this.Address,
                Data = new List<LayerData>()
            };
            foreach (var layer in UserLayer)
            {
                var tempLayerData = new LayerData()
                {
                    Name = layer.name,
                    isVivible = layer.isVisible,
                    Data = new List<ShapeData>()
                };

                foreach (var shape in layer.UserShapes)
                {

                    tempLayerData.Data.Add(new ShapeData()
                    {
                        Name = shape.Name,
                        Data = shape.ToJson()
                    });


                }
                projectData.Data.Add(tempLayerData);
            }

                
            string json = JsonSerializer.Serialize(projectData);
            var plaintext = Encoding.UTF8.GetBytes(json);
            var encodedtext = Convert.ToBase64String(plaintext);
            using (var stream = File.Open(Address, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(encodedtext);
                    IsSaved = true;
                }
            }
        }

        public static Project Parse(string filepath)
        {
            string json = "";
            ProjectData data = null;
            Project result = null;
            if (File.Exists(filepath))
            {
                using (var stream = File.Open(filepath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        
                        try
                        {
                            json = reader.ReadString();
                            var base64EncodedBytes = Convert.FromBase64String(json);
                            var decodedtext = Encoding.UTF8.GetString(base64EncodedBytes);
                            data = (ProjectData)JsonSerializer.Deserialize(decodedtext, typeof(ProjectData));
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            }
            if(data != null)
            {
                try
                {
                    result = new Project();
                    result.IsSaved = true;
                    result.Address = data.Address;
                    foreach (var layer in data.Data)  {
                        result.UserLayer.Add(LayerFactory.Create(layer));
                    }
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            return result;
        }

        public string GetName()
        {
            if (Address != null && Address.Length > 0)
            {
                FileInfo file = new FileInfo(Address);
                string[] tokens = file.Name.Split(".");
                string name = "";
                int n = tokens.Length;
                for(int i = 0; i < n - 1; i++){
                    name += tokens[i];
                    if(i != n - 2)
                    {
                       name += ".";
                    }
                }
                return name;
            }
            return "Untitled";
        }

        public Project Clone()
        {
            Project result = new Project();
            foreach (var layer in this.UserLayer)
            {
                result.UserLayer.Add(layer.Clone());             
            }
            result.Address = this.Address;
            return result;
        }

        public string GetProjectType()
        {
            if (Address != null && Address.Length > 0)
            {
                FileInfo file = new FileInfo(Address);
                string[] tokens = file.Name.Split(".");
                return tokens[tokens.Length - 1];
            }
            return "dat";
        }
    }
}
