using System;
namespace linerider.Plugins
{
    public class Plugin
    {
        public bool loaded
        {
            get {
                return _loaded;
            }
            set {
                if (!value && _loaded)
                {
                    Unload();
                    _loaded = false;
                }
                else if (value && !_loaded)
                {
                    Load();
                    _loaded = true;
                }
            }
        }
        private bool _loaded = true;
        public Plugin()
        {
            
        }
        public string name;
        public virtual void Load() { }
        public virtual void Unload() { }
    }
}
