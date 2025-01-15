# SpawnDev.BlazorJS.TransformersJS
[![NuGet](https://img.shields.io/nuget/dt/SpawnDev.BlazorJS.TransformersJS.svg?label=SpawnDev.BlazorJS.TransformersJS)](https://www.nuget.org/packages/SpawnDev.BlazorJS.TransformersJS) 

# WIP - The API is currently limited. If you are interested in this project, please start an issue to suggest features or areas of interest.

## State-of-the-art Machine Learning for the Web in Blazor WebAssembly
SpawnDev.BlazorJS.TransformersJS brings the awesome Transformers.js library into Blazor WebAssembly apps.

Transformers.js is designed to be functionally equivalent to Hugging Face’s transformers python library, meaning you can run the same pretrained models using a very similar API. These models support common tasks in different modalities, such as:

📝 Natural Language Processing: text classification, named entity recognition, question answering, language modeling, summarization, translation, multiple choice, and text generation.
🖼️ Computer Vision: image classification, object detection, segmentation, and depth estimation.
🗣️ Audio: automatic speech recognition, audio classification, and text-to-speech.
🐙 Multimodal: embeddings, zero-shot audio classification, zero-shot image classification, and zero-shot object detection.

Transformers.js uses ONNX Runtime to run models in the browser. The best part about it, is that you can easily convert your pretrained PyTorch, TensorFlow, or JAX models to ONNX using 🤗 Optimum.

### Demo
The current demo app demos some features of Transformers.js, Blazor, and WebGL.

NOTE: The models used can be large. A fast connection is recommended.  

[Live Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.TransformersJS)  
[2D to 2D+Z](https://lostbeard.github.io/SpawnDev.BlazorJS.TransformersJS)  
[2D to Anaglyph](https://lostbeard.github.io/SpawnDev.BlazorJS.TransformersJS/AnaglyphImageDemo)  
