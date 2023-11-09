# Bot WebHook

Windows and Linux shell scripts are available in [/Commands](../Commands) folder.

* To set bot [webhook](https://core.telegram.org/bots/api#setwebhook) and switch to push mode

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM your secret token for X-Telegram-Bot-Api-Secret-Token header
SET SecretToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM your aws lamda function url
SET WebHook=https://XXXXXXXXXXXXXXXXXXXXXXXX.lambda-url.XXXXXXXXXX.on.aws/update

curl "https://api.telegram.org/bot%BotToken%/setWebhook" --header "Content-Type: application/json" --data "{\"url\":\"%WebHook%\",\"secret_token\":\"%SecretToken%\"}"
```

* To see the current bot webhook 

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl "https://api.telegram.org/bot%BotToken%/getWebhookInfo"
```

* To delete bot webhook and switch to pull mode

```bat
REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

curl "https://api.telegram.org/bot%BotToken%/deleteWebhook?drop_pending_updates=true"
```
