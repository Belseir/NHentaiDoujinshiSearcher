using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NHentai_Doujinshi_Searcher
{
    public partial class MainWindow : Window
    {
        private Doujinshi Doujin = new Doujinshi();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartSearch(object sender, RoutedEventArgs e)
        {
            SettingMenu.Visibility = Visibility.Hidden;

            Doujin.SetInfo();

            try
            {
                await Doujin.MakeSearch();
            }
            catch (NHentaiSharp.Exception.InvalidArgumentException)
            {
                _ = MessageBox.Show("No doujinshis found or Invalid tag was entered", "NHentai Doujinshi Searcher", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (NHentaiSharp.Exception.EmptySearchException)
            {
                _ = MessageBox.Show("No tags provided", "NHentai Doujinshi Searcher", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadResults();
        }

        private async void LoadResults()
        {
            string Title = Doujin.GetName();
            string URL = Doujin.GetURL();
            string ImageURL = Doujin.GetImage();
            string Favourites = Doujin.GetFavourites();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(ImageURL, UriKind.Absolute);
            bitmap.EndInit();

            DoujinshiCover.Source = bitmap;
            DoujinshiTitle.Text = Title;
            DoujinshiCode.Text = URL.Replace("https://nhentai.net/g/", "");
            DoujinshiFavourites.Text = Favourites;

            await Task.Delay(2000);
            SettingMenu.Visibility = Visibility.Hidden;
            SearchResults.Visibility = Visibility.Visible;
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            SearchResults.Visibility = Visibility.Hidden;

            if (SettingMenu.Visibility != Visibility.Visible)
            {
                Doujin.ClearAll();
                SettingMenu.Visibility = Visibility.Visible;
            }
            else
                SettingMenu.Visibility = Visibility.Hidden;
        }

        private void SettingsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            string name = chk.Name;

            switch (name)
            {
                case "AddTags":
                    {
                        Add_Tags.Visibility = Visibility.Visible;
                        break;
                    }
                case "ExcludeTags":
                    {
                        Exclude_Tags.Visibility = Visibility.Visible;
                        break;
                    }
                case "AddCharacters":
                    {
                        Add_Character.Visibility = Visibility.Visible;
                        break;
                    }
                case "AddParodies":
                    {
                        Add_Parody.Visibility = Visibility.Visible;
                        break;
                    }
                case "AddLanguages":
                    {
                        Add_Language.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        private void SettingsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            string name = chk.Name;

            switch (name)
            {
                case "AddTags":
                    {
                        Add_Tags.Visibility = Visibility.Hidden;
                        break;
                    }
                case "ExcludeTags":
                    {
                        Exclude_Tags.Visibility = Visibility.Hidden;
                        break;
                    }
                case "AddCharacters":
                    {
                        Add_Character.Visibility = Visibility.Hidden;
                        break;
                    }
                case "AddParodies":
                    {
                        Add_Parody.Visibility = Visibility.Hidden;
                        break;
                    }
                case "AddLanguages":
                    {
                        Add_Language.Visibility = Visibility.Hidden;
                        break;
                    }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tag = (string)btn.Content;
            Doujin.AddTag(tag.ToLower(), btn);
        }

        private void ExcludeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tag = (string)btn.Content;
            Doujin.ExcludeTag(tag.ToLower(), btn);
        }

        private void AddLanguage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string language = (string)btn.Content;
            Doujin.AddLanguage(language.ToLower(), btn);
        }

        private void EntryButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string btnName = btn.Name;

            switch (btnName)
            {
                case "CustomExclude":
                    {
                        string tag = EntryCustomExclude.Text;
                        EntryCustomExclude.Text = "";
                        Doujin.ExcludeTag(tag.ToLower());
                        break;
                    }
                case "CustomAdd":
                    {
                        string tag = EntryCustomTag.Text;
                        EntryCustomTag.Text = "";
                        Doujin.AddTag(tag.ToLower());
                        break;
                    }
                case "CharacterAdd":
                    {
                        string character = (string)CharacterEntry.Text;
                        CharacterEntry.Text = "";
                        Doujin.AddCharacter(character.ToLower());
                        break;
                    }
                case "ParodyAdd":
                    {
                        string parody = (string)ParodyEntry.Text;
                        ParodyEntry.Text = "";
                        Doujin.AddParody(parody.ToLower());
                        break;
                    }
            }
        }

        private void RedirectButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string btnName = btn.Name;

            switch (btnName)
            {
                case "btnNHentai":
                    {
                        System.Diagnostics.Process.Start("http://nhentai.net");
                        break;
                    }
                case "btnGithub":
                    {
                        System.Diagnostics.Process.Start("https://github.com/Belseir");
                        break;
                    }
                case "btnDoujinshiGo":
                    {
                        System.Diagnostics.Process.Start(Doujin.GetURL());
                        break;
                    }
            }
        }
    }
}

internal class Doujinshi
{
    private string Name;
    private string URL;
    private string Image;
    private int Favourites;

    private List<string> LookingTags = new List<string>();
    private List<string> ExcludedTags = new List<string>();
    private List<string> Characters = new List<string>();
    private List<string> Parodies = new List<string>();
    private List<string> Languages = new List<string>();

    private List<string> Info = new List<string>();

    public void AddTag(string _tag, Button btn)
    {
        if (!LookingTags.Contains(_tag))
        {
            LookingTags.Add(_tag);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF2B2B2B"));
        }
        else
        {
            LookingTags.Remove(_tag);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF595959"));
        }
    }

    public void AddTag(string _tag)
    {
        if (!LookingTags.Contains(_tag))
        {
            LookingTags.Add(_tag);
        }
        else
        {
            LookingTags.Remove(_tag);
        }
    }

    public void ExcludeTag(string _tag, Button btn)
    {
        string tag = NHentaiSharp.Core.SearchClient.GetExcludeTag(_tag);

        if (!ExcludedTags.Contains(tag))
        {
            LookingTags.Add(tag);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF2B2B2B"));
        }
        else
        {
            LookingTags.Remove(tag);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF595959"));
        }
    }

    public void ExcludeTag(string _tag)
    {
        string tag = NHentaiSharp.Core.SearchClient.GetExcludeTag(_tag);

        if (!ExcludedTags.Contains(tag))
        {
            LookingTags.Add(tag);
        }
        else
        {
            LookingTags.Remove(tag);
        }
    }

    public void AddCharacter(string _character)
    {
        string character = NHentaiSharp.Core.SearchClient.GetCategoryTag(_character, NHentaiSharp.Search.TagType.Character);

        if (!Characters.Contains(character))
            Characters.Add(character);
    }

    public void AddParody(string _parody)
    {
        string parody = NHentaiSharp.Core.SearchClient.GetCategoryTag(_parody, NHentaiSharp.Search.TagType.Parody);

        if (!Parodies.Contains(parody))
            Parodies.Add(parody);
    }

    public void AddLanguage(string _language, Button btn)
    {
        string language = NHentaiSharp.Core.SearchClient.GetCategoryTag(_language, NHentaiSharp.Search.TagType.Language);

        if (!Languages.Contains(language))
        {
            Languages.Add(language);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF2B2B2B"));
        }
        else
        {
            Languages.Remove(language);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF595959"));
        }
    }

    public void SetInfo()
    {
        Info.AddRange(LookingTags);
        Info.AddRange(ExcludedTags);
        Info.AddRange(Characters);
        Info.AddRange(Parodies);
        Info.AddRange(Languages);
    }

    public async Task MakeSearch()
    {
        Random randomNumber = new Random();
        string[] SearchQuery = Info.ToArray();

        NHentaiSharp.Search.SearchResult result = await NHentaiSharp.Core.SearchClient.SearchWithTagsAsync(SearchQuery);
        int page = randomNumber.Next(0, result.numPages) + 1;
        result = await NHentaiSharp.Core.SearchClient.SearchWithTagsAsync(SearchQuery, page);
        var doujinshi = result.elements[randomNumber.Next(0, result.elements.Length)];

        Name = doujinshi.prettyTitle.ToString();
        URL = doujinshi.url.ToString();
        Image = Convert.ToString(doujinshi.cover.imageUrl);
        Favourites = doujinshi.numFavorites;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetURL()
    {
        return URL;
    }

    public string GetImage()
    {
        return Image;
    }

    public string GetFavourites()
    {
        return Convert.ToString(Favourites);
    }

    public void ClearAll()
    {
        ClearList(Characters);
        ClearList(Parodies);
        ClearList(Info);
    }

    public void ClearList(List<string> toClear)
    {
        while (toClear.Count != 0)
        {
            toClear.RemoveAt(0);
        }
    }
}