using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Lichess.Bot
{
    internal class Program
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;

        static async Task Main(string[] args)
        {
            _botClient = new TelegramBotClient("");
            _receiverOptions = new ReceiverOptions 
            {
                AllowedUpdates = new[] //https://core.telegram.org/bots/api#update
                {
                    UpdateType.Message, 
                },
                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} is running!");

            await Task.Delay(-1);
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                                InlineKeyboardButton.WithUrl("Open Chess Bot", "https://your-webapp-url.com")
                            });
                            Console.WriteLine("Message received!");
                            if (update.Message != null && update.Message.Text == "/start")
                            {
                                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Click the button below to open the chess bot.");
                            }
                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}