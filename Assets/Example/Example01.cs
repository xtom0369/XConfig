using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XConfig;

public class Example01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Config.Inst = new Config();
        Config.Inst.Init();

        var rows = Config.Inst.itemsTable.rows;
        foreach (var row in rows)
        {
            DebugUtil.LogError(row.Name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
