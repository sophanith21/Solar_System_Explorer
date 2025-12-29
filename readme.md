# Solar System Explorer

**An educational, adventurous, third-person unity game project built for CADT** _Year 3, Term 1: Fundamentals of Game Development_

## 1. Project Overview

- **Pitch:** The game introduces two mode to play, the learning mode, and the exploration where the player can apply what they've learned.
- **Core Loop:** The player starts by learning about the solar system, then go into the exploration mode where a mission is assigned. The mission is for the player to control a spaceship from a planet to another planet. If for some reason the player spaceship is destroyed, then it's a game over. If the player reaches the destination planet, they will win.
- **Platform:** PC

## 2. Gameplay Demo

[Youtube Link](https://youtu.be/SMZ_vr5wFz8)

## 3. Key Features

- **Solar System Simulation:** Using real world AU of each planet in the solar system with real world orbital inclination to produce a simulation that is as scientific accurate as **possible**.
- **Learn & Play:** The player can learn about the order and the characterisitcs of each planet then test their knowledge in the exploration mode where they have to know those facts in order to know what they are currently at and where they will have to go to reach the destination planet.
- **Optimized Gameplay:** Many optimization are done such as lighting layer mask, object pooling, etc. Low-end devices in today's standard can expect to run this game without a problem.

## 4. Technical Implementation

- **Architecture:** Singleton patterns (Audio Manager, Scene Manager, etc are created once and used in all scenes).
- **Tools Used:** Cinemachine
- **External Assets:**
  - Spaceship : [Hi-Rez Spaceships Creator Free Sample](https://assetstore.unity.com/packages/3d/vehicles/space/hi-rez-spaceships-creator-free-sample-153363)
  - Planets
    - 3D : [Planets of the Solar System 3D](https://assetstore.unity.com/packages/3d/environments/planets-of-the-solar-system-3d-90219)
    - 2D : [Planet Icons](https://assetstore.unity.com/packages/2d/gui/icons/planet-icons-176807)
  - Skybox : [Milky Way Skybox](https://assetstore.unity.com/packages/2d/textures-materials/milky-way-skybox-94001)
  - Particles :
    - Portal : [Magic Effects FREE](https://assetstore.unity.com/packages/vfx/particles/spells/magic-effects-free-247933)
    - Thruster Trailing : [Particle Pack](https://assetstore.unity.com/packages/vfx/particles/particle-pack-127325)
  - BGM :
    - Learning Mode : [Free - Sci-Fi and Cyberpunk Music Pack](https://assetstore.unity.com/packages/audio/ambient/sci-fi/free-sci-fi-and-cyberpunk-music-pack-264590)
    - Exploration Mode : [Sci Fi Ambiances](https://assetstore.unity.com/packages/audio/ambient/sci-fi/sci-fi-ambiances-234344)

## 5. Development Process

- **The Challenge:** To create an accurate lighting (the sun as the only source of light for all planets), we need to use the point light and positioned it in the center of the sun. But this method is very computationally expensive, if we were to implement it, most if not all low-end devices will struggle to run the game.
- **The Solution:** Switch from point light to directional light. We create one directional light for each planet. This light source sits between the planet and the sun and look from the sun direction to the planet. With this, we were able to get an accurate lighting and shadows of the all the planets as they revolve around the sun. We also use culling mask so each planet's directional light only affects itself.

## 6. Installation & Controls

### How to Run

1. Download the latest build from the [Releases](https://www.google.com/search?q=link-to-github-releases) page.
2. Run `Solar System Explorer.exe` for Windows and `Solar System Explorer` for Mac.

### Controls

- **Please refer to "How To Play" menu provided inside the game**

## 7. Credits & Feedback

- **Developers:** MEAS Sophanith, MUONG Gek Heang, MAO Sothyda, PICH Sokreaksa, HOEURNG Monica
- **Course:** Fundamentals of Game Development
- **Instructor:** Dr. VA Hongly
