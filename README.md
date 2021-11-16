
# Deep Reinforcement Learning for Autonomous Racing in a Customizable Virtual Environment


We present a customizable enviroment to train ml agents for autonomus racing (or autonomous driving in general).

The enviroment uses Unity3D and the [ML-Agents](https://github.com/Unity-Technologies/ml-agents) plugin to train the agents.

Right now it only supports SAC and PPO. But you can run other agents if you provide the right config files manually rather than editing them through the application.




## Abstract

The main objective of this thesis is the implementation of a system that will provide us 
the ability to train two State of the Art algorithms in Deep Reinforcement Learning as 
well as the application of those algorithms for the training of autonomous driving agents 
that require to minimize the lap time of a circuit track, in an environment that can be 
build from building blocks that we will provide in our application. Our methodology’s 
goal is to build a neural network that is able to navigate a racing car autonomously 
without prior knowledge of the vehicle’s dynamics while minimizing the lap time of a 
given track. We achieve that by providing the controller with the knowledge of its 
actions space and its vector observation space. Hence, we will analyze and study the two 
algorithms, PPO and SAC according to their performance, as well as compare it with a 
sample of human-drivers through different experiments. Those experiments will show 
us the ability of the algorithms to minimize the lap time as well as their robustness in 
different situations, like different tracks that they weren’t trained before or different 
vehicle settings. Our experiments show that both algorithms have their strengths and 
weaknesses. Specifically, PPO seems to be more timid in its driving, whereas SAC is able 
to achieve performance similar to our human-drivers sample, but it’s not as stable in 
maps that it wasn’t trained in.

## Demo

[YouTube Video](https://studio.youtube.com/video/GfbtzLSpfwo/edit)

You can also run it for yourself here : [Final Release](https://github.com/McFatcat/CarsML2021-main/releases/tag/MainBuild)
or here if you want to just drive it yourselft [Testing Builds](https://github.com/McFatcat/CarsML2021-main/releases/tag/DataCollection)
## Authors

- [@NickCh](https://www.github.com/nickch1996)
- [@McFatcat](https://www.github.com/McFatcat)

  
## Roadmap

- Full ml algorithm support

- Make it easier to add assets to map editor

  
