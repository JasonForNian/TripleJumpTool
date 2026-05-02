# 三段跳工具/TripleJumpTool

## 语言/Language
- [中文](#中文)
- [English](#english)

## 中文

# 三段跳(娇小玩偶)

该mod给*空洞骑士丝之歌*添加一个新的**黄色工具**，装备后可以使用三段跳。

---

## ✨ 功能

- 装备 **“娇小玩偶”** 后，在空中使用二段跳后可以再次按下跳跃键执行三段跳。
- 与原版移动机制无冲突，不影响冲刺、蹬墙跳等其他动作（下批，登墙不刷新三段跳bug已解决，二段跳冲刺时不能三段跳已解决）。
- 使用三段跳需要拥有二段跳和滑翔（若提前获取装备没这两个技能也不能增加一次跳跃）。
- 获取方式：第三幕触发后，前往费耶山山顶获取二段跳的琴前面，有一个可拾取的光点，拾取即可获得娇小玩偶。

---

## 📥 安装

1. 确保已安装 BepInEx 和 Needleforge
2. 将本模组文件夹解压至游戏目录下的 `BepInEx\plugins` 文件夹中。

---

## ⚙️ 配置

模组的参数可由mod文件夹内的 `TripleJump.json` 文件控制。你可以用任意文本编辑器修改其属性。

### 默认配置文件

```json
{
    "Name": "娇小玩偶",
    "Desc": "由天空飘落的羽毛编织的雪灵玩偶,可加强供奉者的能力。\n\n增加一次跳跃能力。",
    "Type": "Yellow",
    "Icon": "TripleJump.png",
    "MapID":"Peak_08b",
    "Position":[281,102],
    "state":"act3_wokeUp"
}
```

### 配置项说明

- `Name`：工具名称，可自定义。
- `Desc`：工具描述，可自定义。
- `Type`：工具类型，红，黄，蓝。
- `Icon`：工具图标，可自定义图标文件名，建议144x144像素左右，或者直接改我的图标（太小会显示一小个图标，太大没尝试，我是直接用的其他工具图标改的）。
- `MapID`：地图ID，在哪个地图生成改工具（Peak_08b为费耶山山顶）。
- `Position`：工具生成的坐标，在地图中的坐标（[281,102]为获取二段跳的琴的位置）。
- `state`：玩家状态，当玩家拥有该状态时才能触发（act3_wokeUp为第三幕开始后）。

## English

# Triple Jump (娇小玩偶)

This mod adds a new **yellow tool** to *Hollow Knight: Silksong*. Once equipped, you gain the ability to perform a **third jump** in the air.

---

## ✨ Features

- With the **"娇小玩偶"** equipped, press the jump button again after a double jump to execute a triple jump.
- No conflicts with the original movement system — dashes, wall jumps, and other actions are unaffected.  
  *(Fixed: wall grabs no longer incorrectly reset triple jump; fixed: triple jump is now usable right after a dash double jump.)*
- Triple jump requires both the **double jump** ability and the **glide** ability. (Acquiring the tool early won't grant an extra jump if these skills are missing.)
- Acquisition: After Act 3 begins, head to the top of **Mount Fay**, right in front of the harp that grants the double jump. A glowing pickup will appear — collect it to obtain the Petite Doll.

---

## 📥 Installation

1. Make sure **BepInEx** and **Needleforge** are installed.
2. Extract the mod folder into the game's `BepInEx\plugins` directory.

---

## ⚙️ Configuration

The mod’s parameters are controlled by the `TripleJump.json` file inside the mod folder. You can edit it with any text editor.

### Default configuration file

```json
{
    "Name": "娇小玩偶",
    "Desc": "由天空飘落的羽毛编织的雪灵玩偶,可加强供奉者的能力。\n\n增加一次跳跃能力。",
    "Type": "Yellow",
    "Icon": "TripleJump.png",
    "MapID":"Peak_08b",
    "Position":[281,102],
    "state":"act3_wokeUp"
}

```

### Configuration options

- `Name`：Tool name, can be customized.
- `Desc`：Tool description, can be customized.
- `Type`：Tool type, red, yellow, or blue.
- `Icon`：Tool icon, can be a custom icon file name, but it's recommended to be 144x144 pixels or larger, or you can replace it with my icon (too small will display a small icon, too large I haven't tried).
- `MapID`：Map ID, where to generate the tool (Peak_08b is the peak of Fay).
- `Position`：Tool position, in the map's coordinates ([281,102] is the position of the harp that grants the double jump).
- `state`：Player state, only when the player has this state can the tool be triggered (act3_wokeUp is the third act).
