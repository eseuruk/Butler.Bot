REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl https://api.telegram.org/bot%BotToken%/deleteWebhook?drop_pending_updates=true
