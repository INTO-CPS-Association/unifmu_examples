using System;
using Fmi2Proto;
using NetMQ.Sockets;
using Google.Protobuf;
using NetMQ;
using UnityEngine;
using System.Threading;


  
public class Backend : MonoBehaviour {
    
    private Thread _listenerWorker;
    bool done = false;
    // private readonly ConcurrentQueue<Fmi2Command> _messageRecvQueue = new ConcurrentQueue<Fmi2Command>();
    // private ConcurrentQueue<IMessage> _messageSendQueue = new ConcurrentQueue<IMessage>();
    public Model model;
    private int application_exit_code = 0;

    void Listener()
    {
              Console.WriteLine("Called RUN() in backend");
            RequestSocket socket = new RequestSocket();
              string dispatcher_endpoint = System.Environment.GetEnvironmentVariable("UNIFMU_DISPATCHER_ENDPOINT");
              if (dispatcher_endpoint == null)
              {
                Console.Error.WriteLine("Environment variable 'UNIFMU_DISPATCHER_ENDPOINT' is not set in the current enviornment.");
                application_exit_code = -1;
                return;
              }


              socket.Connect(dispatcher_endpoint);
              Console.WriteLine("Connected to socket {0}", dispatcher_endpoint);
              IMessage message;
              message = new Fmi2ExtHandshakeReturn();
              socket.SendFrame(message.ToByteArray(), false);
              Fmi2Command command;
              
              while (!done)
              {
                command = Fmi2Command.Parser.ParseFrom(socket.ReceiveFrameBytes());
                Console.WriteLine("received command " + command);

                 switch (command.CommandCase)
        {
            case Fmi2Command.CommandOneofCase.Fmi2SetupExperiment:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2SetupExperiment(
                    command.Fmi2SetupExperiment.StartTime,
                    command.Fmi2SetupExperiment.HasStopTime ? (double?)command.Fmi2SetupExperiment.StopTime : null,
                    command.Fmi2SetupExperiment.HasTolerance ? (double?)command.Fmi2SetupExperiment.Tolerance : null
                );
                message = result;
            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2EnterInitializationMode:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2EnterInitializationMode();
                message = result;
            }

            break;

            case Fmi2Command.CommandOneofCase.Fmi2ExitInitializationMode:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2ExitInitializationMode();
                message = result;
            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2DoStep:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2DoStep(command.Fmi2DoStep.CurrentTime, command.Fmi2DoStep.StepSize, command.Fmi2DoStep.NoStepPrior);
                message = result;
            }

            break;

            case Fmi2Command.CommandOneofCase.Fmi2SetReal:
            {
                var result = new Fmi2StatusReturn();
                result = new Fmi2StatusReturn();
                result.Status = model.Fmi2SetReal(command.Fmi2SetReal.References, command.Fmi2SetReal.Values);
                message = result;
            }
            break;


            case Fmi2Command.CommandOneofCase.Fmi2SetInteger:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2SetInteger(
                    command.Fmi2SetInteger.References,
                    command.Fmi2SetInteger.Values);
                message = result;

            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2SetBoolean:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2SetBoolean(
                    command.Fmi2SetBoolean.References,
                    command.Fmi2SetBoolean.Values
                );
                message = result;


            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2SetString:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2SetString(command.Fmi2SetString.References, command.Fmi2SetString.Values);
                message = result;
            }
            break;


            case Fmi2Command.CommandOneofCase.Fmi2GetReal:
            {
                var result = new Fmi2GetRealReturn();
                (var status, var values) = model.Fmi2GetReal(command.Fmi2GetReal.References);
                result.Values.AddRange(values);
                result.Status = status;
                message = result;
            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2GetInteger:
            {
                var result = new Fmi2GetIntegerReturn();
                (var status, var values) = model.Fmi2GetInteger(command.Fmi2GetInteger.References);
                result.Values.AddRange(values);
                result.Status = status;
                message = result;

            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2GetBoolean:
            {
                var result = new Fmi2GetBooleanReturn();
                (var status, var values) = model.Fmi2GetBoolean(command.Fmi2GetBoolean.References);
                result.Values.AddRange(values);
                result.Status = status;
                message = result;

            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2GetString:
            {
                var result = new Fmi2GetStringReturn();
                (var status, var values) = model.Fmi2GetString(command.Fmi2GetString.References);
                result.Values.AddRange(values);
                result.Status = status;
                message = result;

            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2CancelStep:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2CancelStep();
                message = result;

            }

            break;

            case Fmi2Command.CommandOneofCase.Fmi2Reset:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2Reset();
                message = result;

            }

            break;

            case Fmi2Command.CommandOneofCase.Fmi2Terminate:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2Terminate();
                message = result;
            }
            break;

            case Fmi2Command.CommandOneofCase.Fmi2ExtSerializeSlave:
            {
                var result = new Fmi2ExtSerializeSlaveReturn();
                (var status, var state) = model.Fmi2ExtSerialize();
                result.Status = status;
                result.State = ByteString.CopyFrom(state);
                message = result;
            }
            break;
            case Fmi2Command.CommandOneofCase.Fmi2ExtDeserializeSlave:
            {
                var result = new Fmi2StatusReturn();
                result.Status = model.Fmi2ExtDeserialize(command.Fmi2ExtDeserializeSlave.State.ToByteArray());
                message = result;
            }

            break;

            case Fmi2Command.CommandOneofCase.Fmi2FreeInstance:
            {
                Console.WriteLine("received fmi2FreeInstance, exiting with status code 0");
                application_exit_code = 0;
                socket.Dispose();
                done = true;                     
            }


            break;

            default:
            Console.Error.WriteLine("unrecognized command {0}, exiting with status code -1", command.CommandCase);
            application_exit_code = -1;
            done = true;
            break;
        }

        socket.SendFrame(message.ToByteArray(), false);
              
        }
    }


    void Start()
    {
        _listenerWorker = new Thread(Listener);
        _listenerWorker.Start();
    }

    void Update()
    {
        if (done)
        {
            Console.WriteLine("QUITTING UNITY APPLICATION");
            NetMQConfig.Cleanup();
            _listenerWorker.Join();
            Console.WriteLine("Joined and cleaned APPLICATION");

            Application.Quit(application_exit_code);
            Console.WriteLine("application not shut down");
          
        }
            
    }
}
