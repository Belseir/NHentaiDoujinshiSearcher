using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        /*
         * CALLED WHEN USER CLICKS SEARCH BUTTON
         * SETS SEARCH QUERY AND HANDLE SEARCH EXCEPTIONS
         */
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

        /*
         * CALLED AFTER MAKING THE SEARCH
         * SHOWS IN THE SCREAM THE DOUJINSHI COVER AND THE INFO (FAVOURITES, CODE AND NAME)
         */
        private async void LoadResults()
        {
            string Title = Doujin.GetName();
            string URL = Doujin.GetURL();
            string ImageURL = Doujin.GetImage();
            string Favourites = Doujin.GetFavourites();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(ImageURL, UriKind.Absolute);     //ImageURL is the url for the cover of the doujinshi
            bitmap.EndInit(); 

            DoujinshiCover.Source = bitmap;
            DoujinshiTitle.Text = Title;
            DoujinshiCode.Text = URL.Replace("https://nhentai.net/g/", "");
            DoujinshiFavourites.Text = Favourites;

            await Task.Delay(2000);
            SettingMenu.Visibility = Visibility.Hidden;
            SearchResults.Visibility = Visibility.Visible;
        }

        /* SHOWS THE SEARCH SETTINGS MENU
         * AFTER SEARCHING THE SETTINGS WILL BE CLEARED
         */
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            SearchResults.Visibility = Visibility.Hidden;

            if (SettingMenu.Visibility != Visibility.Visible)
            {
                Doujin.ClearAll();
                SettingMenu.Visibility = Visibility.Visible;
            }
        }

        //SHOWS THE MENU OF EACH CHECKBOX
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

        //HIDES THE MENU OF EACH CHECKBOX
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

        //WHEN A TAG IS CLICKED CALLS THE ADDTAG METHOD
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tag = (string)btn.Content;
            Doujin.AddTag(tag.ToLower(), btn);
        }

        //WHEN A TAG IS CLICKED CALLS THE EXCLUDETAG METHOD
        private void ExcludeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tag = (string)btn.Content;
            Doujin.ExcludeTag(tag.ToLower(), btn);
        }

        //WHEN A LANGUAGE IS CLICKED CALLS THE ADDLANGUAGE METHOD
        private void AddLanguage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string language = (string)btn.Content;
            Doujin.AddLanguage(language.ToLower(), btn);
        }

        //CALLED WHEN THE USER ENTERS A CUSTOM SEARCH AT AN ENTRY AND THEN PRESS THE BUTTON
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

        //CALLED WHEN USER PRESSES "NHENTAI/GITHUB/GO TO DOUJINSHI" BUTTON
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

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string filePath = "";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    filePath = saveFileDialog.FileName;
                }
                catch (System.UnauthorizedAccessException)
                {
                    _ = MessageBox.Show("System.UnauthorizedAccessException", "NHentai Doujinshi Searcher", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (filePath != "")
            {
                Doujin.Download(filePath);
            }
            else
            {
                _ = MessageBox.Show("Filename wasn't provided", "NHentai Doujinshi Searcher", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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

    private List<string> CustomTags = new List<string>();

    private List<string> Info = new List<string>();
    private List<NHentaiSharp.Search.Page> Pages = new List<NHentaiSharp.Search.Page>();

    //ADDS/REMOVES THE TAG TO THE LIST AND CHANGES BUTTON COLOR
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

    //ADDS/REMOVES THE TAG TO THE LIST (CUSTOM TAG)
    public void AddTag(string _tag)
    {
        if (!CustomTags.Contains(_tag))
        {
            CustomTags.Add(_tag);
        }
        else
        {
            CustomTags.Remove(_tag);
        }
    }

    //ADDS/REMOVES THE TAG TO THE LIST AND CHANGES BUTTON COLOR
    public void ExcludeTag(string _tag, Button btn)
    {
        string tag = NHentaiSharp.Core.SearchClient.GetExcludeTag(_tag);

        if (!ExcludedTags.Contains(tag))
        {
            ExcludedTags.Add(tag);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF2B2B2B"));
        }
        else
        {
            ExcludedTags.Remove(tag);
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF595959"));
        }
    }

    //ADDS/REMOVES THE TAG TO THE LIST (CUSTOM TAG)
    public void ExcludeTag(string _tag)
    {
        string tag = NHentaiSharp.Core.SearchClient.GetExcludeTag(_tag);

        if (!CustomTags.Contains(tag))
        {
            CustomTags.Add(tag);
        }
        else
        {
            CustomTags.Remove(tag);
        }
    }

    //ADDS THE CHARACTER TO THE LIST (CHARACTER ENTRY)
    public void AddCharacter(string _character)
    {
        string character = NHentaiSharp.Core.SearchClient.GetCategoryTag(_character, NHentaiSharp.Search.TagType.Character);

        if (!Characters.Contains(character))
            Characters.Add(character);
    }

    //ADDS THE PARODY TO THE LIST (PARODY ENTRY)
    public void AddParody(string _parody)
    {
        string parody = NHentaiSharp.Core.SearchClient.GetCategoryTag(_parody, NHentaiSharp.Search.TagType.Parody);

        if (!Parodies.Contains(parody))
            Parodies.Add(parody);
    }

    //ADDS THE LANGUAGE TO THE LIST (LANGUAGE ENTRY)
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

    //SETS ALL THE SEARCH QUERY
    public void SetInfo()
    {
        Info.AddRange(CustomTags);
        Info.AddRange(LookingTags);
        Info.AddRange(ExcludedTags);
        Info.AddRange(Characters);
        Info.AddRange(Parodies);
        Info.AddRange(Languages);
    }

    //MAKES THE SEARCH AND THEN RETRIVES THE NAME, URL, COVER AND FAVORITES OF THE DOUJINSHI
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
        Pages = doujinshi.pages.ToList();
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

    //CLEARS ALL THE CUSTOMIZED INFO & THE SEARCH QUERY
    public void ClearAll()
    {
        ClearList(CustomTags);
        ClearList(Characters);
        ClearList(Parodies);
        ClearList(Info);
    }

    //CLEARS A LIST
    public void ClearList(List<string> toClear)
    {
        while (toClear.Count != 0)
        {
            toClear.RemoveAt(0);
        }
    }

    public void Download(string DownloadPath)
    {
        int DownloadedPages = 0;

        foreach (NHentaiSharp.Search.Page currentPage in Pages)
        {
            string Path = DownloadPath + DownloadedPages.ToString() + "." + currentPage.format.ToString().ToLower();

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(currentPage.imageUrl.ToString()), Path);
            }

            DownloadedPages += 1;
        }

        _ = MessageBox.Show("Download finished!", "NHentai Doujinshi Searcher", MessageBoxButton.OK, MessageBoxImage.Asterisk);
    }
}
