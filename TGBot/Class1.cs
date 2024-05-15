using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using Telegram.Bot.Types.InputFiles;
using System.IO;
namespace TGBot
{
    class Program
    {  
        // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
        private static ITelegramBotClient _botClient;

        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions _receiverOptions;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("6914596505:AAERAgPqNp296VGflNahA3dGjv69uQHverk"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
            _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
            {
                AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
                {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
                // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
                // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
                ThrowPendingUpdates = true,
            };
            using var cts = new CancellationTokenSource();

            // UpdateHander - обработчик приходящих Update`ов
            // ErrorHandler - обработчик ошибок, связанных с Bot API
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

            var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }

     

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
                var message = update.Message;

                var user = message.From;

                Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                var chat = message.Chat;

                if (message.Text == "/start")
                {  
                    
               
                await botClient.SendTextMessageAsync(
                        chat.Id,
                        "Отправьте фото в формате документа"
                        );
                }
                
                
                if (message.Document != null)
                {
                   

                var fileId = update.Message.Document.FileId;
                    var fileInfo = await _botClient.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;
                    string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}\{message.Document.FileName}";

                    await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                    await _botClient.DownloadFileAsync(
                         filePath,
                         fileStream);
                fileStream.Close();

                using (StreamWriter sw = new StreamWriter(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt", true))
                {
                    await sw.WriteLineAsync(destinationFilePath);
                }
                var replyKeyboard = new ReplyKeyboardMarkup(
                   new List<KeyboardButton[]>()
                       {
                                                     new KeyboardButton[]
                                                     {
                                                         new KeyboardButton("Шум"), new KeyboardButton("Хром")
                                                      }
                                                     , new KeyboardButton[]
                                                     {
                                                         new KeyboardButton("Ч/Б"),  new KeyboardButton("Инверсия")
                                                     }

                       });

                await botClient.SendTextMessageAsync(
                        chat.Id,
                        "Выберите нужный фильтр",
                        replyMarkup: replyKeyboard);
                return;
                }

            if (message.Text == "Шум")
            {
                string readpath = @"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt";
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
                Console.WriteLine(destinationFilePath);
               
                    Process.Start(@"C:\Паскаль\NewTGBot\TGBot\Droplet2.exe", $@"""{destinationFilePath}""");
                await Task.Delay(15000);

                await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Edited_File.jpg"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }

            if (message.Text == "Хром")
            {
                string readpath = @"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt";
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
                Console.WriteLine(destinationFilePath);

                Process.Start(@"C:\Паскаль\NewTGBot\TGBot\Хром1.exe", $@"""{destinationFilePath}""");
                await Task.Delay(15000);

                await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Edited_File.jpg"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }

            if (message.Text == "Ч/Б")
            {
                string readpath = @"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt";
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
                Console.WriteLine(destinationFilePath);

                Process.Start(@"C:\Паскаль\NewTGBot\TGBot\Чб1.exe", $@"""{destinationFilePath}""");
                await Task.Delay(15000);

                await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Edited_File.jpg"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }

            if (message.Text == "Инверсия")
            {
                string readpath = @"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt";
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
                Console.WriteLine(destinationFilePath);

                Process.Start(@"C:\Паскаль\NewTGBot\TGBot\Инверсия.exe", $@"""{destinationFilePath}""");
                await Task.Delay(15000);

                await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Edited_File.jpg"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }

            if (message.Type != MessageType.Document && message.Type != MessageType.Photo)
            {    
                if (message.Text != "Шум" && message.Text != "Хром" && message.Text != "Ч/Б" && message.Text != "Инверсия" && message.Text != "/start")
                  {
                            await botClient.SendTextMessageAsync(
                            chat.Id,
                            "Введите корректную команду!"
                            );
                  }}

            

            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Отправить фото можно только в формате документа!"
                       );
            }
        }
    }
}
    

