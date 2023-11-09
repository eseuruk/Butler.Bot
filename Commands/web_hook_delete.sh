#! /bin/sh

# your bot token
BotToken='XXXXXXXXXXXXXXXXXXXXXXXXXXXXX'

curl "https://api.telegram.org/bot$BotToken/deleteWebhook?drop_pending_updates=true"
echo
