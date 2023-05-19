using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static CS_Examination_Dictionary.Program;
using static System.Net.Mime.MediaTypeNames;
/*Создать приложение «Словари».
Основная задача проекта: хранить словари на разных языках и разрешать пользова-
телю находить перевод нужного слова или фразы.
Интерфейс приложения должен предоставлять такие возможности:
■ Создавать словарь. При создании нужно указать тип словаря.
Например, англо-русский или русско-английский.
■ Добавлять слово и его перевод в уже существующий словарь. Так как у слова мо-
жет быть несколько переводов, необходимо поддерживать возможность создания
нескольких вариантов перевода.//
■ Заменять слово или его перевод в словаре.
■ Удалять слово или перевод. Если удаляется слово, все его переводы удаляются
вместе с ним. Нельзя удалить перевод слова, если это последний вариант пере-
вода.
■ Искать перевод слова.
■ Словари должны храниться в файлах.
■ Слово и варианты его переводов можно экспортировать в отдельный файл резуль-
тата.

■ При старте программы необходимо показывать меню для работы с программой.
Если выбор пункта меню открывает подменю, то тогда в нем требуется предусмо-
треть возможность возврата в предыдущее меню.*/
namespace CS_Examination_Dictionary
{
    internal class Program
    {
        #region cOLLECTvOCABULARY
        [KnownType(typeof(CollectionVocabulary))]
        [KnownType(typeof(Vocabulary))]
        [DataContract]
        class CollectionVocabulary
        {
            
            public CollectionVocabulary() { }
            [DataMember]

            public List<IVocabulary> vocabulary = new List<IVocabulary>();
            public void AddNewVocabulary(IVocabulary vocabulary)
            {
                this.vocabulary.Add(vocabulary);
            }
            public void CreateNewVocabulary(VocabularyCreator obj)
            {
                Console.WriteLine("Enter name of vocabulary : ");
                string name = Console.ReadLine();
                this.vocabulary.Add(obj.VocabularyCreate());
                vocabulary.Last().Name = name;

            }
            public void OutputVocabularyList()
            {
                int count = 0;
                foreach (var el in vocabulary)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(++count + " . " + el.Name);

                }
                Console.ResetColor();
               
              
                
            }
            public IVocabulary getVocabulary(int index)
            {
                return vocabulary.ElementAt(index);
            }
        }

        #endregion CollectVocabulary



        #region vocabulary

        class VocabularyCreator
        {
            public IVocabulary VocabularyCreate()
            {
                return new Vocabulary();
            }
        }

        [DataContract]
        abstract class IVocabulary
        {
            [DataMember]
            public string Name { set; get; }
            [DataMember]
            protected Dictionary<string, List<string>> vocabulary = new Dictionary<string, List<string>>();
            public virtual void AddNewWord() { }
            public virtual void AddNewTranslate() { }
            public virtual void OutputVocabulary() { }
            public virtual void DeleteWord() { }
            public virtual void DeleteTranslate() { }
            public virtual void ChangeWord() { }
            public virtual void ChangeTranslate() { }
            public virtual void Search() { }
            public virtual void AddNewWord(IWord word) { }

            public Dictionary<string, List<string>> GetVocabulary()
            {
                return vocabulary;
            }
            public IWord GetWordByIndex(int index)
            {
                IWord word = new Word();
                word.SetIWord(vocabulary.ElementAt(index).Key, vocabulary.ElementAt(index).Value);
                return word;
            }

        }
       
        [DataContract]
        class Vocabulary : IVocabulary
        {


            public Vocabulary() { }
            public Vocabulary(Dictionary<string, List<string>> vocabulary, string name)
            {
                this.vocabulary = vocabulary;
                vocabulary.OrderBy(x => x);
                this.Name = name;
            }

