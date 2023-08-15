# Butler.Bot
Welcome to **Butler.Bot**, generic bot which is built to help people joining private groups describing themselves with #whois messages.

Standard telegram invite links work well but require effort from administrators to review each join request manually. So **Butler.Bot** is specially designed to interact with people and add them to the group only when proper information is provided.

Let’s imagine you have a private group with invite link like: https://t.me/GenericButlerBot If you click on it and follow bot instructions you will be smoothly added to Butler.Bot Discussion group without any manual intervention from administrators.

[ [Feel free to try](https://t.me/GenericButlerBot). Group is specially created to discuss **Buttler.Bot** behaviour and configuration ]

![Butler of London Yuppies](/Docs/Images/Butler.Bot.png)

## Invite link

**Butler.Bot** is not able to add people to **Target Group** because of Bot API limitations. Instead, it uses procreated invite link with approval which is shared with the new member each time #whois is written.

That is the reason why bot needs to be added to your **Target Group** with administrator privileges. It allows bit to monitor all join requests and approve only from people who provided #whois messages. 

## Administrator Group

**Butler.Bot** supports dedicated administrator group where all new members are reported after join. Members of this group might review new joiners and remove some of them if #whois messages are not good enough.

There is also an option for manual review #whois messages before join. Please see [bot configuration](Docs/Configuration.md) for more details.

## How to use
Bot is not multitenant. At least for now. Each instance of the bot is configured to work with one particular **Target Group**. It means you need to run the instance of bot yourself.

Likely we know how to do it for free. Please see [detailed instructions](Docs/BotCreation.md).

## Technology
Bot is written in C# using [.Net 6 LTS](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). Code is fully managed and use [Telegram.Bot library](https://github.com/TelegramBots/Telegram.Bot) to interact with [Telegram Bot API](https://core.telegram.org/bots/api). 

## Serverless
Bot is originally designed to be serverless. It currently supports AWS and might be easy ported to any other cloud provider.

### Butler.Bot.AWS
Serverless version of the bot which uses [AWS Lambda](https://aws.amazon.com/lambda) as an execution engine and [AWS DynamoDB](https://aws.amazon.com/dynamodb) as a state storage.
Both of them have free plan propositions and might be used for bot deployment.

![Butler.Bot.AWS](/Docs/Images/Butler.Bot.AWS.png)

### Butler.Bot.Local
Traditional pull model is also supported and currently used in development. SQLite is a default database for local runs.

![Butler.Bot.Local](/Docs/Images/Butler.Bot.Local.png)

## Configuration
Configuration is file based and very simple. Please see [full description](Docs/Configuration.md).

## Messaging
All bot messages are overridable via configuration. Please see [configuration](Docs/Configuration.md) and the source code.



