#! /bin/sh

# your bot token
BotToken='XXXXXXXXXXXXXXXXXXXXXXXXXXXXX'

# bot menu commands
Commands='[{"command":"start","description":"join the group"}, {"command":"leave","description":"leave the group"}, {"command":"version","description":"bot version"}]'

# menu scope for private chats only
Scope='{"type":"all_private_chats"}'

curl "https://api.telegram.org/bot$BotToken/setMyCommands" --header 'Content-Type: application/json' --data "{\"commands\":$Commands, \"scope\":$Scope}"
echo