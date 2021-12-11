using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class jsonDataClass
{
    public List<layersList> layers; 
}

[Serializable]
public class layersList
{
    public string type;
    public string path;
    public List<placementInfo> placement;
    public List<operationsInfo> operations;
}

[Serializable]
public class placementInfo
{
    public positionInfo position;
}

[Serializable]
public class positionInfo
{
   public int x;
   public int y;
   public int width;
   public int height;
}

[Serializable]
public class operationsInfo
{
   public string name;
   public string argument;
}