            public override void AddNewWord()
            {

                IWord word = new Word();
                word.SetWord();

                foreach (var el in word.GetWordAsDict())
                {
                    vocabulary.Add(el.Key, el.Value);
                }
                vocabulary.OrderBy(x => x);

            }
            public override void AddNewWord(IWord word)
            {
                foreach (var el in word.GetWordAsDict())
                {
                    vocabulary.Add(el.Key, el.Value);
                }
                vocabulary.OrderBy(x => x);

            }
            public override void AddNewTranslate()
            {

                Console.Clear();
                OutputVocabulary();
                Console.WriteLine("Select a word to add a translation");
                int choice = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter a new word");
                string word = Console.ReadLine();
                vocabulary.ElementAt(choice - 1).Value.Add(word);
                vocabulary.OrderBy(x => x);
                OutputVocabulary();



            }
            public override void OutputVocabulary()
            {
                int count = 0;
                foreach (var el in vocabulary)
                {

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(++count + ". " + el.Key + " : ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    foreach (var item in el.Value)
                    {
                        Console.WriteLine("\t" + item);
                    }
                    Console.ResetColor();
                }

            }
            public override void DeleteWord()
            {
                Console.Clear();
                OutputVocabulary();
                Console.WriteLine("Choose a word for delete");
                int choice = int.Parse(Console.ReadLine());
                vocabulary.Remove(vocabulary.ElementAt(choice - 1).Key);
                Console.Clear();

                OutputVocabulary();
            }
            void DeleteWord(int choice)
            {
                Console.Clear();
                OutputVocabulary();
                vocabulary.Remove(vocabulary.ElementAt(choice - 1).Key);
                Console.Clear();
                OutputVocabulary();
            }
            public override void DeleteTranslate()
            {
                int count = 0;
                Console.Clear();
                OutputVocabulary();
                Console.WriteLine("Choose a word to remove a translation");
                int choice = int.Parse(Console.ReadLine());
                Console.WriteLine("\nChoose a word for removing");

                foreach (var el in vocabulary.ElementAt(choice - 1).Value)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(++count + " " + el);
                    Console.ResetColor();
                }
                int choice2 = int.Parse(Console.ReadLine());
                if (vocabulary.ElementAt(choice - 1).Value.Count > 1)
                    vocabulary.ElementAt(choice - 1).Value.Remove(vocabulary.ElementAt(choice - 1).Value.ElementAt(choice2 - 1));
                else
                    Console.WriteLine("u can't delete last translate.For removing this word u need delete an original word.");
                Console.WriteLine("Word was deleted");
            }
            public override void ChangeWord()
            {
                int choice;
                do
                {
                    Console.Clear();
                    OutputVocabulary();
                    Console.WriteLine("Choose a word for changing\n0-Back");
                    bool flag = int.TryParse(Console.ReadLine(), out choice);
                    if (flag && choice <= vocabulary.Count && choice!=0)
                    {
                        List<string> words = vocabulary.ElementAt(choice - 1).Value;
                        DeleteWord(choice);
                        Console.WriteLine("Enter a new word");
                        string word = Console.ReadLine();
                        vocabulary.Add(word, words);
                        vocabulary.OrderBy(x => x);
                    }
                    if (!flag) choice = 1;
                } while (choice != 0);
            }
            public override void ChangeTranslate()
            {
                int choice;
                do
                {
                    int count = 0;
                    Console.Clear();
                    OutputVocabulary();
                    Console.WriteLine("Choose a word for changing\n0-Back");
                    bool flag = int.TryParse(Console.ReadLine(),out choice);
                    if (flag && choice <= vocabulary.Count && choice != 0)
                    {
                        Console.WriteLine("\nChoose a word for changing");

                        foreach (var el in vocabulary.ElementAt(choice - 1).Value)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine(++count + " " + el);
                            Console.ResetColor();
                        }
                        int choice2 = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter a new word");
                        string word = Console.ReadLine();
                        vocabulary.ElementAt(choice - 1).Value.Remove(vocabulary.ElementAt(choice - 1).Value.ElementAt(choice2 - 1));
                        vocabulary.ElementAt(choice - 1).Value.Add(word);
                        vocabulary.OrderBy(x => x);
                    }
                    if (!flag) choice = 1;
                    
                }while(choice != 0);

            }
            public override void Search()
            {
                Console.WriteLine("Enter a word for searching");
                string word = Console.ReadLine();
                if (vocabulary.Keys.Contains(word))

                    foreach (var el in vocabulary.Keys)
                    {
                        if (el == word)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(el);
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            foreach (var el2 in vocabulary[el])
                                Console.WriteLine("\t" + el2);
                            Console.ResetColor();
                            break;
                        }


                    }
                else
                {
                    bool flag = true;
                    foreach(var el in vocabulary)
                      if(el.Value.Contains(word))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(el.Key);
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            foreach (var el2 in vocabulary[el.Key])
                                Console.WriteLine("\t" + el2);
                            Console.ResetColor();
                             flag = false;
                        }
                    if(flag == true) Console.WriteLine("Word not found :(");
                    Thread.Sleep(1000);
                }

               
            }



        }
        #endregion vocabulary



        #region Class Word
        [DataContract]
        [KnownType(typeof(IWord))]
        public abstract class IWord
        {
            [DataMember]
            public string word = string.Empty;
            [DataMember]
            public List<string> translation = new List<string>();


            virtual public void SetWord() { }
            public Dictionary<string, List<string>> GetWordAsDict()
            {
                Dictionary<string, List<string>> Res = new Dictionary<string, List<string>>();
                Res.Add(word, translation);
                return Res;
            }
            public IWord() { }
            public void SetIWord(string name, List<string> obj)
            {
                this.word = name;
                this.translation = obj;

            }
            public IWord GetWord()
            { return this; }


        }
      
        [DataContract]
        [KnownType(typeof(Word))]
        public class Word : IWord
        {
            

            public Word() { }
            public override void SetWord()
            {

                int choice;
                Console.WriteLine("Enter a word : ");
                word = Console.ReadLine();

                Console.WriteLine("Enter a translate : ");
                string tempWord = Console.ReadLine();
                translation.Add(tempWord);
                do
                {

                    Console.WriteLine("Do u like add other translate ? \n 1-yes\n2-no");
                    choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            {
                                Console.WriteLine("Enter a translate : ");
                                tempWord = Console.ReadLine();
                                if (!String.IsNullOrEmpty(tempWord))
                                    translation.Add(tempWord);
                                else Console.WriteLine("Incorrect input");
                            }
                            break;
                        default:
                            break;
                    }
                } while (choice != 2);
            }


        }

        #endregion Word



        static void Main(string[] args)
        {
            Dictionary<string, List<string>> Collect = new Dictionary<string, List<string>>();
            List<string> translates = new List<string>();
            translates.Add("Киса");
            translates.Add("Котенок");
            translates.Add("Кошара");
            Collect.Add("Cat", translates);
            List<string> translates2 = new List<string>();
            translates2.Add("Машина");
            translates2.Add("Машинка");
            translates2.Add("Авто");
            Collect.Add("Car", translates2);
            IVocabulary vocabulary = new Vocabulary(Collect, "Eng-Ru");


            CollectionVocabulary collectionVocabulary = new CollectionVocabulary();
            collectionVocabulary.AddNewVocabulary(vocabulary);
            ClientInterface clientInterface = new ClientInterface();

            //Thread t1 = new Thread(Melodi);
            //Thread t2 = new Thread(Greating);
            //t1.Start();
            //t2.Start();
            //Thread.Sleep(8000);
            //Console.Clear();
            

            clientInterface.Main_Menu(collectionVocabulary);
        }

        class ClientInterface
        {

            public void Main_Menu(CollectionVocabulary obj)
            {
               
                int choice;
                do
                {
                    
                    Console.WriteLine("Choose an option : "); 
                    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("1-Show all vocabularies"); Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("2-Create new vocabularies"); Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("3-Working with vocabulary"); Console.ResetColor();
                    Console.WriteLine("4-Saving collection of vocabularies");
                    Console.WriteLine("5-Downloading collection of vocabularies");
                    Console.WriteLine("0-Exit");
                    
                    
                    bool flag = int.TryParse(Console.ReadLine(), out choice);
                    if (flag)
                    {
                        Console.Clear();
                        switch (choice)
                        {
                            case 1: { Menu_1(obj); } break;
                            case 2: { Menu_2(obj); } break;
                            case 3: { Menu_3(obj); } break;
                            case 4:
                                {
                                    SavingToFile savingToFile = new SavingToFile();
                                    savingToFile.SaveVocabularyCollection(obj);
                                    Console.WriteLine("File was saved to \"Word.xml\"");
                                }
                                break;
                            case 5:
                                {
                                    DownloadVocFromFile downloadVocFromFile = new DownloadVocFromFile();
                                    obj = downloadVocFromFile.DownloadCollection();

                                    Console.WriteLine("File was download from \"Collection.xml\"");

                                }
                                break;
                            default: break;
                            case 0: { Environment.Exit(0); } break;
                        }
                    }
                    else
                    {
                        choice = -1;
                        Console.Clear();
                    }
                } while (choice != 0);
            }

            public void Menu_1(CollectionVocabulary obj)
            {
                obj.OutputVocabularyList();

            }
            public void Menu_2(CollectionVocabulary obj)
            {
                obj.CreateNewVocabulary(new VocabularyCreator());
            }
            public void Menu_3(CollectionVocabulary obj)
            {
                int choice = 1;
                while(choice != 0) 
                {

                    Console.WriteLine("Choose vocabulary : ");
                    obj.OutputVocabularyList();
                    Console.WriteLine("Back-0");
                    
                    bool Flag = int.TryParse(Console.ReadLine(), out choice);


                    if (Flag && choice <= obj.vocabulary.Count && choice > 0)
                    {
                        int localChoice;
                        do
                        {

                            Console.Clear();
                            Console.WriteLine("Choose an option : ");
                            Console.WriteLine("1-Add new word to vocabulary");
                            Console.WriteLine("2-Change word or translation");
                            Console.WriteLine("3-Delete word or Translation");
                            Console.WriteLine("4-Search word");
                            Console.WriteLine("5-Show all words in Vocabulary");
                            Console.WriteLine("6-Save this word to XML File");
                            Console.WriteLine("0-Back");
                            IVocabulary voc = obj.vocabulary.ElementAt(choice - 1);
                            Console.WriteLine();
                           
                            bool localFLAG = int.TryParse(Console.ReadLine(), out localChoice);

                            
                            if (localFLAG) switch (localChoice)
                                {
                                    case 1:
                                        {
                                            voc.AddNewWord();
                                        }
                                        break;
                                    case 2:
                                        {
                                          
                                            int tempchoice;
                                            do
                                            {
                                                Console.WriteLine("Choose an option : \n1-Change word\n2-Change Translation\n0-Back");
                                                bool flag = int.TryParse(Console.ReadLine(), out tempchoice);
                                                if (!flag) tempchoice = -1;
                                                Console.Clear();
                                                switch (tempchoice)
                                                {
                                                    case 1: voc.ChangeWord(); break;
                                                    case 2: voc.ChangeTranslate(); break;
                                                    case 0:tempchoice = 0; break;
                                                    default: Console.WriteLine("Incorrect choice"); tempchoice = -1; break;
                                                }
                                               
                                            } while (tempchoice != 0);

                                        }
                                        break;
                                    case 3:
                                        {
                                            
                                            int tempchoice;
                                            do
                                            {
                                                Console.WriteLine("Choose an option : \n1-Delete word\n2-Delete Translation\n0-Back\n");
                                                bool flag = int.TryParse(Console.ReadLine(), out tempchoice);
                                                if (!flag) tempchoice = -1;
                                                Console.Clear();
                                                switch (tempchoice)
                                                {
                                                    case 1: voc.DeleteWord(); break;
                                                    case 2: voc.DeleteTranslate(); break;
                                                    case 0: tempchoice = 0; break;
                                                    default: Console.WriteLine("Incorrect choice"); tempchoice = -1; break;
                                                }
                                            } while (tempchoice != 0);

                                        }
                                        break;
                                    case 4:
                                        {
                                            voc.Search();
                                        }
                                        break;
                                    case 5:
                                        {
                                            voc.OutputVocabulary();
                                            Console.WriteLine("Push any key for exit");
                                            string temp = Console.ReadLine();
                                        }
                                        break;
                                    case 6:
                                        {
                                            SavingToFile savingToFile = new SavingToFile();
                                            Console.WriteLine("Choose a word for saving");
                                            voc.OutputVocabulary();
                                            int tempChoice = int.Parse(Console.ReadLine());
                                            savingToFile.SaveWord(voc.GetWordByIndex(tempChoice - 1));
                                            Console.WriteLine("File was saved to \"Word.xml\" ");
                                            Thread.Sleep(1000);

                                        }
                                        break;
                                    case 0: localChoice = 0; break;
                                    default: localChoice = 1; break;

                                }
                            else localChoice = -1;


                        } while (localChoice != 0);

                    }
                    else if (Flag == true && choice == 0)
                        choice = 0;
                    else choice = 1;
                    



                    Console.Clear();
                }
               
            }


            }

        #region saver

        class DownloadVocFromFile
        {
            public CollectionVocabulary DownloadCollection()
            {
                FileStream stream = new FileStream("Collection.xml", FileMode.Open);
                DataContractJsonSerializer downloader = new DataContractJsonSerializer(typeof(CollectionVocabulary));
                CollectionVocabulary collectionVocabulary = (CollectionVocabulary)downloader.ReadObject(stream);
                stream.Close();
                return collectionVocabulary;

            }
            public IWord DownloadWord()
            {
                FileStream stream = new FileStream("Collection.xml", FileMode.Open);
                DataContractJsonSerializer downloader = new DataContractJsonSerializer(typeof(IWord));
                IWord word = (IWord)downloader.ReadObject(stream);
                stream.Close();
                return word;
            }
        }

       
        class SavingToFile
        {
            public void SaveWord(IWord word)
            {
                Stream stream = new FileStream("Word.xml", FileMode.Create);
                DataContractJsonSerializer saver = new DataContractJsonSerializer(typeof(IWord));

                saver.WriteObject(stream, word);
                stream.Close();
                Console.WriteLine("Json serializer OK");

            }
            public void SaveVocabularyCollection(CollectionVocabulary collection)
            {
                FileStream stream = new FileStream("Collection.xml", FileMode.Create);
                DataContractJsonSerializer saver = new DataContractJsonSerializer(typeof(CollectionVocabulary));

                saver.WriteObject(stream, collection);
                stream.Close();
                Console.WriteLine("Json serializer OK");

            

            }
        }

        #endregion saver

        #region Hahaha
        public static void Melodi()
        {
            Console.Beep(659, 120);
            Console.Beep(659, 120);
            Console.Beep(659, 120);
            Thread.Sleep(100);
            Console.Beep(659, 120);
            Console.Beep(659, 120);
            Console.Beep(659, 120);
            Thread.Sleep(300);
            Console.Beep(659, 120);
            Console.Beep(783, 120);
            Console.Beep(523, 120);
            Console.Beep(587, 130);
            Console.Beep(659, 140);
            Console.Beep(261, 150);
            Console.Beep(293, 160);
            Console.Beep(329, 170);
            Console.Beep(698, 180);
            Console.Beep(698, 200);
            Console.Beep(698, 200);
            Thread.Sleep(300);
            Console.Beep(698, 210);
            Console.Beep(659, 220);
            Console.Beep(659, 230);
            Thread.Sleep(300);
            Console.Beep(659, 240);
            Console.Beep(587, 250);
            Console.Beep(587, 260);
            Console.Beep(659, 270);
            Console.Beep(587, 280);
            Thread.Sleep(300);
            Console.Beep(783, 300);
            Thread.Sleep(300);
          
          
        }
        public static void Greating()
        {
            #region greetings
            Random random = new Random();
            string text = "Убедительная просьба , в случае нахождения ошибки прлошу сообщить при первой возможности!. " +
                "\nПо умолчанию у Вас подключен один словарь. Вы можете добавить новый и с ним работать." +
                "\nИнтерфейс я постарался сделать максимально интуитивно понятным." +
                "\nФайл сохраняется в корень проекта , потому что сил на расширение функций сохранения и загрузки у меня уже не осталось." +
                "\nПриятного пользования.";
            foreach (var item in text)
            {
                Console.Write(item);
                Thread.Sleep(random.Next(1, 5));
            }





            #endregion greetings
        }
        #endregion hahaha

    }





}
