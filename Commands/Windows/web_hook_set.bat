REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM your secret token for X-Telegram-Bot-Api-Secret-Token header
SET SecretToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM your aws lamda function url
SET WebHook=https://XXXXXXXXXXXXXXXXXXXXXXXX.lambda-url.XXXXXXXXXX.on.aws/update

curl https://api.telegram.org/bot%BotToken%/setWebhook --json "{\"url\":\"%WebHook%\", \"secret_token\":\"%SecretToken%\"}"
