#! /bin/sh

# your bot token
BotToken='XXXXXXXXXXXXXXXXXXXXXXXXXXXXX'

curl "https://api.telegram.org/bot$BotToken/getWebhookInfo"
echo
