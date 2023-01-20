using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using dllClass;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Controls;
using System.Text.Json.Serialization;

namespace lab4
{
    class AppViewModel : INotifyPropertyChanged
    {
        readonly string path = "C:\\Users\\artem\\Desktop\\программирование ч.2\\WriteFile.txt";
        public string Russian { get; set; }
        public string English { get; set; }

        private List<Page> pages;

        private readonly Frame MainFrame;

        public AppViewModel(Frame frame)
        {
            wgFactory = new WriteGameFactory();

            vocabular = new ObservableCollection<Word>();
            Vocabular = new ReadOnlyObservableCollection<Word>(vocabular);
            MainFrame = frame;
            pages = new List<Page>()
            {
                new MainPage(),
                new PageTranslateGame()
            };
            foreach (var page in pages)
            {
                page.DataContext = this;
            }
            MainFrame.Navigate(pages[0]);
        }

        #region Игра
        private readonly WriteGameFactory wgFactory;
        public SeparateGame GetWriteSG()
        {
            return wgFactory.CreateSeparateGame(vocabular.ToList());
        }

        private SeparateGame writeSG;
        public SeparateGame WriteSG
        {
            get
            {
                return writeSG;
            }
            set
            {
                writeSG = value;
                OnPropertyChanged();
            }
        }
        private string writeSeparateGameSelected;
        public string WSGSelected
        {
            get
            {
                return writeSeparateGameSelected;
            }
            set
            {
                writeSeparateGameSelected = value;
                OnPropertyChanged();
            }
        }
        private string writeSeparateGameAnswer;
        public string WSGAnswer
        {
            get
            {
                return writeSeparateGameAnswer;
            }
            set
            {
                writeSeparateGameAnswer = value;
                OnPropertyChanged();
            }
        }

        private void StartWrite()
        {
            if (WSGSelected != null && WriteSG.EnterAnswer(WSGSelected))
            {
                MessageBox.Show("Ответ верный!");
            }
            else
            {
                MessageBox.Show("Неверно!\nПравильный ответ:\n" + WriteSG.GetAnswer());
            }
            WriteSG = GetWriteSG();
            WSGSelected = "";
        }

        private Command startWriteGame;
        public Command StartWriteGame
        {
            get
            {
                return startWriteGame ?? (startWriteGame = new Command(obj =>
                {
                    StartWrite();
                }));
            }
        }
        #endregion

        #region Работа со страницами
        private Command openPageMainMenu;
        public Command OpenPageMainMenu
        {
            get
            {
                return openPageMainMenu ??
                    (openPageMainMenu = new Command(obj =>
                    {
                        MainFrame.Navigate(pages[0]);
                    }));
            }
        }
        private Command openPageTranslateGame;
        public Command OpenPageTranslateGame
        {
            get
            {
                return openPageTranslateGame ??
                    (openPageTranslateGame = new Command(obj =>
                    {
                        WriteSG = GetWriteSG();
                        MainFrame.Navigate(pages[1]);
                    }));
            }
        }
        #endregion

        #region Работа со словарём

        private ObservableCollection<Word> vocabular;
        public ReadOnlyObservableCollection<Word> Vocabular { get; }

        private Word word;

        private Word selectedWord;

        public Word SelectedWord
        {
            get
            {
                return selectedWord;
            }
            set
            {
                selectedWord = value;
                OnPropertyChanged("SelectedWord");
            }
        }

        private Command addVocabulary;

        public Command AddVocabulary
        {
            get
            {
                return addVocabulary ?? (addVocabulary = new Command(obj =>
                {
                    if (English != null && Russian != null)
                    {
                        word = new Word(English, Russian);
                        vocabular.Add(word);
                    }
                    else MessageBox.Show("Введите слово(а)");
                }));
            }
        }

        private Command deleteVocabulary;
        public Command DeleteVocabulary 
        {
            get
            {
                return deleteVocabulary ?? (deleteVocabulary = new Command(obj =>
                {
                    //var v = vocabular.Where(a => SelectedWord.English == a.English);
                    //v.First());
                    vocabular.Remove(SelectedWord);
                }));
            }
        }
        #endregion

        #region Запись и чтение файла
        private Command readFile;
        public Command ReadFile
        {
            get 
            {
                return readFile ?? (readFile = new Command(obj =>
                {
                    vocabular.Clear();
                    string json = File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<Word[]>(json);
                    foreach (var l in list)
                    {
                        vocabular.Add(l);
                    }
                }));
            }
        }
        
        private Command writeFile;
        public Command WriteFile
        {
            get 
            {
                return writeFile ?? (writeFile = new Command(obj =>
                {
                        string json = JsonSerializer.Serialize(vocabular);
                        File.WriteAllText(path, json);
                }));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
