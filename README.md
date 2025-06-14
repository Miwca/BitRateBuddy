<img src="https://github.com/Miwca/BitRateBuddy/blob/main/images/bitratebuddy_logo.png?raw=true" width="100" height="100" align="right" />

# BitRateBuddy
**BitRateBuddy** is a complimentary service designed for IRL streamers using RTMP workflows. It runs locally to restream RTMP feeds and improve connection stability while integrating with [Streamer.bot](https://streamer.bot/) to trigger on-stream events such as filter toggling or live bitrate display.

## 🛠️ What It Does
- Observes the RTMP stream state from an NGINX server (working dockerfile included in project).
- Provides a lightweight WebSocket client that communicates with [Streamer.bot](https://streamer.bot/).
- Helps automate scene behavior, effects, and bitrate-related alerts based on live connection metrics.
- Meant for **local hosting** on either:
  - Your **streaming PC** (for tighter OBS integration).
  - Or a **dedicated server** hosting the NGINX RTMP container.

## ⚙️ Technologies
- **.NET 8** BackgroundService
- Configurable via `appsettings.json`
- Works with NGINX RTMP Docker setups
- Actions defined in [Streamer.bot](https://streamer.bot/)

## 📦 Setup
To get started, clone the repository and update the configuration in `appsettings.json`:

```json
  "StreamSettings": {
    "BaseUrl": "http://<RTMP_SERVER_IP>:1939",
    "StreamKey": "[KEY_IDENTIFIER]",
    "LowBitrateThreshold": 1000
  },
  "StreamerBotSettings": {
    "Websocket": {
      "BaseUrl": "ws://127.0.0.1:8080",
      "Password": "test"
    },
    "Http": {
      "BaseUrl": "http://127.0.0.1:7474"
    },
    "Actions": {
      "StreamLowBitrateAction": "[ACTION_ID_HERE]", // Leave empty if not applicable
      "StreamOfflineAction": "[ACTION_ID_HERE]", // Leave empty if not applicable
      "StreamHealthyAction": "[ACTION_ID_HERE]" // Leave empty if not applicable
    }
  }
```


## 🚀 Running the Project

### 🛰️ NGINX RTMP Server
This project includes a preconfigured `Dockerfile` for quickly launching an **NGINX RTMP server** compatible with BitRateBuddy. To get started:

1.  Navigate to the `nginx-rtmp-server` directory:
    `cd nginx-rtmp-server` 
2.  Build the Docker image:
    `sudo docker build -t miwca-rtmp-server .` 
3.  List available images to retrieve the image ID:
    `sudo docker image ls` 
4.  Start the container using the image ID (the container will automatically restart with the Docker engine):
    `sudo docker run -d --restart always -p 1935-1939:1935-1939 --name nginx-rtmp <image-id>` 
    
This will expose ports `1935–1939` for your RTMP ingest and restreaming needs.

----------

### ⚙️ Streamer.bot Integration
BitRateBuddy can connect to [Streamer.bot](https://streamer.bot/) to trigger actions based on the current state of your RTMP stream. You can define and configure these actions in Streamer.bot using the WebSocket integration.

#### Required Actions
Create the following actions (names are suggestions):
-   **Set RTMP Stream Healthy** – Triggered when the incoming bitrate is above the configured healthy threshold.
-   **Set RTMP Stream Unhealthy** – Triggered when the bitrate drops below the threshold.
-   **Set RTMP Stream Offline** – Triggered when the RTMP stream disconnects entirely.

Once your actions are defined, copy their **Action IDs** to be referenced in `appsettings.json`.
![Copy action ID](https://github.com/Miwca/BitRateBuddy/blob/main/images/streamer-bot_01_copy-action-id.png?raw=true)

> If you don’t wish to trigger specific actions for any of these events, you may leave their respective IDs blank in the configuration

> Note that the bitrate can be accessed with the variable ``%bitrate%``

## 🎥 Come Hang Out Live!
This entire contraption —**BitRateBuddy**— was forged in the fiery depths of my stream cave, specifically for my own chaotic IRL setup. Think of it as a watchdog gremlin for my RTMP pipeline, making sure the bitrate doesn't misbehave while I’m out in the world causing stream mayhem.

Wanna see it in action? Got questions? Wanna talk about .NET, docker gremlins, or the weird spaghetti that is IRL streaming tech?

Join the gremlin horde live on [**Twitch**](https://twitch.tv/miwca)!  
Or hop into our cozy dev chaos den over on [**Discord**](https://discord.gg/EYFwBzEeS4) — snacks and stream logs not included.
