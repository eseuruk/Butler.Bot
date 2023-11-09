@ECHO OFF

REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl "https://api.telegram.org/bot%BotToken%/getWebhookInfo"
