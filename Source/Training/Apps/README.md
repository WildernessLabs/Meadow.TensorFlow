# Application
This section shares some applications to assist users in collecting data on sensors using the ProjLab.

## Gesture Detector & Magic Wang
Those applications need the BMI270, a motion sensor that has an accelerometer, and gyroscope, to recognize the movement.

### Workflow
- The routine of the application was controlled by to button on ProjLab.
    - `Up Button`: Start the training
    -  `Down Button`: Stop the training
    -  `Left Button`: Restart current training
    -  `Right Button`: Save current training

- Record the gestures you must extract the data into the device and move to the computer.
    ```
    $ meadow read file your_file.txt ./your_file.txt
    ``` 
- Reset your device by pressing the `RST` button and start collect again. We recommend reproducing this procedure 10 times and copy and paste to `your_gesture.csv` file.

- Collect and organize each gesture data it's time to move to [Colab Machine][1] and start training your model.

>[!NOTE]
>  The Gesture App, for each gesture collected you have a limited buffer to recording the movement, so take your mind a short movements.

[1]: https://colab.research.google.com/drive/1ZjYB2r6U4jTtcKxQGC0esBYEMOv2YJ5f?usp=sharing
