using System;
using Gwen;
using Gwen.Controls;
using linerider.Plugins;
namespace linerider.UI
{
    public class PluginsDialog : DialogBase
    {
        private string title = "Plugins";
        public TreeControl _tree;
        public Button _load;
        public Button _unload;
        public PluginsDialog(GameCanvas parent, Editor editor) : base(parent,editor)
        {
            Title = title;
            _tree = new TreeControl(this)
            {
                Dock = Dock.Fill
            };
            Panel bottom = new Panel(this)
            {
                Dock = Dock.Bottom,
                AutoSizeToContents = true,
                Margin = new Margin(0, 5, 0, 0)
            };

            _load = new Button(bottom)
            {
                Dock = Dock.Left,
                Text = "Load",
                Padding = new Padding(10, 0, 10, 0)
            };
            _unload = new Button(bottom)
            {
                Dock = Dock.Right,
                Text = "Unload",
                Padding = new Padding(10, 0, 10, 0)
            };
            _unload.Clicked += (o, e) =>
            {
                if (_tree.SelectedChildren.Count == 1)
                {
                    PluginManager.pluginsByName[_tree.SelectedChildren[0].Text].loaded = false;
                }
            };
            _load.Clicked += (o, e) =>
            {
                if (_tree.SelectedChildren.Count == 1)
                {
                    PluginManager.pluginsByName[_tree.SelectedChildren[0].Text].loaded = true;
                }
            };
            SetSize(400, 400);
            Setup();
        }
        public void Setup() { 
            foreach (string name in PluginManager.pluginsByName.Keys) {
                _tree.AddNode(name);
            }
        }
    }
}
