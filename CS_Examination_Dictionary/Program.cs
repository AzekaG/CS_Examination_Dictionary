using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
        [Serializable]

        class CollectionVocabulary
        {
            public CollectionVocabulary() { }
            
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



        class VocabularyCreator
        {
            public IVocabulary VocabularyCreate()
            {
                return new Vocabulary();
            }
        }

    
       abstract class IVocabulary
        {
            public string Name { set; get; }
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
    #region vocabulary
    class Vocabulary : IVocabulary
        {
            

            public Vocabulary() { }
            public Vocabulary(Dictionary<string, List<string>> vocabulary , string name)
            {
                this.vocabulary = vocabulary;
                vocabulary.OrderBy(x => x);
                this.Name = name;
            }
            
            public override void AddNewWord()
            {
              
                IWord word = new Word();
                word.SetWord();
               
                foreach(var el in word.GetWordAsDict())
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
                vocabulary.ElementAt(choice-1).Value.Add(word);
                vocabulary.OrderBy(x => x);
                OutputVocabulary();



            }
            public override void OutputVocabulary()
            {
                int count = 0;
                foreach (var el in vocabulary)
                {
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(++count + ". "+el.Key + " : ");
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
                vocabulary.Remove(vocabulary.ElementAt(choice-1).Key);  
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
      
                foreach (var el in  vocabulary.ElementAt(choice - 1).Value)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(++count + " "+ el);
                    Console.ResetColor();
                }
                int choice2 = int.Parse(Console.ReadLine());
                if(vocabulary.ElementAt(choice - 1).Value.Count>1)
                        vocabulary.ElementAt(choice - 1).Value.Remove(vocabulary.ElementAt(choice - 1).Value.ElementAt(choice2 - 1));
                else    
                        Console.WriteLine("u can't delete last translate.For removing this word u need delete an original word.");
                Console.WriteLine("Word was deleted");
            }
            public override void ChangeWord()
            {
                
                Console.Clear();
                OutputVocabulary();
                Console.WriteLine("Choose a word for changing");
                int choice = int.Parse(Console.ReadLine());
               
                List<string> words = vocabulary.ElementAt(choice - 1).Value;
                DeleteWord(choice);
                Console.WriteLine("Enter a new word");
                string word = Console.ReadLine();
                vocabulary.Add(word, words);
                vocabulary.OrderBy(x => x);

            }
            public override void ChangeTranslate()
            {
                int count = 0;
                Console.Clear();
                OutputVocabulary();
                Console.WriteLine("Choose a word for changing");
                int choice = int.Parse(Console.ReadLine());

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
                 else Console.WriteLine("Word not found :(");
            }
            
           

        }

        [Serializable]
      public  abstract class IWord
        {
           public string word = string.Empty;
            public List<string> translation = new List<string>();

             
            virtual public void SetWord() { }
            public Dictionary<string , List<string>> GetWordAsDict()
            {
                Dictionary<string, List<string>> Res = new Dictionary<string , List<string>>();
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
        [Serializable]
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
                    switch(choice)
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
                }while (choice !=2);
            }

           
        }

    #endregion






        

   
    static void Main(string[] args)
        {
            Dictionary<string, List<string>> Collect = new Dictionary<string, List<string>>();
            List<string>translates = new List<string>();
            translates.Add("Киса" );
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
                    Console.WriteLine("1-Show all vocabularies");
                    Console.WriteLine("2-Create new vocabularies");
                    Console.WriteLine("3-Working with vocabulary");
                    Console.WriteLine("4-Saving collection of vocabularies");
                    Console.WriteLine("5-Downloading collection of vocabularies");
                    Console.WriteLine("0-Exit");
                    choice = int.Parse(Console.ReadLine());
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
                                CollectionVocabulary collectionVocabulary = downloadVocFromFile.DownloadCollection();

                                Console.WriteLine("File was download from \"Collection.xml\"");

                            }
                            break;
                        default: break;
                        case 0: { Environment.Exit(0); } break;
                    }
                } while (choice != 0);
            }

            public void Menu_1(CollectionVocabulary obj)
            {
                obj.OutputVocabularyList();

            }
            public void Menu_2(CollectionVocabulary obj)
            {
                obj.AddNewVocabulary(new VocabularyCreator().VocabularyCreate());
            }
            public void Menu_3(CollectionVocabulary obj)
            {
                int choice;
                do
                {

                    Console.WriteLine("Choose vocabulary : ");
                    obj.OutputVocabularyList();
                    choice = int.Parse(Console.ReadLine());
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
                    choice = int.Parse(Console.ReadLine());
                   
                    switch (choice)
                    {
                        case 1:
                            {
                                voc.AddNewWord();
                            }
                            break;
                        case 2:
                            {
                                Console.WriteLine("Choose an option : \n1-Change word\n2-Change Translation\n");
                                int tempchoice = int.Parse(Console.ReadLine());

                                switch (tempchoice)
                                {
                                    case 1: voc.ChangeWord(); break;
                                    case 2: voc.ChangeTranslate(); break;
                                    default: Console.WriteLine("Incorrect choice"); break;
                                }

                            }
                            break;
                        case 3:
                            {
                                Console.WriteLine("Choose an option : \n1-Delete word\n2-Delete Translation\n");
                                int tempchoice = int.Parse(Console.ReadLine());

                                switch (tempchoice)
                                {
                                    case 1: voc.DeleteWord(); break;
                                    case 2: voc.DeleteTranslate(); break;
                                    default: Console.WriteLine("Incorrect choice"); break;
                                }

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

                            }
                            break;
                        default: { } break;



                    }


                } while (choice != 0);
            }



        }



        class DownloadVocFromFile
        {
            public CollectionVocabulary DownloadCollection()
            {
                FileStream stream = new FileStream("Collection.xml", FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(CollectionVocabulary));
                CollectionVocabulary collectionVocabulary = (CollectionVocabulary)serializer.Deserialize(stream);
                stream.Close();
                return collectionVocabulary;

            }
            public Dictionary<string, List<string>> DownloadWord()
            {
                FileStream stream = new FileStream("Collection.xml", FileMode.Open);
                
                XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string, List<string>>));
                Dictionary<string, List<string>> word = (Dictionary<string, List<string>>)serializer.Deserialize(stream);
                stream.Close();
                return word;
            }
        }

        class SavingToFile
        {
            public void SaveWord(IWord word)
            {
               Stream stream = new FileStream("Word.xml", FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(Word));
                serializer.Serialize(stream, word);
                stream.Close();

            }
            public void SaveVocabularyCollection(CollectionVocabulary collection)
            {
                FileStream stream = new FileStream("Collection.xml", FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(CollectionVocabulary));
                serializer.Serialize(stream, collection);
                stream.Close();

            }
        }
    }





}
