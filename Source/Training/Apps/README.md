# Application
This section share some applications to assist users in collecting data on sensors using the ProjLab.

## Gesture Detector & Magic Wang
Those applications need the BMI270, a motion sensor that has an accelerometer, and gyroscope, to recognize the movement.

### Workflow
- The routine of application was controlled by to button on ProjLab.
    - `Up Button` : Start the trainning
    -  `Down Button` : Stop the training
    -  `Left Button`: Restart current training
    -  `Right Button`: Save current training

- Recorded the gestures you must extract the data into device and move to the computer, using the command:
    ```
    $ meadow read file your_file.txt ./your_file.txt
    ``` 
- Reset your device by pressing the `RST` button and start collect again. We recommend reproducing this proceed 10 time and copy and paste to `your_gesture.csv` file.

- Collected and organized each gesture data it's time to move to [Colab Machine][1] and start training your model.

>[NOTE]
  The Gesture App, for each gesture collected you have a limited buffer to recording the movement, so take your mind a short movements.

[1]: https://colab.research.google.com/drive/1ZjYB2r6U4jTtcKxQGC0esBYEMOv2YJ5f?usp=sharing