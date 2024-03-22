# Machine Learning on Meadow

## Dataset

The Dataset is an essential part of performing machine learning. There are two options for acquiring a dataset:
- Create your own data set;
- Consume public and open datasets.

When creating your own data set, you must consider that the collection must follow a parameterization and replication that is closer to reality.

This repository is available on the App to assist in data collection as well as a small set of data for import into training scripts. Consult the `Data` directory and see which one best fits the project.

## Models

[TensorFlow Lite][1] allows you to use pre-trained models, and modify or create your own. These models can perform tasks such as: voice recognition, video, pattern recognition, text, and various applications.

## Traning

Building the model for gesture recognition in TensorFlow requires training with the dataset. To do this, use a [virtual machine][2] in colab and follow the flow of the cells.

The first step is the importance of the data and the separation between training and validation data.

Bellow shows the collected acceleration data.

![right](../Traning/Images/right_data.png "Acceleromter Data")

The second is neural network learning is carried out, after training there is a loss of Validation vs Training.

![Traning and Validation Loss](../Traning/Images/traning_vs_validation.png "Validation vs Traning")

The third step converts the model to the format expected by TensorFlow Lite.

```
const unsigned char model[] = {
  0x1c, 0x00, 0x00, 0x00, 0x54, 0x46, 0x4c, 0x33, 0x14, 0x00, 0x20, 0x00,
  0x1c, 0x00, 0x18, 0x00, 0x14, 0x00, 0x10, 0x00, 0x0c, 0x00, 0x00, 0x00,
  0x08, 0x00, 0x04, 0x00, 0x14 ....
}
```

[1]: https://www.tensorflow.org/lite/models
[2]: https://colab.research.google.com/drive/1ZjYB2r6U4jTtcKxQGC0esBYEMOv2YJ5f?usp=sharing