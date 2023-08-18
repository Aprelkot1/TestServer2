using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace TestKot
{
    public partial class MainWindow : Window
    {
        public void addQuestion_Click(Object sender, EventArgs e)
        {
            StringBuilder question = new StringBuilder();
            StringBuilder questionType = new StringBuilder();
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if (childType.Name == "addQuestionBox")
                    {
                        question.Append(childType.Text);
                    }
                }
                System.Windows.Controls.ComboBox childType2 = child as System.Windows.Controls.ComboBox;
                if (childType2 != null)
                {
                    if (childType2.Name == "addQuestionCombo")
                    {
                        questionType.Append(childType2.Text);
                    }
                }
                
            }
            if (!string.IsNullOrEmpty(question.ToString()) && MysqlReader("SELECT * FROM Questions WHERE question = '" + question.ToString() + "' AND testname = " + tag.Tag.ToString(), 1).Count == 0)
            {
                    //добавляем новый вопрос
                    MysqlReader("INSERT INTO Questions(testName,question,type) VALUES('" + tag.Tag + "','" + question.ToString() + "','" + questionType.ToString() + "')", 0);
                    testsList.Clear();
                    TestOut();
            }
            else
            {
                MessageBox.Show("Вопрос не может быть пустым или такой вопрос уже есть!");
            }
        }
        public void updateQuestion_Click(Object sender, EventArgs e)
        {
            StringBuilder question = new StringBuilder();
            StringBuilder questionOldValue = new StringBuilder();
            StringBuilder questionType = new StringBuilder();
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if (childType.Name == "editQuestionBox")
                    {

                        question.Append(childType.Text);
                    }
                    if (childType.Name == "oldQuestionBox")
                    {
                        questionOldValue.Append(childType.Text);
                    }
                  
                }
                System.Windows.Controls.ComboBox childType2 = child as System.Windows.Controls.ComboBox;
                if (childType2 != null)
                {
                    if (childType2.Name == "editQuestionCombo")
                    {
                        questionType.Append(childType2.Text);
                    }
                }
            }
            if (!string.IsNullOrEmpty(question.ToString()) && MysqlReader("SELECT * FROM Questions WHERE question = '" + question.ToString() + "' AND testname = " + tag.Tag.ToString(), 1).Count == 0)
            {
                //изменяем новый вопрос
                MysqlReader("UPDATE Questions SET question = '" + question.ToString() + "',type ='" + questionType.ToString() + "' WHERE testname = " + tag.Tag.ToString() + " AND question = '" + questionOldValue.ToString() + "'", 0);
                MessageBox.Show(question.ToString());
                testsList.Clear();
                TestOut();
            }
            else if (!string.IsNullOrEmpty(question.ToString()) && MysqlReader("SELECT * FROM Questions WHERE question = '" + question.ToString() + "' AND testname = " + tag.Tag.ToString(), 1).Count == 1)
            {
                //изменяем новый вопрос
                MysqlReader("UPDATE Questions SET question = '" + question.ToString() + "',type ='" + questionType.ToString() + "' WHERE testname = " + tag.Tag.ToString() + " AND question = '" + questionOldValue.ToString() + "'", 0);
                testsList.Clear();
                TestOut();
            }
            else
            {
                MessageBox.Show("Вопрос не может быть пустым или такой уже есть!");
            }

        }
        public void removeQuestion_Click(object sender, EventArgs e)
        {
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            //удаляем вопрос

            var result = MessageBox.Show("Вы точно хотите удалить вопрос?", "Аккуратно!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
                int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parentObj, i);
                    System.Windows.Controls.ListBox childType = child as System.Windows.Controls.ListBox;
                    if (childType != null)
                    {
                        if (childType.Name == "questions")
                        {
                            MysqlReader("DELETE FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + childType.SelectedItem + "'", 0);
                            testsList.Clear();
                            TestOut();
                        }
                    }
                }
            }
        }
        public void questionChanged_Click(Object sender, EventArgs e)
        {
            StringBuilder question = new StringBuilder();
            StringBuilder questionOldValue = new StringBuilder();
            StringBuilder questionType = new StringBuilder();
            System.Windows.Controls.ListBox tag = sender as System.Windows.Controls.ListBox;
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if (childType.Name == "editQuestionBox" && tag.SelectedItem != null)
                    {
                        
                        childType.Text = tag.SelectedItem.ToString();
                    }
                }
                System.Windows.Controls.ComboBox childType2 = child as System.Windows.Controls.ComboBox;
                if (childType2 != null)
                {
                    if (childType2.Name == "editQuestionCombo" && tag.SelectedItem != null)
                    {
                        childType2.Text = MysqlReader("SELECT * FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + tag.SelectedItem + "'", 3)[0];
                    }
                }
                System.Windows.Controls.Button childType3 = child as System.Windows.Controls.Button;
                if (childType3 != null)
                {
                    if (childType3.Name== "addAnswerButton" && tag.SelectedItem != null || childType3.Name == "editAnswerButton" && tag.SelectedItem != null)
                    {
                        childType3.Tag = MysqlReader("SELECT * FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + tag.SelectedItem + "'", 1)[0];
                        
                    }
                }
                System.Windows.Controls.ListBox childType4 = child as System.Windows.Controls.ListBox;
                if (childType4 != null)
                {
                    if (childType4.Name == "answers" && tag.SelectedItem != null)
                    {
                        answerList.Clear();
                        childType4.Tag = MysqlReader("SELECT * FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + tag.SelectedItem + "'", 1)[0];
                        for (int u = 0; u < MysqlReader("SELECT * FROM Answers WHERE question = '" + childType4.Tag + "'", 1).Count; u++) {
                            answerList.Add(MysqlReader("SELECT * FROM Answers WHERE question = '" + childType4.Tag + "'", 1)[u]+ " |" + (MysqlReader("SELECT * FROM Answers WHERE question = '" + childType4.Tag + "'", 2)[u]));
                        }
                        childType4.ItemsSource = answerList;
                    }
                }
                System.Windows.Controls.ComboBox childType5 = child as System.Windows.Controls.ComboBox;
                if (childType5 != null)
                {
                    if (childType5.Name == "addAnswerCombo" && tag.SelectedItem != null)
                    {
                        childType5.Items.Clear();
                        if (MysqlReader("SELECT * FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + tag.SelectedItem + "'", 3)[0].Contains("вариант"))
                        {
                            childType5.Items.Add("Верный");
                            childType5.Items.Add("Неверный");
                            childType5.SelectedIndex = 0;
                        }
                        if (MysqlReader("SELECT * FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + tag.SelectedItem + "'", 3)[0].Contains("Последовательность"))
                        {
                            childType5.Items.Add("1");
                            for (int count = 1; count <= answerList.Count; count++)
                            {
                                childType5.Items.Add(count+1);
                                childType5.SelectedIndex = 0;
                            }
                        }

                    }
                  
                }
                System.Windows.Controls.TextBox childType6 = child as System.Windows.Controls.TextBox;
                if (childType6 != null)
                {
                    if (childType6.Name == "idQuestonOfAnswer" && tag.SelectedItem != null)
                    {
                        childType6.Text = MysqlReader("SELECT * FROM Questions WHERE testName = '" + tag.Tag + "' AND question = '" + tag.SelectedItem + "'", 1)[0];
                    }
                }
            }
        }

    }
    
}

