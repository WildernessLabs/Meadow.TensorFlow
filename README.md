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
  
  The expected output of the Hello World demo.
   ```
  Meadow StdOut: Running App
  Meadow StdOut: 0 - (0, 0)
  Meadow StdOut: 1 - (0.3141593, 0.3727683)
  Meadow StdOut: 2 - (0.6283185, 0.5591524)
  Meadow StdOut: 3 - (0.9424779, 0.8387287)
  Meadow StdOut: 4 - (1.256637, 0.9658087)
  Meadow StdOut: 5 - (1.570796, 1.042057)
  Meadow StdOut: 6 - (1.884956, 0.9573368)
  Meadow StdOut: 7 - (2.199115, 0.8217847)
  Meadow StdOut: 8 - (2.513274, 0.5337364)
  Meadow StdOut: 9 - (2.827433, 0.2372162)
  Meadow StdOut: 10 - (3.141593, 0.008472007)
  Meadow StdOut: 11 - (3.455752, -0.3049923)
  Meadow StdOut: 12 - (3.769912, -0.5337364)
  Meadow StdOut: 13 - (4.08407, -0.7794246)
  Meadow StdOut: 14 - (4.39823, -0.9658087)
  Meadow StdOut: 15 - (4.712389, -1.109833)
  Meadow StdOut: 16 - (5.026548, -0.9827528)
  Meadow StdOut: 17 - (5.340708, -0.7455366)
  Meadow StdOut: 18 - (5.654867, -0.5337364)
  Meadow StdOut: 19 - (5.969026, -0.3558243)
  Meadow StdOut: TensorFlow completed
   ```
 ## Magic Wand
   Recognize gestures using machine learning to analyze accelerometer data.

# Troubloubes

If by chance the Tensorflow DLL isn't in the file system this message should appear:

```
stdout> Initializing App
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> Initialize Tensor Flow ...
stdout> Mono: DllImport unable to load library 'Tensorflow.so'.
stdout> App Error in App Initialize: Tensorflow.so assembly:<unknown assembly> type:<unknown type> member:(null)
stdout>  System.DllNotFoundException: Tensorflow.so assembly:<unknown assembly> type:<unknown type> member:(null)
stdout>   at (wrapper managed-to-native) Tensorflow.litemicro.c_api_lite_micro.TfLiteMicroGetModel(int,intptr,intptr)
  at MeadowApp.MeadowApp.Initialize () [0x000b5] in <fd8ba0d23bc8457baefb5100962ea831>:0 
  at Meadow.MeadowOS.Start (System.String[] args, Meadow.IApp app) [0x000a9] in <ffa488067a224532bb9ed3ec47e94b92>:0 
stdout> App shutting down
stdout> Shutdown
```
