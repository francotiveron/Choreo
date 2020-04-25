using System;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo.TwinCAT {
    public interface ITag {
        string Path { get; }
        void Write(object value);
        object Read();
        void Push(object value);
    }
    
    public class Tag: ITag {
        public Tag(string path, Action<object> pusher = null) { 
            Path = $"GVL.{path}";
            Pusher = pusher;
        }

        public string Path { get; private set; }
        public Action<object> Pusher { get; private set; }

        public virtual void Push(object value) {
            Pusher?.Invoke(value);
        }

        public virtual object Read() {
            throw new NotImplementedException();
        }

        public virtual void Write(object value) {
            throw new NotImplementedException();
        }
    }
}
