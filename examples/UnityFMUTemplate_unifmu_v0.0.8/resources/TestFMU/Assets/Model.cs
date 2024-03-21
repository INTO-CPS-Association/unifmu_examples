using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Fmi2Messages;

public class Model : MonoBehaviour
{

    private double _x;
    private double _y;
    private double _z;
    public double x
    {
        get {return _x;}
        set { 
            if (value != _x)
            {
                _x = value;
            }
        }
        // get
        // {
        //     return cubeManipulator.transform.position.x;
        // }
        // set
        // {
        //     float y = cubeManipulator.transform.position.y;
        //     float z = cubeManipulator.transform.position.z;
        //     cubeManipulator.transform.position = new Vector3((float)value, y, z);
        // }
    }

    public double y
    {
        get {return _y;}
        set {if (value != _y)
            {
                _y = value;
            }}
    //   get
    //   {
    //     return cubeManipulator.transform.position.y;
    //   }
    //   set
    //   {
    //     float x = cubeManipulator.transform.position.x;
    //     float z = cubeManipulator.transform.position.z;
    //     cubeManipulator.transform.position = new Vector3(x, (float)value, z);
    //   }
    }

    public double z
    {
        get {return _z;}
        set {if (value != _z)
            {
                _z = value;
            }}
    //   get
    //   {
    //     return cubeManipulator.transform.position.z;
    //   }
    //   set
    //   {
    //     float x = cubeManipulator.transform.position.x;
    //     float y = cubeManipulator.transform.position.y;
    //     cubeManipulator.transform.position = new Vector3(x, y, (float)value);
    //   }
    }


    public CubeManipulator cubeManipulator;
    private Dictionary<uint, PropertyInfo> reference_to_attributes = new Dictionary<uint, PropertyInfo>();


    public Model()
    {
        Console.WriteLine("Unity FMU initialized");

        // Populate map from value reference to attributes of the model. 
        this.reference_to_attributes = new Dictionary<uint, PropertyInfo>
        {
            { 0, this.GetType().GetProperty("x") },
            { 1, this.GetType().GetProperty("y") },
            { 2, this.GetType().GetProperty("z") },
        };
    }

    private void GetVariables()
    {
        _x = cubeManipulator.transform.position.x;
        _y = cubeManipulator.transform.position.y;
        _z = cubeManipulator.transform.position.z;
    }

    void SetVariables()
    {
        cubeManipulator.transform.position = new Vector3((float)_x, (float)_y, (float)_z);
    }

    void Start()
    {
        GetVariables();
    }

    // Only used when testing in Unity Editor with BackendDummy
    void Update()
    {
        SetVariables();
        GetVariables();
    }

    public Fmi2Status Fmi2DoStep(double currentTime, double stepSize, bool noStepPrior)
    {
        UpdateOutputs();
        return Fmi2Status.Fmi2Ok;
    }

    public Fmi2Status Fmi2SetupExperiment(double startTime, double? stopTime, double? tolerance)
    {
        return Fmi2Status.Fmi2Ok;
    }

    public Fmi2Status Fmi2EnterInitializationMode()
    {
        return Fmi2Status.Fmi2Ok;
    }

    public Fmi2Status Fmi2ExitInitializationMode()
    {
        this.UpdateOutputs();
        return Fmi2Status.Fmi2Ok;
    }

    public Fmi2Status Fmi2SetReal(IEnumerable<uint> references, IEnumerable<double> values)
    {
        return this.SetValueReflection(references, values);
    }

    public Fmi2Status Fmi2SetInteger(IEnumerable<uint> references, IEnumerable<int> values)
    {
        return this.SetValueReflection(references, values);
    }

    public Fmi2Status Fmi2SetBoolean(IEnumerable<uint> references, IEnumerable<bool> values)
    {
        return this.SetValueReflection(references, values);
    }

    public Fmi2Status Fmi2SetString(IEnumerable<uint> references, IEnumerable<string> values)
    {
        return this.SetValueReflection(references, values);

    }

    public (Fmi2Status, IEnumerable<double>) Fmi2GetReal(IEnumerable<uint> references)
    {
        return this.GetValueReflection<double>(references);
    }

    public (Fmi2Status, IEnumerable<int>) Fmi2GetInteger(IEnumerable<uint> references)
    {
        return this.GetValueReflection<int>(references);
    }

    public (Fmi2Status, IEnumerable<bool>) Fmi2GetBoolean(IEnumerable<uint> references)
    {
        return this.GetValueReflection<bool>(references);
    }

    public (Fmi2Status, IEnumerable<String>) Fmi2GetString(IEnumerable<uint> references)
    {
        return this.GetValueReflection<String>(references);
    }

    public Fmi2Status Fmi2CancelStep()
    {
        return Fmi2Status.Fmi2Ok;
    }

    public Fmi2Status Fmi2Reset()
    {
        return Fmi2Status.Fmi2Ok;
    }

    public Fmi2Status Fmi2Terminate()
    {
        return Fmi2Status.Fmi2Ok;
    }

    public (Fmi2Status, byte[]) Fmi2SerializeFmuState()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
              writer.Write(x);
              writer.Write(y);
              writer.Write(z);
            }
            return (Fmi2Status.Fmi2Ok, m.ToArray());
        }
    }

    public Fmi2Status Fmi2DeserializeFmuState(byte[] state)
    {
        using (MemoryStream m = new MemoryStream(state))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
              x = (float)reader.ReadDouble();
              y = (float)reader.ReadDouble();
              z = (float)reader.ReadDouble();
            }
        }
        return Fmi2Status.Fmi2Ok;
    }

    private void UpdateOutputs()
    {
        // SetVariables();
    }

    private Fmi2Status SetValueReflection<T>(IEnumerable<uint> references, IEnumerable<T> values)
    {
        // SetVariables();
        List<uint> referencesList = references.ToList();
        List<T> valuesList = values.ToList();
        for (int i = 0; i < referencesList.Count; ++i)
        {
            this.reference_to_attributes[referencesList[i]].SetValue(this, (object)valuesList[i]);
        }
        return Fmi2Status.Fmi2Ok;
    }

    private (Fmi2Status, IEnumerable<T>) GetValueReflection<T>(IEnumerable<uint> references)
    {
        // GetVariables();
        var values = new List<T>(references.Count());

        foreach (var r in references)
        {
            values.Add((T)this.reference_to_attributes[r].GetValue(this));
        }

        return (Fmi2Status.Fmi2Ok, values);
    }

}