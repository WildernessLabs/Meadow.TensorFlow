# Meadow.Tensorflow
Welcome to the Meadow TensorFlow Library. This library provides a .NET application using the [TensorFlow Lite Micro](https://www.tensorflow.org/lite/microcontrollers), a powerful tool using machine learning on microcontrollers. The Meadow TensorFlow implements the most common Tensorflow API in C# which allows .NET developers to develop and deploy Machine Learning models on Meadow Devices.

# Workflow

The following steps are required to run the Meadow TensorFlow Library.
  - Write the Tensorflow DLL on the development board. You may find on `Tensorflow/Tensorflow.so`
   ```bash
  meadow file write -f Tensorflow.so
  ```
  - Build the Meadow application
  - Deploy

# Demos
 ## Hello World
   Demonstrate the absolute basics of using TensorFlow. This demo will predict the output of the sine function.

 ## Magic Wand
   Recognize gestures using machine learning to analyze accelerometer data.

# Troubloubes

If by chance the Tensorfloow DLL isn't in the file system this message should appear:

```
Meadow StdOut: PlatformOS Initialize starting...
Meadow StdOut: System.DllNotFoundException: Tensorflow.so assembly:<unknown assembly> type:<unknown type> member:(null)
Meadow StdOut: at (wrapper managed-to-native) Tensorflow.litemicro.c_api_lite_micro.TfLiteMicroGetModel(int,intptr,intptr)
Meadow StdOut: at MeadowApp.MeadowApp.Initialize () [0x000a3] in <aeed14d41b00478398903db563ecff6a>:0
Meadow StdOut: at Meadow.MeadowOS.Start (System.String[] args, Meadow.IApp app) [0x000a9] in <8c25465e4ef54a56afc8b98d0018204a>:0
Meadow StdOut: App shutting down
Meadow StdOut: Shutdown
```
