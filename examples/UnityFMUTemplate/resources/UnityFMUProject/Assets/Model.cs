using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Fmi2Proto;

public class Model : MonoBehaviour
{
    public double x
    {
        get
        {
            return (double)cubeManipulator.transform.position.x;
        }
        set
        {
            float y = cubeManipulator.transform.position.y;
            float z = cubeManipulator.transform.position.z;
            cubeManipulator.transform.position = new Vector3((float)value, y, z);
        }
    }

    public double y
    {
      get
      {
        return (double)cubeManipulator.transform.position.y;
      }
      set
      {
        float x = cubeManipulator.transform.position.x;
        float z = cubeManipulator.transform.position.z;
        cubeManipulator.transform.position = new Vector3(x, (float)value, z);
      }
    }

    public double z
    {
      get
      {
        return (double)cubeManipulator.transform.position.z;
      }
      set
      {
        float x = cubeManipulator.transform.position.x;
        float y = cubeManipulator.transform.position.y;
        cubeManipulator.transform.position = new Vector3(x, y, (float)value);
      }
    }


    public CubeManipulator cubeManipulator;
    private Dictionary<uint, PropertyInfo> reference_to_attributes = new Dictionary<uint, PropertyInfo>();


    public Model()
    {
        Console.WriteLine("Unity FMU initialized");
        // Populate map from value reference to attributes of the model.
        string references_to_values = System.Environment.GetEnvironmentVariable("UNIFMU_REFS_TO_ATTRS");
        if (references_to_values == null)
        {
            Console.WriteLine("the environment variable 'UNIFMU_REFS_TO_ATTRS' was not set");
        }
        var dict = JsonConvert.DeserializeObject<Dictionary<uint, String>>(references_to_values);
        foreach (var item in dict)
        {
            this.reference_to_attributes.Add(item.Key, this.GetType().GetProperty(item.Value));
        }


    }
    public Fmi2Status Fmi2DoStep(double currentTime, double stepSize, bool noStepPrior)
    {
        UpdateOutputs();
        return Fmi2Status.Ok;
    }

    public Fmi2Status Fmi2SetupExperiment(double startTime, double? stopTime, double? tolerance)
    {
        return Fmi2Status.Ok;
    }

    public Fmi2Status Fmi2EnterInitializationMode()
    {
        return Fmi2Status.Ok;
    }

    public Fmi2Status Fmi2ExitInitializationMode()
    {
        this.UpdateOutputs();
        return Fmi2Status.Ok;
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
        return Fmi2Status.Ok;
    }

    public Fmi2Status Fmi2Reset()
    {
        return Fmi2Status.Ok;
    }

    public Fmi2Status Fmi2Terminate()
    {
        return Fmi2Status.Ok;
    }

    public (Fmi2Status, byte[]) Fmi2ExtSerialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
              writer.Write(x);
              writer.Write(y);
              writer.Write(z);
            }
            return (Fmi2Status.Ok, m.ToArray());
        }
    }

    public Fmi2Status Fmi2ExtDeserialize(byte[] state)
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
        return Fmi2Status.Ok;
    }

    private void UpdateOutputs()
    {

    }

    private Fmi2Status SetValueReflection<T>(IEnumerable<uint> references, IEnumerable<T> values)
    {
        List<uint> referencesList = references.ToList();
        List<T> valuesList = values.ToList();
        for (int i = 0; i < referencesList.Count; ++i)
        {
            this.reference_to_attributes[referencesList[i]].SetValue(this, (object)valuesList[i]);
        }

        return Fmi2Status.Ok;
    }

    private (Fmi2Status, IEnumerable<T>) GetValueReflection<T>(IEnumerable<uint> references)
    {

        var values = new List<T>(references.Count());

        foreach (var r in references)
        {
            values.Add((T)this.reference_to_attributes[r].GetValue(this));
        }

        return (Fmi2Status.Ok, values);
    }

}