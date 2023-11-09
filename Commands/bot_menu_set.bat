@ECHO OFF

REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM bot menu commands
SET "Commands=[{\"command\":\"start\",\"description\":\"join the group\"}, {\"command\":\"leave\",\"description\":\"leave the group\"}, {\"command\":\"version\",\"description\":\"bot version\"}]"

REM menu scope for private chats only
SET "Scope={\"type\":\"all_private_chats\"}"

curl "https://api.telegram.org/bot%BotToken%/setMyCommands" --header "Content-Type: application/json" --data "{\"commands\":%Commands%, \"scope\":%Scope%}" 
