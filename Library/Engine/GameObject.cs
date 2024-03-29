﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class Layer
{
    public readonly ulong ID = 0;
    public Layer(ulong id)
    {
        ID = id;
    }

    public string Name
    {
        get
        {
            return InternalCalls.GetLayerName(ID);
        }
    }
    public static bool operator ==(Layer lhs, Layer rhs) => lhs.Equals(rhs);
    public static bool operator !=(Layer lhs, Layer rhs) => !lhs.Equals(rhs);
    public bool Equals(Layer rhs) => ID.Equals(rhs.ID);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is Layer && Equals((Layer)obj);
    }
    
    public override int GetHashCode()
    {
        unchecked
        {
            return (ID.GetHashCode()) * 397;
        }
    }
}

public class GameObject
{
    const ulong NIL = 0;
    public readonly ulong ID = 0;
    public GameObject(ulong e)
    {
        ID = e;
    }

    public Layer Layer
    {
        get
        {
            ulong layerid = InternalCalls.GetEntityLayerID(ID);
            return new Layer(layerid);
        }
    }

    public GameObject Parent
    {
        get
        {
            ulong parentID = InternalCalls.GetEntityParentID_Native(ID);
            if (parentID != 0)
            {
                return new GameObject(parentID);
            }
            else
            {
                return null;
            }
        }
    }

    public GameObject GetChild(int index)
    {
        ulong childID = InternalCalls.GetEntityChildID_Native(ID, index);
        if (childID != 0)
        {
            return new GameObject(childID);
        }
        else
        {
            return null;
        }
    }

    public int GetChildrenCount()
    {
        return InternalCalls.GetEntityChildrenCount_Native(ID);
    }

    public List<GameObject> GetChildren()
    {
        if (GetChildrenCount() == 0)
            return new List<GameObject>();

        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < GetChildrenCount(); ++i)
        {
            GameObject child = GetChild(i); // should not be null here; has at least 1 child
            children.Add(child); // although its possible to add null; list count increases
        }
        return children;
    }

    public string Name
    {
        get
        {
            return InternalCalls.GetEntityNameByID_Native(ID);
        }
    }

    public void EnableEntity(ulong ent = NIL)
    {
        if (ent == NIL)
            InternalCalls.EnableDisable_Native(ID, true);
        else
            InternalCalls.EnableDisable_Native(ent, true);
    }

    public void DisableEntity(ulong ent = NIL)
    {
        if (ent == NIL)
            InternalCalls.EnableDisable_Native(ID, false);
        else
            InternalCalls.EnableDisable_Native(ent, false);
    }

    /**
        * Checks if Component Exists in Current Entity
        * 
        * 
        * \return bool
        */
    public bool HasComponent<T>() where T : Component, new()
    {
        return InternalCalls.HasComponent_Native(ID, typeof(T));
    }
    /**
        * Checks if Component Exists in An Entity
        * 
        * \param ent
        * \return smth
        */
    public bool HasComponent<T>(ulong ent) where T : Component, new()
    {
        return InternalCalls.HasComponent_Native(ent, typeof(T));
    }

    public int GetComponentsCount<T>() where T : Component, new()
    {
        return InternalCalls.GetComponentsCount_Native(ID, typeof(T));
    }

    public T AddComponent<T>() where T : Component, new()
    {
        if (!HasComponent<T>())
        {
            InternalCalls.AddComponent_Native(ID, typeof(T));
            T component = new T();
            component.entity = ID;

            // index of newly added component via script would have index of current count
            component.index = GetComponentsCount<T>();

            return component;
        }
        return null;
    }

    // disabling for now as im not seeing its use yet
    /*public T AddComponent<T>(ulong ent) where T : Component, new()
    {
        if (!HasComponent<T>(ent))
        {
            InternalCalls.AddComponent_Native(ent, typeof(T));
            T component = new T()
            {
                entity = ent
            };
            return component;
        }
        return null;
    }*/

    public T GetComponent<T>(int idx = 0) where T : Component, new()
    {
        if (HasComponent<T>())
        {
            // index boundary check
            if (idx >= GetComponentsCount<T>())
            {
                throw new IndexOutOfRangeException("Component index out of range!");
            }
            else
            {
                T component = new T();
                component.entity = ID;
                component.index = idx;
                return component;
            }
        }

        return null;
    }

    public bool HasScriptComponent<T>()
    {
        // send name of script to script system to check
        string scriptName = typeof(T).ToString();
        return InternalCalls.HasScriptComponent_Native(ID, scriptName);
    }

    public T GetScriptComponent<T>()
    {
        if (HasScriptComponent<T>())
        {
            // return reference of the instance of the script component
            string scriptName = typeof(T).ToString();
            object scriptObj = InternalCalls.GetScriptComponent_Native(ID, scriptName);
            return (T)scriptObj;
        }

        return default(T);
    }

    public static T GetGlobalScriptComponent<T>()
    {
        // return reference of the instance of the script component
        string scriptName = typeof(T).ToString();
        object scriptObj = InternalCalls.GetGlobalScriptComponent_Native(scriptName);
        return (T)scriptObj;
    }

    // disabling as its params are conflicting with above get component function
    /*public T GetComponent<T>(ulong ent, int idx = 0) where T : Component, new()
    {
        if (HasComponent<T>(ent))
        {
            T component = new T()
            {
                entity = ent,
                index = idx
            };
            return component;
        }
        return null;
    }*/

    public Transform transform
    {
        get
        {
            return GetComponent<Transform>();
        }
    }

    public static GameObject Instantiate(GameObject original)
    {
        ulong id = InternalCalls.CreateEntity_Native(original != null ? original.ID : 0);

        if (id != 0)
        {
            return new GameObject(id);
        }
        else
        {
            Debug.Log("Failure to instantiate a GameObject");
            return null;
        }
    }

    public static GameObject InstantiateUI(GameObject original)
    {
        ulong id = InternalCalls.CreateEntityUI_Native(original != null ? original.ID : 0);

        if (id != 0)
        {
            return new GameObject(id);
        }
        else
        {
            Debug.Log("Failure to instantiate a UI GameObject");
            return null;
        }
    }

    public static GameObject InstantiatePrefab(string prefab)//, bool isUI = false)
    {
        ulong id = InternalCalls.CreateEntityPrefab_Native(prefab);

        if (id != 0)
        {
            return new GameObject(id);
        }
        else
        {
            Debug.Log("Attempted to instantiate an Invalid Prefab: " + prefab);
            return null;
        }
    }

    public static void Destroy(GameObject obj)
    {
        InternalCalls.DestroyEntity_Native(obj.ID);
    }

    public static GameObject Find(string name)
    {
        ulong id = InternalCalls.GetEntityIDByName_Native(name);

        if (id != 0)
        {
            return new GameObject(id);
        }
        else
        {
            Debug.Log("Attempted to get an Invalid Name: " + name);
            return null;
        }
        
    }
}