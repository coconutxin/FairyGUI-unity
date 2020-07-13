using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FairyGUI
{
    public class GLoaderEx : GLoader
    {
        static private Action<GLoaderEx, string> _loadContentFunc = null;
        static private Action<GLoaderEx, string> _clearContentFunc = null;
        static public Action<GLoaderEx, string> LoadContentFunc
        {
            set
            {
                _loadContentFunc = value;
            }
        }

        static public Action<GLoaderEx, string> ClearContentFunc
        {
            set
            {
                _clearContentFunc = value;
            }
        }

        public new string url
        {
            get { return _url; }
            set
            {
                if (_url == value)
                    return;
                ClearContent();
                _url = value;
                LoadContent();
            }
        }

        protected new void LoadContent()
        {
            if (string.IsNullOrEmpty(_url))
                return;

            UnityEngine.Profiling.Profiler.BeginSample("GLoaderEx.LoadContent");
            _loadContentFunc.Invoke(this, _url);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        protected new void ClearContent()
        {
            base.ClearContent();
            if (_url != null)
            {
                UnityEngine.Profiling.Profiler.BeginSample("GLoaderEx.ClearContent");
                _clearContentFunc.Invoke(this, _url);
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public void OnLoadFromPackageFinish(string url)
        {
            LoadFromPackage(url);
            UpdateGear(7);
        }

        public void OnLoadExternalTextureFinish(Texture2D tex)
        {
            onExternalLoadSuccess(new NTexture(tex));
            UpdateGear(7);
        }
    }
}
