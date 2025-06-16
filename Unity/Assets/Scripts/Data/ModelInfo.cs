using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class ModelInfo
{
    public string modelName;
    public string modelPath;
    public List<ClipInfo> clipList;
}
