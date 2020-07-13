using SLua;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

[CustomLuaClass]
public static class DontDestroyOnLoadManager
{
    private class GameObjectHolder
    {
        public int priority = 0;
        public GameObject go = null;
        public string name = "";
    }

    static List<GameObjectHolder> _ddolObjects = new List<GameObjectHolder>();
    static Dictionary<GameObject, Action> _map = new Dictionary<GameObject, Action>();

    public static void DontDestroyOnLoad(this GameObject go)
    {
        DontDestroyOnLoadInner(go, 0, null);
    }

    public static void DontDestroyOnLoad(this GameObject go, int priority = 0)
    {
        DontDestroyOnLoadInner(go, priority, null);
    }

    public static void DontDestroyOnLoad(this GameObject go, Action on_destory = null)
    {
        DontDestroyOnLoadInner(go, 0, on_destory);
    }

    public static void DontDestroyOnLoad(this GameObject go, int priority = 0, Action on_destory = null)
    {
        DontDestroyOnLoadInner(go, priority, on_destory);
    }

    private static void DontDestroyOnLoadInner(this GameObject go, int priority = 0, Action on_destory = null) //on_destory主要用来定制处理静态变量的状态
    {  
        UnityEngine.Object.DontDestroyOnLoad(go);

        GameObjectHolder holder = new GameObjectHolder();
        holder.go = go;
        holder.priority = priority;
        holder.name = go.name;

        _ddolObjects.Add(holder);
        _ddolObjects.Sort((a, b) => {
            if (a.priority < b.priority)
                return -1;
            else if (a.priority > b.priority)
                return 1;
            else
                return 0;
        });

        if (on_destory != null)
        {
            _map[go] = on_destory;
        }
    }

    public static IEnumerator DestroyAll() {

        List<Action> _action = new List<Action>();
        foreach (var holder in _ddolObjects)
        {
            if (holder != null)
            {
                string name = holder.name;

                //Debug.LogError(String.Format("Destroy{0} priority {1}", name, holder.priority));

                UnityEngine.Object.Destroy(holder.go);

                Action act;
                if (_map.TryGetValue(holder.go, out act))
                {
                    _action.Add(act);
                }

                while (GameObject.Find(name) != null)
                {
                    yield return null;
                }
            }
        }

        foreach (Action act in _action)
        {
            act();
        }

        _action.Clear();
        _ddolObjects.Clear();
        _map.Clear();

        yield return null;
    }
}