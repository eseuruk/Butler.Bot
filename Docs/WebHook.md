# Bot WebHook

* To set bot [webhook](https://core.telegram.org/bots/api#setwebhook) and switch to push mode

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM your secret token for X-Telegram-Bot-Api-Secret-Token header
SET SecretToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM your aws lamda function url
SET WebHook=https://XXXXXXXXXXXXXXXXXXXXXXXX.lambda-url.XXXXXXXXXX.on.aws/update

curl -X POST https://api.telegram.org/bot%BotToken%/setWebhook -d "{\"url\": \"%WebHook%\", \"secret_token\":\"%SecretToken%\"}" -H "Content-Type: application/json"
```

* To see the current bot webhook 

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl https://api.telegram.org/bot%BotToken%/getWebhookInfo
```

* To delete bot webhook and switch to pull mode

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl https://api.telegram.org/bot%BotToken%/deleteWebhook?drop_pending_updates=true
```


