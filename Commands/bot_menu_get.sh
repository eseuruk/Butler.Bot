#! /bin/sh

# your bot token
BotToken='XXXXXXXXXXXXXXXXXXXXXXXXXXXXX'

# menu scope for private chats only
Scope='{"type":"all_private_chats"}'

curl --get "https://api.telegram.org/bot$BotToken/getMyCommands" --data-urlencode scope="$Scope"
echo