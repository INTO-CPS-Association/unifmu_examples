# Unity FMU example of an Incubator


The FMU is divided into the typical folders and files: _binaries_, _resources_, and _modelDescription.xml_.
The resources folder contains the Unity project used to generate the Unity application for the FMU, see the UnityProject folder.
The compiled executable is named _FMU_Unity_Tutorial.exe_. This is the executable that is run, as defined in the _launch.toml_ file.

The Unity project is configured in such a way that allows testing the Unity project in the Unity editor, if the _Backend Dummy_ script is chosen on the Incubator object. If the _Backend_ script is chosen, then the project is configured to run as an FMU.

Note: This FMU can currently only be run on windows. 

