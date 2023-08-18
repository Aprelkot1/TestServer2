using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TestKot
{
    //[tests] отправляет список тестов
    //[q] отправляет список вопросов
    //[qt] отправляет вопрос с ответами
    public partial class MainWindow : Window
    {
        public async void StartServer()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
            try
            {
                socket.Bind(ipPoint);
                socket.Listen(1000);
                LogsCreator("Сервер запущен: " + socket.LocalEndPoint);
                while (true)
                {
                    // получаем подключение в виде TcpClient
                    var client = await socket.AcceptAsync();
                    // создаем новую задачу для обслуживания нового клиента
                    Task.Run(async () => await ProcessClientAsync(client));

                    // вместо задач можно использовать стандартный Thread
                    // new Thread(async ()=> await ProcessClientAsync(tcpClient)).Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        async Task ProcessClientAsync(Socket сlient) //в отделньном потоке ловим клиента
        {
            using var stream = new NetworkStream(сlient);
            // буфер для получения данных
            var responseData = new byte[512];
            // получаем данные
            var bytes = await stream.ReadAsync(responseData,0, responseData.Count());
            // преобразуем полученные данные в строку
            string response = Encoding.UTF8.GetString(responseData, 0, bytes);
            ReturnData(response);
            var data = Encoding.UTF8.GetBytes(ReturnData(response));
            // отправляем массив байт на сервер 
            await stream.WriteAsync(data,0,data.Count());
        }
        public string ReturnData(string response)
        {
            StringBuilder dataToClient = new StringBuilder();
            if (response.Contains("[tests]"))//отправляем список тестов
            {
                LogsCreatorAsync(response.Replace("[tests]", ""));
                foreach(var data in MysqlReader("SELECT * FROM Tests", 1))
                {
                    dataToClient.Append(data + "|");
                }
                LogsCreatorAsync("Отправляю тесты");
                dataToClient.Append("[tests]");
                return dataToClient.ToString();
            }
            if (response.Contains("[q]"))
            {
                dataToClient.Append(response.Replace("[q]", "") + "|");
                
                string testId = MysqlReader("SELECT * FROM Tests WHERE testName ='" + response.Replace("[q]","") + "'", 0)[0];
                dataToClient.Append(testId + "|");
                foreach (var data in MysqlReader("SELECT * FROM Questions WHERE testName ='" + testId + "'", 2))
                {
                    dataToClient.Append(data + "|");
                }
                dataToClient.Append("[q]");
                return dataToClient.ToString();
            }
            if (response.Contains("[qt]"))
            {
                string[] questionToSend = response.Replace("[qt]","").Split('|');// [0] - id теста, [1] - переданный вопрос
                LogsCreatorAsync("Получил вопрос " + questionToSend[1]);
                string questionId = MysqlReader("SELECT * FROM Questions WHERE testName ='" + questionToSend[0] + "' AND question = '" + questionToSend[1] +"'", 1)[0];// получаем id вопроса
                string questionType = MysqlReader("SELECT * FROM Questions WHERE testName ='" + questionToSend[0] + "' AND question = '" + questionToSend[1] + "'", 3)[0]; //получаем тип вопроса
                //получаем список правильных ответов
                if (questionType.Contains("вариант"))
                {
                   
                    List<string> rightAnswers = MysqlReader("SELECT * FROM Answers WHERE question ='" + questionId + "' AND isRight = 'Верный'", 1);
                    //соединяем вопрос, его тип и количество правильных ответов с разделителями
                    dataToClient.Append(questionToSend[1] + "|" + rightAnswers.Count + "|" + questionType + "|");
                    //добавляем правильные ответы
                    foreach (var answerR in rightAnswers)
                    {
                        dataToClient.Append(answerR + "|");
                    }
                }
                if (questionType.Contains("Последовательность"))
                {

                    List<string> rightAnswers = MysqlReader("SELECT * FROM Answers WHERE question ='" + questionId + "'", 1);
                    //соединяем вопрос, его тип и количество правильных ответов с разделителями
                    dataToClient.Append(questionToSend[1] + "|" + "1" + "|" + questionType + "|");
                    //добавляем правильные ответы
                    foreach (var answerR in rightAnswers)
                    {
                        dataToClient.Append(answerR);
                    }
                    dataToClient.Append("|");
                }
                List<string> allAnswers = MysqlReader("SELECT * FROM Answers WHERE question ='" + questionId + "'", 1);
                var rnd = new Random();
                allAnswers = allAnswers.OrderBy(item => rnd.Next()).ToList();

                //добавляем все ответы
                foreach (var answerAll in allAnswers)
                {
                    dataToClient.Append(answerAll + "|");
                }
                dataToClient.Append("[qt]");
                LogsCreatorAsync("Отправляю " + dataToClient.ToString());
                return dataToClient.ToString();
            }
            if (response.Contains("[end]"))
            {
                string[] result = response.Replace("[end]", "").Split('|');// имя, тест, резултат
                LogsCreatorAsync(result[0] + " сдал тест " + result[1] + " на " + result[2] + "%.");
                MysqlReader("INSERT INTO UserResult(userName, test, result) VALUES('" + result[0] + "','" + result[1] + "','" + result[2] + "')", 0);
                UserOut();//шото неделает обновление сам надобо кнопку обновить нажать
            }
            return null;
        }
    }
}
