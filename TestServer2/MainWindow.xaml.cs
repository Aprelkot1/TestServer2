using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using static System.Net.Mime.MediaTypeNames;

namespace TestKot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Test> testsList = new ObservableCollection<Test>();
        ObservableCollection<string> answerList = new ObservableCollection<string>();
        ObservableCollection<UserResult> resultList = new ObservableCollection<UserResult>();
        public MainWindow()
        {
            InitializeComponent();
            OnStart();
            StartServer();
            TestOut();
            UserOut();
        }
        //выводим тесты
        public void TestOut()
        {
            List<string> testsOutTemp = MysqlReader("SELECT * FROM Tests", 1);
            List<string> testsOutTempId = MysqlReader("SELECT * FROM Tests", 0);
            for (int i = 0; i < testsOutTemp.Count; i++)
            {

                Test testTemp = new Test
                {
                    testName = testsOutTemp[i],
                    questionList = MysqlReader("SELECT * FROM Questions WHERE testName = '" + testsOutTempId[i] + "'", 2),
                    testId = testsOutTempId[i]
                };

                testsList.Add(testTemp);
            }
            testsOut.ItemsSource = testsList;
        }
        public void UserOut()
        {
            resultList.Clear();
            List<string> users = MysqlReader("SELECT * FROM UserResult", 0);
            List<string> tests = MysqlReader("SELECT * FROM UserResult", 1);
            List<string> results = MysqlReader("SELECT * FROM UserResult", 2);
            for (int i = 0; i < users.Count; i++)
            {
                UserResult tempResult = new UserResult
                {
                    userResult = users[i] + " сдал " + tests[i] + " на " + results[i] + "%"
                };
                resultList.Add(tempResult);
            }
            resultList.Reverse();
            resultOut.ItemsSource = resultList;
        }
        public void UpdateResult_Click(Object sender, EventArgs e)
        {
            UserOut();
        }

       
    }
    public class Test
    {
       public string testName { get; set; }
       public List<string> questionList { get; set; }
        public string testId { get; set; }
    }
    public class UserResult
    {
        public string userResult { get; set; }
    }

}
