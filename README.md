# Read me

[TOC]

## Bomb_AR

Bomb_AR is an AR platform built as Unity project with some C# script for a bomb diffusion game. In this document, I will explain the structures of AR program and the TCP connection between AR and Server in details.  

**Note**: please use git to download and use Unity to initialize. 

## Installation and initialization

#### download

Clone the AR into local address.

`git clone -b Aolei_bomb_AR 地址`

Open the download directory in Unity.

Open `Bomb_AR/Server_simulation.py` in PyCharm.

#### run

Run `Server_simulation.py`.

Run Bomb_AR in Unity. After tutorial mode, please turn on server before step into the game mode. 

## Key Element in AR

### Connection Socket

Script:`GameObject TestSocket: TestSocket.cs`

TestSocket is the Hub of this project. It is in charge of **building TCP connection**, **sending commands** to server and **receiving messages**, **decoding messages** and invoking methods in other script, **recording the status** (mode = 0: tutorial mode; mode =1: game mode preparing before ready; mode =2: game mode playing), changing the **visibility** of UI controls. 

###  Popup

Almost every buttons in Popup windows has its own script, invoking methods from `TestSocket.cs` or changing the visibility of them self.

**Note**: I also use On Click() methods in Unity editor as well as using script to define functions triggered by buttons! 

+ popupTutorial: show up each turn in tutorial mode to decide stay in tutorial mode or start the game

  + text: Tutorial Part; button: [Start, Skip]

  + Script `GameObject NRCameraRig>CenterCamera>TutorialPanel`

+ popupReady: show up at the beginning of each turn in game mode. User can can choose ready to start the game. It also in charge of hinting explode/ diffuse/ time out information of last turn.

  + text: 
    + "Connecting Server ......": the AR is waiting for server's connection
    + "Are you ready": the AR is already connect to server and load in the terminal information of the first episode.
    + "DIFFUSED~": Player Diffuse the bomb.
    + "EXPLODED!!!": Player explode the bomb

  + button: [ready]: send message "ready" after press the button.

### Bomb

Bomb consist of right side terminals and left side terminals, buttons panel and timer.

+ Terminal: The cylinder that makes up the bomb
  + Script: `GameObject: DClockbomb_Aged>Terminals: TerminalsController.cs`
  + Structure: 6 terminals. Each terminal has left side and right side. Each side have two lines (upper line and lower line) which can show different colors.
  + Highlight: Each terminal can highlight in its left side and its right side.
  + Terminal indexing: [To do] 
+ Button panel:
  + Script `GameObject: DClockbomb_Aged>PbcBoard>UpperPane: UpperController.cs`
  + Structure: 6 buttons. 
  + Status: not selectable, selected, focused, highlight. Some buttons cannot be selected because they are previously selected. 
  + Button indexing: [To do]
+ Timer:
  + Script `GameObject: CountDown: CountDown.cs`
  + Only start to count at game mode. Timer doesn't work at tutorial mode and the time waiting for ready.

### Prompt box

There are three prompt board suspending in view. They are CanvasSpeaker, CanvasCommu, CanvasTest.

+ CanvasSpeaker: The board showing the content of speech of communication info, which is received from server.
  + Element: `GameObject: NRCameraRig>CenterCamera>CanvasSpeaker `
  + Script: `GameObject TestSocket: TestSocket.cs`
  + Text: It shows the content of the speech when AR receive communication information. Otherwise it shows "No instruction"

+ CanvasCommu: The board of two buttons let player request help (get communication info from server) or confirm his or hers selection.
  + Element:`GameObject: NRCameraRig>CenterCamera>CanvasCommu `
  + Script: `GameObject TestSocket: TestSocket.cs`
  + Buttons: [Help, Confirm]
  + When AR is transferring the confirmation of button selection to server and waiting for terminal information for next step, these two buttons are unclickable. 

+ CanvasTest: The board used to debug.
  + Element: `GameObject CanvasText`
  + Script: `GameObject TestSocket: TestSocket.cs`

### Text To Speech Interface

Script: `GameObject: TestToSpeech TextToSpeechController.cs`

I use **Azure** text-to-speech interface to convert text message into audio file, and then control the audio file into speech.

Note: if the token got from Azure expired, please re-apply.

reference tutorial: https://blog.csdn.net/Clovera/article/details/82705193

## Game Process

### Tutorial Mode

[to do]

## Format of Messages

### AR >>> Server

Script: `GameObject TestSocket: TestSocket.cs`

Use the first digit of the string to distinguish four types of message sent from AR.

+ "0 x": button selection confirmation. Request for the next episode (or end this turn). x is the index of button selection (from 0 to 5).

+ "1 x y": request for communication information (request for help).
  + x = 0: player cannot see the **left** side of terminals. x = 1: player can see the **left** side of terminals.
  + y = 0: player cannot see the **right** side of terminals. y=1: player can see the **right** side of terminals.

