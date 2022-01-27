using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Paint
{
    class RecentFile
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public RecentFile() { }
        
        public RecentFile(string path)
        {
            Path = path;
            Name = GetName(path);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public string GetName(string path)
        {
            if (path != null && path.Length > 0)
            {
                FileInfo file = new FileInfo(path);
                string[] tokens = file.Name.Split(".");
                string name = "";
                int n = tokens.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    name += tokens[i];
                    if (i != n - 2)
                    {
                        name += ".";
                    }
                }
                return name;
            }
            return "";
        }

        public static BindingList<RecentFile> GetRecentFileList(string filepath)
        {
            string json = "";
            BindingList<RecentFile> recentList = null;
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
                            recentList = (BindingList<RecentFile>)JsonSerializer.Deserialize(decodedtext, typeof(BindingList<RecentFile>));
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            }
            for (int i = recentList.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(recentList[i].Path))
                {
                    recentList.RemoveAt(i);
                }
            }
            return recentList;
        }

        public static void WriteRecentFile(string filepath, BindingList<RecentFile> recentlist)
        {
            BindingList<RecentFile> newRecentList = new BindingList<RecentFile>();
            for (int i = 0; i < Math.Min(10, recentlist.Count); i++)
            {
                newRecentList.Add(recentlist[i]);
            }
            string json = JsonSerializer.Serialize(newRecentList);
            var plaintext = Encoding.UTF8.GetBytes(json);
            var encodedtext = Convert.ToBase64String(plaintext);
            using (var stream = File.Open(filepath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(encodedtext);
                }
            }
        }
    }
}
