using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyXml.PrProj
{
    public class PremiereProject
    {
        private readonly XDocument _doc;
        private readonly Dictionary<PremiereObjectKey, BasePremiereObject> _objects = new Dictionary<PremiereObjectKey, BasePremiereObject>();

        public PremiereProject(XDocument doc)
        {
            this._doc = doc;
            //_doc.Elements()
        }

        public List<T> GetAll<T>()
        {
            return _objects.Values.OfType<T>().ToList();
        }

        public T AddOrGet<T>(PremiereObjectKey key) where T : BasePremiereObject, new()
        {
            BasePremiereObject res;
            if (_objects.TryGetValue(key, out res))
                return (T) res;
            res = new T
            {
                Key = key,
            };
            _objects.Add(key, res);
            return (T)res;
        }

        public void Add(BasePremiereObject value)
        {
            _objects[value.Key] = value;
        }

        public void Save(string filePath)
        {
            var bakPath = GetFreeBackPath(filePath + ".bak");

            File.Move(filePath, bakPath);
            using (var file = File.OpenWrite(filePath))
            {
                _doc.Save(file);
            }
        }

        private string GetFreeBackPath(string path)
        {
            if (!File.Exists(path))
                return path;
            var ext = Path.GetExtension(path).Split('-');
            var index = 0;
            if (ext.Length > 1)
                int.TryParse(ext[1], out index);

            for (int i = index; ; i++)
            {
                var newPath = path + "-" + i;
                if (!File.Exists(newPath))
                    return newPath;
            }
        }
    }
}
