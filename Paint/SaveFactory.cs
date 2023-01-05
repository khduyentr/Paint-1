using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class SaveFactory
    {
        static private SaveFactory instance = null;
        private Dictionary<string, ISaveStrategy> strategies = new Dictionary<string, ISaveStrategy>();

        static public SaveFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new SaveFactory();
            }
            return instance;
        }

        private SaveFactory()
        {
            strategies.Add("BMP", new SaveAsBPM());
            strategies.Add("PNG", new SaveAsPNG());
            strategies.Add("JPG", new SaveAsJPG());
        }

        public ISaveStrategy GetSaveStrategy(string key)
        {
            if (!strategies.ContainsKey(key)) return null;
            return strategies[key];
        }

    }
}