+ "9": Time out.
+ "ready": player start a new turn.

### Server >>> AR

Message has two basic types, terminal information (tdata) and communication information (cdata).

**Terminal information**: set colors of terminals and set colors (selectable or not selectable info) of buttons. It includes information type (T), left side terminal colors, right side terminal colors, button selectable status.

**Communication information**: offer help to AR/human players by highlighting terminal backgrounds button backgrounds and speech. It includes information type (T), left side terminal highlight, right side terminal highlight, button highlight.

+ Splitor of digits: $

+ Information type (T): 1-digit decimal number (for both terminal info and communication info)

  > -2: communication info 
  >
  > -1: terminal info, explode. Sent the next turn first episode terminal information after explode (this turn end).
  >
  > 0: terminal info, continue. Sent the next episode terminal information after player press the correct button but not finish this turn.
  >
  > 1: terminal info, diffuse. Sent the next turn first episode terminal information after  diffuse (this turn end).
  >
  > 2: terminal info, initialize. Send the first terminal information after creating the connection.
  >
  > 3: terminal info, time out. Send the next episode terminal information after time out and bomb explode (this turn end).

+ Example: terminal info

  ```
    T|    left side color    |   right side color    |    btn    |
    1|1 1|2 2|3 3|4 4|5 5|6 6|1 1|2 2|3 3|4 4|5 5|6 6|1 2 3 4 5 6|
  "-1$2$3$2$3$2$3$2$2$2$2$2$2$0$1$2$2$1$0$2$1$0$1$1$2$0$0$0$0$0$1"
  ```

  + T=-1 means it is a terminal info, sent after receiving AR error button selection which lead to bomb explosion. The rest of information means the initial set up (colors of terminals and buttons) of next turns.
  + Six pairs of colors indicates left side terminal patterns. The color codind are explained below.
  + Six button status 0-1 digit. 0: selectable. 1: not selectable.  

+ Example: communication info

  ```
   T|L highlight|R Highlight|B|  R text  # L text
   1|1 2 3 4 5 6|1 2 3 4 5 6|1|
  -2$1$0$1$0$0$1$0$0$0$1$0$0$0$left text#
  ```

  + T=-2 means it is a communication into, sent after receiving AR help request.
  + Six 0-1 digits of right side terminal highlight and six for left side. 0: default, 1: highlight.
  + One digit indicate the index of button to be highlight.

## Color coding

### Terminal colors and terminal background colors (left & right) 

Script position: `GameObject DClockbomb_Aged>Terminals`: **TerminalsController.cs** (Awake() methods, line 83)

Materials position: `Assets>Resources>Materials`

> Black (Background): no code, default (not highlight) 
>
> Yellow (Background): no code ,highlight  
>
> Red (Terminal element): 0
>
> Purple (Terminal element): 1
>
> Green (Terminal element): 2
>
> Orange (Terminal element): 3
>
> Blue (Terminal element): 4

### Button colors and button background colors

Script position: `GameObject DClockbomb_Aged>PbcBoard`:  **UpperController.cs** (private variable colors in class Button)

> Gray (Background): 0, default (not focused & not highlight / not selectable)
>
> Red (Background): 1, highlight
>
> Yellow (Background): no code, selected but not confirmed
>
> Green (Background): no code, focused (not selected)
>
> White (Button): 0, selectable 
>
> Black (Button): 1, not selectable

## Configuration

**Server host address, port**: `GameObject TestSocket: TestSocket.cs>Addr/port`

**Count down time**: `GameObject CountDown>CountDown.cs: TimeSetting`(in second)

**Text to speech Token**: `GameObject TextToSpeech: TextToSpeech.cs>Subkey/Region/ResourceName`

## Themes

Please refer to `Help` → `Custom Themes` from menu bar.

## Publish

Currently Typora only support to export as **PDF** or **HTML**. More data format support as import/export will be integrated in future.

## Auto Save and File Recovery

Typora support  auto save feature, user could enable it from preference panel. 

Typora does not provide professional version control and file backup feature, but typora would backup the last file content from time to time automatically, so even when typora crashes or users forget to save the file before close, it is possible to recovery most of the work by clicking `Recovery Unsaved Drafts` from preference folder, and copy out backed-up files. The File name in this folder is consists of last saved date, originally file name and last saved timestamp.

## More Useful Tips & Documents

<https://support.typora.io/>

## And More ?

For more questions or feedbacks, please contact us by:

- Home Page: http://typora.io
- Email: <hi@typora.io>
- Twitter [@typora](https://twitter.com/typora)

We opened a Github issue page in case you want to start a discussion or as an alternative way to report bugs/suggestions: https://github.com/typora/typora-issues/issues