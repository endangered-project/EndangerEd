# 🌱 EndangerEd Project

---

## 🕹️ EndangerEd

### 🔍 Overview
EndangerEd is the game client of the EndangerEd platform. It is the main method of interacting with the video game aspects of the platform.
This includes playing the game, viewing scores, and viewing statistics summary.

### 📟 Technical Details
The game client uses osu!framework as its framework.

The game can be segmented into three parts.

#### 🏠 Pre-game
When booting up the client, before the player can start playing the game, they will be greeted with the menu. 
The menu lets the player do the following:
- Start playing the video game.
- View the scoreboard.
- Adjust the client's settings.
- Authorization and authentication (login, signup, logout).
- Access the knowledge base.
#### 👾 Gameplay
The gameplay involves completing microgames one after another until the game is over. For the condition to be met, the player's 
lives must be depleted or the game is ended prematurely by the player via pressing the end button. The player is given three lives in total, 
which acts as a three-strikes system. A life is removed when the player fails a microgame or skip a microgame. 
The former happens when they choose the wrong answer or the time runs out, while the latter happens when they press the skip button.
On the other hand, a player is able to gain points by completing microgames. To keep the gameplay engaging, the game to increases
the difficulty the more microgames are completed. It does this by decreasing the time limit for completing each microgame and introducing
harder microgames to the game rotation.
#### 📝 Post-game
After the game is over, if the player is logged in, the scores will be sent to the server registration to the scoreboard. The player is able
to view their statistics summary and the scoreboard showing their ranking on the post-game interface. They can then choose to either retry
or quit to the main menu after they finished viewing the information.

### 📥 Installation

This project required .NET 8.0 to run. You can download it [here](https://dotnet.microsoft.com/download/dotnet).

1. Clone the repository to your machine using the following command:
```
git clone https://github.com/endangered-project/EndangerEd.git
```
2. Use Visual Studio or an equivalent IDE to open the solution.

### Deploying

We use [Velopack](https://github.com/velopack/velopack) (former Clowd.Sqirrel) to deploy the game client.

Windows

```powershell
cd EndangerEd.Desktop
dotnet tool update -g vpk
dotnet publish -c Release --self-contained -r win-x64 -o .\publish
vpk pack -u EndangerEd -v 1.0.0 -p .\publish -e "EndangerEd.exe"
```

MacOS

```bash
cd EndangerEd.Desktop
dotnet tool update -g vpk
dotnet publish -c Release --self-contained -r osx-x64 -o ./publish  # or osx-arm64
vpk pack -u EndangerEd -v 1.0.0 -p ./publish -e "EndangerEd"
```

### 📊 Progression
See [GitHub Project](https://github.com/orgs/endangered-project/projects/1/) ([or the old board](https://github.com/users/HelloYeew/projects/8/views/2)).

---

## 🧭 Navigation

#### &emsp;&emsp;&emsp; [📚 Shīdo - Platform's Knowledge Base](https://github.com/HelloYeew/shido)

#### &emsp;&emsp;&emsp; [🗄️ Gameserver - Platform's Server](https://github.com/endangered-project/gameserver)

#### &emsp;&nbsp;&nbsp;👉 🕹️ EndangerEd - Platform's Client