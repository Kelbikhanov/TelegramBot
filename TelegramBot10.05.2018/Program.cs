using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using ApiAiSDK;
using ApiAiSDK.Model;

namespace TelegramBot10._05._2018
{
    class Program
    {
        static TelegramBotClient Bot;
        static ApiAi ApiAi;
        private static ApiAi apiAi;

        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("552315064:AAHyOUtEI40osyVG3RvYZdPhFriL09WGIuU");
            AIConfiguration config = new AIConfiguration ("775e1777c7cc40a3977f65b1e12e9d69" , SupportedLanguage.Russian);
            apiAi = new ApiAi(config);

            Bot.OnMessage += BotOnMessageReceived;

            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;


            var me = Bot.GetMeAsync().Result;
            Console.WriteLine(me.FirstName);

            Bot.StartReceiving();

            Console.ReadLine();

            Bot.StopReceiving();
        }

        private static async void BotOnCallbackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.FirstName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");

            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"вы нажали кнопку {buttonText}");
        }

        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;// создаем переменную Message 

            if (message == null || message.Type != MessageType.TextMessage)


                return ;

            string name = $"{message.From.FirstName}  {message.From.LastName}";

            Console.WriteLine($"{name } отправил сообщение :'{ message.Text}'"); // такой то пользователь отправил такое то сообщение 

            switch (message.Text) // если текст сообщения равен команде старт , то будут выполняться определенные дейстия 
            {

                case "/start":
                    string text =
                        @"список команд:
                        /start - список команд
                        /inline- вывод меню 
                        /keyboard - вывод клавиатуры ";
                  await   Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/inline":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("VK", "https://vk.com/id345147153") ,
                            InlineKeyboardButton.WithUrl("Vk2", "https://vk.com/id177186771")

                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Пункт 1 "),
                            InlineKeyboardButton.WithCallbackData("Пункт 2 ")

                        }

                    });
                    await Bot.SendTextMessageAsync(message.From.Id , "Выберите пункт меню", 
                        replyMarkup: inlineKeyboard);
                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Привет"),
                            new KeyboardButton("Как дела ?")
                        },
                        new[]
                        {
                            new KeyboardButton("Контакт") {RequestContact = true },
                            new KeyboardButton("Геолокация") {RequestLocation = true}
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Сообщение", replyMarkup: replyKeyboard);

                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                        answer = "прости я тебя не понял";
                    await Bot.SendTextMessageAsync(message.From.Id, answer);

                    break;



            }

        }
    }
}