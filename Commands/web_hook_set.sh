#! /bin/sh

# your bot token
BotToken='XXXXXXXXXXXXXXXXXXXXXXXXXXXXX'

# your secret token for X-Telegram-Bot-Api-Secret-Token header
SecretToken='XXXXXXXXXXXXXXXXXXXXXXXXXXXXX'

# your aws lamda function url
WebHook='https://XXXXXXXXXXXXXXXXXXXXXXXX.lambda-url.XXXXXXXXXX.on.aws/update'

curl "https://api.telegram.org/bot$BotToken/setWebhook" --header "Content-Type: application/json" --data "{\"url\":\"$WebHook\",\"secret_token\":\"$SecretToken\"}"
echo
