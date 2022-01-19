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
        public List<ShapeData> Data { get; set; }
        
        public string Address { get; set; }
    }
    
    public class Project
    {
        public List<IShape> UserShapes { get; set; }
        public string Address { get; set; }

        public bool IsSaved { get; set; }
        public Project()
        {
            UserShapes = new List<IShape>();
            Address = "";
            IsSaved = true;
        }

        public void SaveToFile()
        {
            ProjectData data = new ProjectData() { 
                Address = this.Address,
                Data = new List<ShapeData>()
            };
            foreach(var shape in UserShapes)
            {
                data.Data.Add(new ShapeData()
                {
                    Name = shape.Name,
                    Data = shape.ToJson()
                });
            }
            string json = JsonSerializer.Serialize(data);
            using (var stream = File.Open(Address, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(json);
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
                            data = (ProjectData)JsonSerializer.Deserialize(json, typeof(ProjectData));
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
                    foreach (var shape in data.Data)
                    {
                        result.UserShapes.Add(ShapeFactory.GetInstance().Create(shape));
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
            foreach (var shape in this.UserShapes)
            {
                result.UserShapes.Add(shape.Clone());
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
