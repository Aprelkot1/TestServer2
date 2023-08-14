using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TestKot
{
    public partial class MainWindow : Window
    {
        public void addAnswer_Click(Object sender, EventArgs e)
        {
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            StringBuilder answer = new StringBuilder(); //билдер для значения ответа
            StringBuilder answerIsRight = new StringBuilder(); //билдер для значения Верно/Неверно
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);//ищем родителя
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if (childType.Name == "addAnswerBox")
                    {
                        answer.Append(childType.Text);
                    }
                }
                System.Windows.Controls.ComboBox childType2 = child as System.Windows.Controls.ComboBox;
                if (childType2 != null)
                {
                    if (childType2.Name == "addAnswerCombo")
                    {
                        answerIsRight.Append(childType2.Text);
                    }
                }
            }
            if (!string.IsNullOrEmpty(answer.ToString()) && tag.Tag != null && MysqlReader("SELECT * FROM Answers WHERE answer = '" + answer.ToString() + "' AND question = " + tag.Tag.ToString(), 1).Count == 0)
            {
                //добавляем новый ответ
                MysqlReader("INSERT INTO Answers(question,answer,isRight) VALUES('" + tag.Tag + "','" + answer.ToString() + "','" + answerIsRight.ToString() + "')", 0);
                answerList.Add(answer.ToString() + " |"+ answerIsRight.ToString());
            }
            else
            {
                MessageBox.Show("Ответ не может быть пустым или сначала выберите вопрос и редактируемый ответ! Ну, или такой ответ уже есть.");
            }

        }
        public void removeAnswer_Click(object sender, EventArgs e)
        {
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            //удаляем ответ
            StringBuilder questionId = new StringBuilder();
            var result = MessageBox.Show("Вы точно хотите удалить ответ?", "Аккуратно!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
                int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parentObj, i);
                    System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                    if (childType != null)
                    {
                        if (childType.Name == "idQuestonOfAnswer")//скрытый текстбокс где хранится id вопроса
                        {
                            questionId.Append(childType.Text);
                        }
                    }
                    System.Windows.Controls.ListBox childType2 = child as System.Windows.Controls.ListBox;
                    if (childType2 != null)
                    {
                        if (childType2.Name == "answers")
                        {
                            if (childType2.SelectedItem != null)
                            {
                                MysqlReader("DELETE FROM Answers WHERE question = '" + questionId + "' AND answer = '" + childType2.SelectedItem.ToString().Split('|')[0].Trim() + "'", 0);
                                answerList.Remove(childType2.SelectedItem.ToString());
                            }
                        }
                    }
                }
            }
        }
        public void editAnswer_Click(Object sender, EventArgs e)
        {
            StringBuilder answer = new StringBuilder();
            StringBuilder answerIsRight = new StringBuilder();
            StringBuilder answerOldValue= new StringBuilder();
            System.Windows.Controls.Button tag = sender as System.Windows.Controls.Button;
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if (childType.Name == "editAnswerBox")
                    {

                        answer.Append(childType.Text);
                        childType.Clear();
                    }
                    if (childType.Name == "oldAnswerBox")
                    {
                        answerOldValue.Append(childType.Text.Split('|')[0].Trim());
                    }
                }
                System.Windows.Controls.ComboBox childType2 = child as System.Windows.Controls.ComboBox;
                if (childType2 != null)
                {
                    if (childType2.Name == "editAnswerCombo")
                    {
                        answerIsRight.Append(childType2.Text);
                    }
                }
                System.Windows.Controls.ComboBox childType3 = child as System.Windows.Controls.ComboBox;
                if (childType3 != null)
                {
                    if (childType2.Name == "answers")
                    {
                        
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(answer.ToString()) && tag.Tag != null)
            {
                answerList.Clear();
                if (!answerIsRight.ToString().Equals(MysqlReader("SELECT * FROM Answers WHERE answer = '" + answer.ToString() + "' AND question = " + tag.Tag.ToString(), 2)))
                {

                    MysqlReader("UPDATE Answers SET isRight ='" + answerIsRight.ToString() + "' WHERE question = " + tag.Tag.ToString() + " AND answer = '" + answerOldValue.ToString() + "'", 0);

                    if (MysqlReader("SELECT * FROM Answers WHERE answer = '" + answer.ToString() + "' AND question = " + tag.Tag.ToString(), 1).Count == 0)
                    {
                        MysqlReader("UPDATE Answers SET answer = '" + answer.ToString() + "',isRight ='" + answerIsRight.ToString() + "' WHERE question = " + tag.Tag.ToString() + " AND answer = '" + answerOldValue.ToString() + "'", 0);
                    }
                }
                for (int u = 0; u < MysqlReader("SELECT * FROM Answers WHERE question = '" + tag.Tag.ToString() + "'", 1).Count; u++)
                {
                    answerList.Add(MysqlReader("SELECT * FROM Answers WHERE question = '" + tag.Tag.ToString() + "'", 1)[u] + " |" + (MysqlReader("SELECT * FROM Answers WHERE question = '" + tag.Tag.ToString() + "'", 2)[u]));
                }
            }
            else 
            {
                MessageBox.Show("Ответ не может быть пустым или сначала выберите вопрос и редактируемый ответ! Возможно такой ответ в этом вопросе уже есть.");
            }
          
        }
        public void answerChanged_Click(Object sender, EventArgs e)
        {
            StringBuilder questionId = new StringBuilder();
            System.Windows.Controls.ListBox tag = sender as System.Windows.Controls.ListBox;
            questionId.Append(tag.Tag.ToString());
            DependencyObject parentObj = VisualTreeHelper.GetParent(tag);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentObj, i);
                System.Windows.Controls.TextBox childType = child as System.Windows.Controls.TextBox;
                if (childType != null)
                {
                    if (childType.Name == "editAnswerBox" && tag.SelectedItem != null)
                    {

                        childType.Text = tag.SelectedItem.ToString().Split('|')[0].Trim();
                    }
                }
                
                System.Windows.Controls.ComboBox childType2 = child as System.Windows.Controls.ComboBox;
                if (childType2 != null)
                {
                    if (childType2.Name == "editAnswerCombo" && tag.SelectedItem != null)
                    {
                        childType2.Text = MysqlReader("SELECT * FROM Answers WHERE question = '" + questionId.ToString() + "' AND answer = '" + tag.SelectedItem.ToString().Split('|')[0].Trim() + "'", 2)[0];
                    }
                }
                
            }
        }
    }
}
