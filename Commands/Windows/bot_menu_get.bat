REM your bot token
SET BotToken=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX

REM menu scope for private chats only
SET "Scope={\"type\":\"all_private_chats\"}"

curl --get https://api.telegram.org/bot%BotToken%/getMyCommands --data-urlencode scope="%Scope%"
