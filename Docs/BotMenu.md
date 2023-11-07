# Bot Menu

If you want bot command menu to be visable in the private chat, please follow the instructions bellow.

![Bot.Menu](Images/Bot.Menu.png)

* To make bot menu visable in the private chat

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl -X POST https://api.telegram.org/bot%BotToken%/setMyCommands -H "Content-Type: application/json" -d "{\"commands\":[{\"command\":\"start\",\"description\":\"join the group\"}, {\"command\":\"leave\",\"description\":\"leave the group\"}], \"scope\":{\"type\":\"all_private_chats\"}}" 
```

* To delete private chat menu 

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl https://api.telegram.org/bot%BotToken%/deleteWebhook?drop_pending_updates=true
```

* To see the current settings of private chat menu 
 
```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl https://api.telegram.org/bot%BotToken%/getMyCommands?scope=%%7B%%22type%%22%%3A%%22all_private_chats%%22%%7D
```
