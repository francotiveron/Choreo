using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo.TwinCAT
{
    public class TagCollection: IEnumerable<ITag> {
        Dictionary<string, ITag> tags = new Dictionary<string, ITag>();
        Dictionary<string, string> pathMap = new Dictionary<string, string>();

        public TagCollection() {
            BuildTags(VM);
            foreach (var axis in VM.Axes) BuildTags(axis);
        }

        void BuildTags(object obj) {
            Type type = obj.GetType();

            string FullPath((PropertyInfo pi, PlcAttribute plc) _) {
                string key = null, path = null;

                switch(obj) {
                    case ViewModel vm:
                        path = $"GVL.{_.plc.Path}";
                        key = Tag.GetKey(nameof(ViewModel), _.pi.Name);
                        break;

                    case Motor m:
                        path = $"GVL.Axis_{m.Number:00}_{_.plc.Path ?? _.pi.Name}";
                        key = Tag.GetKey(nameof(Motor), _.pi.Name, m.Number);
                        break;

                    case Group g:
                        if (_.plc.Path is string _path && _path.StartsWith("GVL."))
                            path = _path.Replace("NN", $"{ (g.Number):00}");
                        else
                            path = $"GVL.Axis_{(g.Number + 16):00}_{_.plc.Path ?? _.pi.Name}";
                        key = Tag.GetKey(nameof(Group), _.pi.Name, g.Number);
                        break;
                }

                pathMap[key] = path;
                return path;
            }

            Action<object> PropertySetter(PropertyInfo pi) {
                return (value) => pi.SetValue(obj, value);
            }

            void NewTag((PropertyInfo pi, PlcAttribute plc) _) => tags[FullPath(_)] = new Tag(PropertySetter(_.pi));

            var pi_plc =
                type
                .GetProperties()
                .Select(pi => { var plc = pi.GetCustomAttribute<PlcAttribute>(); return (pi, plc); })
                .Where(pa => pa.plc != null);

            foreach (var pp in pi_plc) NewTag(pp);
        }

        public ReadOnlySymbolCollection Symbols {
            set {
                foreach (var kv in tags) kv.Value.Symbol = value[kv.Key];
            }
        }

        public IEnumerator<ITag> GetEnumerator() => tags.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => tags.Values.GetEnumerator();

        public ITag this[string path] => tags[path];
        public ITag this[ISymbol symbol] => tags[symbol.InstancePath];
        public ITag this[object obj, string propName] => tags[PathOf(obj, propName)];
        public string PathOf(object obj, string propName) => pathMap[Tag.GetKey(obj, propName)];
    }

}
