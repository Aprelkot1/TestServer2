using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestKot
{
    public partial class MainWindow : Window
    {
        public void addTest_Click(Object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(addTestNameBox.Text.ToString()) && MysqlReader("SELECT * FROM Tests WHERE testName = '" + addTestNameBox.Text.ToString() + "'", 1).Count == 0)
            {
                    //добавляем новый тест
                    MysqlReader("INSERT INTO Tests(testName) VALUES('" + addTestNameBox.Text + "')", 0);
                    testsList.Clear();
                    TestOut();
            }
            else
            {
                MessageBox.Show("Тест с таким названием уже есть или текстовое поле пустое.");
            }
        }
        public void removeTest_Click(object sender, EventArgs e)
        {
           System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            //удаляем тест
            var result = MessageBox.Show("Вы точно хотите удалить тест?", "Аккуратно!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MysqlReader("DELETE FROM Tests WHERE id = " + tag.Tag, 0);
                testsList.Clear();
                TestOut();
            }
        }
        public void updateTest_Click(object sender, EventArgs e)
        {
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if(MysqlReader("SELECT * FROM Tests WHERE testName = '" + childType.Text + "'", 1).Count > 0)
                    {
                        MessageBox.Show("Тест с таким названием уже есть.");
                    }
                    if (childType.Name == "testNameBox" && childType.Text != null && MysqlReader("SELECT * FROM Tests WHERE testName = '" + childType.Text + "'", 1).Count == 0)
                    {
                            MysqlReader("UPDATE Tests SET testName = '" + childType.Text + "' WHERE id = " + tag.Tag, 0);
                            testsList.Clear();
                            TestOut();
                    }
                }
            }
        }
    }
}
