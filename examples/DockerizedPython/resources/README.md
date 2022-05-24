**This FMU was generated using UniFMU.
For general instructions on how to use the tool access the repository https://github.com/INTO-CPS-Association/unifmu**

# Implementing the model

The `resources/model.py` file defines the functional relationship between inputs and outputs of the FMU.

## Declaring inputs and outputs

By default, each input, output or parameter declared in the `modelDescription.xml` file is represented as attributes on the instance of the `Model` class.
For instance if a variable `a` is declared in the `modelDescription.xml` file, it an attribute of the same name should be declared in the `Model` class:

```xml
<ScalarVariable name="a" valueReference="0" variability="continuous" causality="input">
    <Real start="0.0" />
</ScalarVariable>
```

```python
def __init__(self) -> None:
    self.a = 0.0

    self.reference_to_attribute = {
        0: "a",
    }
```

The FMI C-API uses numerical indices rather than names to which variables to read or write to.
As such a mapping between the indices declared by the `valueReference` attribute of the xml and the attributes must be defined.
By default the mapping between a value reference and its corresponding Python attribute is defined by adding an entry to the `reference_to_attributes` variable of the `Model` class.

## Defining the behavior

The `Model` class declares several methods that can be used to define the behavior of the FMU.
Methods prefixed with `fmi2` mirror the methods declared in the C-API defined by the FMI specification.

For instance, to update an output `b` to be twice the value of `a` the `fmi2DoStep` method could be defined as:

```python
def fmi2DoStep(self, current_time, step_size, no_step_prior):
    self.b = self.a * 2
    return Fmi2Status.ok
```

# Testing and debugging the model

The `model.py` is _plain_ Python code, which means we can test the model using test cases and debugging tools.
A small test program can be written and placed in the `model.py` as seen below:

```python
if __name__ == "__main__":
    m = Model()

    assert m.a == 0.0
    assert m.b == 0.0

    m.a = 1.0
    m.fmiDoStep(0.0, 1.0, False)

    assert m.b == 2.0
```

The program can be executed in your IDE with or from the command line by running the `resources/model.py` script.

# Runtime dependencies

The environment that invokes the Python code must provide all the dependencies, otherwise the simulation will fail when instantiating or simulation the model.
For instance, if the `resources/model.py` imports a third-party package such as `numpy`

```python
import numpy as np
```

this must also be available to the Python interpreter specified by the `launch.toml` file, in this case the system's `python3` interpreter:

```toml
linux = ["python3", "backend.py"]
```

One way to address a missing dependency is to install using package manager such as `pip`

```
python3 -m pip install numpy
```

**Any Python FMU generated UniFMU requires the `protobuf` package.
The easiest way to install this is using pip:**

```
python3 -m pip install protobuf
```

# File structure

An overview of the role of each file is provided in the tree below:

```python
📦model
 ┣ 📂binaries
 ┃ ┣ 📂darwin64
 ┃ ┃ ┗ 📜unifmu.dylib       # binary for macOS
 ┃ ┣ 📂linux64
 ┃ ┃ ┗ 📜unifmu.so          # binary for Linux
 ┃ ┗ 📂win64
 ┃ ┃ ┗ 📜unifmu.dll         # binary For Windows
 ┣ 📂resources
 ┃ ┣ 📂schemas
 ┃ ┃ ┗ 📜unifmu_fmi2_pb2.py # schema defining structure of messages sent over RPC
 ┃ ┣ 📜backend.py           # receives messages and dispatched function calls to "model.py"
 ┃ ┣ 📜launch.toml*         # specifies command used to start FMU
 ┃ ┗ 📜model.py*            # implementation of FMU
 ┗ 📜modelDescription.xml*  # definition of inputs and outputs
```

\* denotes files that would typically be modified by the implementor of the FMU

# Dockerized FMU

## How to install dependencies in the container?

Dependencies can be installed through the `Dockerfile`. 
If, for instance, your model requires python packages to be installed, this is the place to do it.
You can either install them directly by adding: `pip install {some_package, some_other_package}`
or by providing a `requirements.txt` file and adding
```Dockerfile
COPY common/requirements.txt /tmp/requirements.txt
RUN pip install -r /tmp/requirements.txt
```
to the `Dockerfile`.

## When is the container rebuilt?

Per default the containers for one FMU (identified by its `GUID`) are retained and reused to minimize overhead.
If you want the container to be recreated you can add a flag to the launch command in the `launch.toml`, like so:
```
linux = ["docker-compose", "run", "--rm", "python", "backend" ]
``` 

## When does the image rebuilt?

By default the image is only build once, which means that updates to the FMU's resources on the host machine are not automatically updated inside the image.
Two strategies for rebuilding the image are:

### 1. Manually delete the existing container and image through docker.
### 2. Automatially rebuild the image on every invocation
One way to do this is to have docker build the image before it is run.
To do this, you can change the `launch.toml` such that the launch command invokes a shell script 
```
linux = ["bash", "rebuild.sh"]
```
that contains the two `docker-compose` commands:
```bash
docker-compose build backend
docker-compose run backend python backend.py
```
There are other ways to handle automatic rebuilding through docker e.g. through using `docker-compose up`, for more information see:
[docker-compose reference](https://docs.docker.com/compose/reference/).


## Networking

Setting `--net=host` when the container is run, shares the network between container and host.
All communication on the host is available within the container as well.
So, from the host's perspective, the dockerized FMU is equivalent to the native version.

### Linux

No additional changes are required for Linux.

### Windows, OSx

To distinguish the `localhost` address of the host from the `localhost` of the container, the special alias `host.docker.internal` is used to replace occurences of `127.0.0.1` or `localhost` in the handshake endpoint
In this way the backend running in the container knows to connect to the host's localhost and not the containers localhost.

Consequently, the endpoint has to be adapted accordingly;
the substitution is carried out by the `deploy.py` script invoked when the container is started.

```python
    endpoint = os.environ["UNIFMU_DISPATCHER_ENDPOINT"]
    dispatcher_endpoint = os.environ["UNIFMU_DISPATCHER_ENDPOINT"].replace(
        "127.0.0.1", "host.docker.internal"
    )
    os.environ["UNIFMU_DISPATCHER_ENDPOINT"] = dispatcher_endpoint

    subprocess.call(["python", "backend.py"])
```
