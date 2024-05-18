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
using System.Drawing;
using System.Runtime.Intrinsics.X86;
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
            var correct_numbers = new List<string>();
            for (var i = 1; i <= 255; i++)
            {
                correct_numbers.Add(i.ToString());
            }

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
                                                         new KeyboardButton("Контраст"), new KeyboardButton("Сепия")
                                                      }
                                                     , new KeyboardButton[]
                                                     {
                                                         new KeyboardButton("Ч/Б")
                                                     }

                       });

                await botClient.SendTextMessageAsync(
                        chat.Id,
                        "Выберите нужный фильтр",
                        replyMarkup: replyKeyboard);
                return;
                }

            if (message.Text == "Контраст")
            {
                using (StreamWriter sw = new StreamWriter(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile2.txt", true))
                {
                    await sw.WriteLineAsync($"{message.Text}");
                    sw.Close();
                }
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Задайте параметр уровня контрастности (от 1 до 255)"
                       );
            }

            if (message.Text == "Сепия")
            {
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
                Console.WriteLine(destinationFilePath);
                Bitmap bmp2 = new Bitmap(destinationFilePath);
                for (int i = 0; i < bmp2.Width; i++)
                    for (int j = 0; j < bmp2.Height; j++)
                    {
                        Color clr = bmp2.GetPixel(i, j);
                        var oldRed = clr.R;
                        var oldGreen = clr.R;
                        var oldBlue = clr.R;

                        var sepiaRed = Convert.ToInt32(0.393 * oldRed + 0.769 * oldGreen + 0.189 * oldBlue);
                        var sepiaGreen = Convert.ToInt32(0.349 * oldRed + 0.686 * oldGreen + 0.168 * oldBlue);
                        var sepiaBlue = Convert.ToInt32(0.272 * oldRed + 0.534 * oldGreen + 0.131 * oldBlue);

                        if (sepiaRed > 255)
                        {
                            sepiaRed = 255;
                        }

                        if (sepiaGreen > 255)
                        {
                            sepiaGreen = 255;
                        }

                        if (sepiaBlue > 255)
                        {
                            sepiaBlue = 255;
                        }
                        clr = Color.FromArgb(clr.A, sepiaRed, sepiaGreen, sepiaBlue);
                        bmp2.SetPixel(i, j, clr);
                    }

                bmp2.Save(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png", System.Drawing.Imaging.ImageFormat.Png);

                await Task.Delay(5000);

                await using Stream stream = System.IO.File.OpenRead(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png");
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Sepia_File.png"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }

            //if (message.Text == "Размытие")
            //{
            //    using (StreamWriter sw = new StreamWriter(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile2.txt", true))
            //    {
            //        await sw.WriteLineAsync($"{message.Text}");
            //        sw.Close();
            //    }
            //    await botClient.SendTextMessageAsync(
            //           chat.Id,
            //           "Задайте параметр уровня размытия (от 1 до 3)"
            //           );
            //}

            if (message.Text == "Ч/Б")
            {
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
                Console.WriteLine(message.Text);
                Bitmap bmp = new Bitmap(destinationFilePath);
                for (int x = 0; x < bmp.Width; x++)
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color clr = bmp.GetPixel(x, y);
                        int r = clr.R;
                        int g = clr.G;
                        int b = clr.B;
                        int avg = (r + g + b) / 3;
                        Color setpix = Color.FromArgb(avg, avg, avg);
                        bmp.SetPixel(x, y, setpix);
                    }
                bmp.Save(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png", System.Drawing.Imaging.ImageFormat.Png);

                await Task.Delay(5000);

                await using Stream stream = System.IO.File.OpenRead(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png");
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Black_File.png"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }



            if (correct_numbers.Contains(message.Text) && System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile2.txt").Last() == "Контраст")
            {
                string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
               // Console.WriteLine(destinationFilePath);
                Console.WriteLine(message.Text);
                var number_of_contrast = int.Parse(message.Text);

                Bitmap bmp2 = new Bitmap(destinationFilePath);
                for (int i = 0; i < bmp2.Width; i++)
                    for (int j = 0; j < bmp2.Height; j++)
                    {
                        Color clr = bmp2.GetPixel(i, j);
                        double C = ((100.0 + number_of_contrast - 100) / 100) * ((100.0 + number_of_contrast - 100) / 100);

                        double temp = ((((clr.R / 255.0) - 0.5) * C) + 0.5) * 255.0;
                        int nr = (int)temp;
                        temp = ((((clr.R / 255.0) - 0.5) * C) + 0.5) * 255.0;
                        int ng = (int)temp;
                        temp = ((((clr.R / 255.0) - 0.5) * C) + 0.5) * 255.0;
                        int nb = (int)temp;

                        if (nr < 0) { nr = 0; }
                        if (nr > 255) { nr = 255; }
                        if (ng < 0) { ng = 0; }
                        if (ng > 255) { ng = 255; }
                        if (nb < 0) { nb = 0; }
                        if (nb > 255) { nb = 255; }

                        bmp2.SetPixel(i, j, Color.FromArgb(clr.A, nr, ng, nb));

                    }

                bmp2.Save(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png", System.Drawing.Imaging.ImageFormat.Png);

                await Task.Delay(5000);

                await using Stream stream = System.IO.File.OpenRead(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png");
                await _botClient.SendDocumentAsync(
                        chat.Id,
                        new InputOnlineFile(stream, "Contrast_File.png"));
                await botClient.SendTextMessageAsync(
                       chat.Id,
                       "Для продолжения работы отправьте следующее изображение"
                       );
            }

            //TODO

            //if (correct_numbers.Contains(message.Text) && System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile2.txt").Last() == "Размытие")
            //{
            //    string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
            //    Console.WriteLine("ghbdtn");
            //    int red = 0, green = 0, blue = 0, blurPixelCount = 0;
            //    var number_of_blur = int.Parse(message.Text);

            //    Bitmap bmp2 = new Bitmap(destinationFilePath);
                
            //    for (int i = number_of_blur; i < bmp2.Width - number_of_blur; i++)
            //        for (int j = number_of_blur; j < bmp2.Height - number_of_blur; j++)
            //        {
            //            Color clr = new Color();
            //            for (int k = i - number_of_blur; k <= i + number_of_blur; k++)
            //                for (int l = j - number_of_blur; l <= j + number_of_blur; l++)
            //                {
            //                    red += bmp2.GetPixel(k, l).R;
            //                    green += bmp2.GetPixel(k, l).G;
            //                    blue += bmp2.GetPixel(k, l).B;

            //                    blurPixelCount++;
            //                }
            //            red = red / blurPixelCount;
            //            green = green / blurPixelCount;
            //            blue = blue / blurPixelCount;
            //            clr = Color.FromArgb(red, green, blue);
            //            bmp2.SetPixel(i, j, clr);
            //            red = 0;
            //            green = 0;
            //            blue = 0;
            //            blurPixelCount = 0;
            //        }

            //    bmp2.Save(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png", System.Drawing.Imaging.ImageFormat.Png);

            //    await Task.Delay(10000);

            //    await using Stream stream = System.IO.File.OpenRead(@"C:\Паскаль\NewTGBot\TGBot\TGBot\Doc.png");
            //    await _botClient.SendDocumentAsync(
            //            chat.Id,
            //            new InputOnlineFile(stream, "Blur_File.png"));
            //    await botClient.SendTextMessageAsync(
            //           chat.Id,
            //           "Для продолжения работы отправьте следующее изображение"
            //           );
            //}

            //////////// if i want to start it using droplets

            //if (message.Text == "Ч/Б")
            //{
            //    string readpath = @"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt";
            //    string destinationFilePath = System.IO.File.ReadLines(@"C:\Паскаль\NewTGBot\TGBot\TGBot\TextFile1.txt").Last();
            //    Console.WriteLine(destinationFilePath);

            //    Process.Start(@"C:\Паскаль\NewTGBot\TGBot\Чб1.exe", $@"""{destinationFilePath}""");
            //    await Task.Delay(15000);

            //    await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
            //    await _botClient.SendDocumentAsync(
            //            chat.Id,
            //            new InputOnlineFile(stream, "Edited_File.jpg"));
            //    await botClient.SendTextMessageAsync(
            //           chat.Id,
            //           "Для продолжения работы отправьте следующее изображение"
            //           );
            //}


            if (message.Type != MessageType.Document && message.Type != MessageType.Photo)
            {
                if (message.Text != "Контраст" && message.Text != "Сепия" && message.Text != "Ч/Б" && message.Text != "Размытие" && message.Text != "/start" && correct_numbers.Contains(message.Text) == false)
                {
                    await botClient.SendTextMessageAsync(
                    chat.Id,
                    "Введите корректную команду!"
                    );
                }
            }



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
    

