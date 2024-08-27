using GamesClassLibrary.Data;
using GamesClassLibrary.Models;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GamesApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Game? game;
        private static Studio? studio;
        private static Genre? genre;
        //private static Game? releaseDate;

        private static Game selectedGame;

        public MainWindow()
        {
            InitializeComponent();
            ShowGames();
            deleteBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
        }

        private void Clear()
        {
            nameTxtBox.Clear();
            studioTxtBox.Clear();
            genreTxtBox.Clear();

            if (selectedGame != null)
            {
                selectedGame.Id = 0;
            }

            deleteBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
            addBtn.IsEnabled = true;
            searchBtn.IsEnabled = true;
        }

        private void ShowGames()
        {
            using (GamesContext? db = new())
            {
                dataGridView.ItemsSource = db.Games.ToList();
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string.IsNullOrWhiteSpace(nameTxtBox.Text) &&
                    string.IsNullOrWhiteSpace(studioTxtBox.Text) &&
                    string.IsNullOrWhiteSpace(genreTxtBox.Text)) ||
                    (string.IsNullOrWhiteSpace(releaseDatePicker.Text)))
                {
                    MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля перед додаванням книги.", "Ой", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (GamesContext? db = new())
                {
                    studio = db.Studios.SingleOrDefault(s => s.Name == studioTxtBox.Text);
                    if (studio == null)
                    {
                        studio = new Studio { Name = studioTxtBox.Text };
                        db.Studios.Add(studio);
                    }

                    genre = db.Genres.SingleOrDefault(ge => ge.Name == genreTxtBox.Text);
                    if (genre == null)
                    {
                        genre = new Genre { Name = genreTxtBox.Text };
                        db.Genres.Add(genre);
                    }

                    DateTime? releaseDate = releaseDatePicker.SelectedDate;

                    game = new Game
                    {
                        Name = nameTxtBox.Text,
                        Studio = studio,
                        Genre = genre,
                        ReleaseDate = releaseDate ?? DateTime.Now.Date
                    };

                    db.Games.Add(game);
                    db.SaveChanges();
                }

                MessageBox.Show("Дані додано успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowGames();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Clear();
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (GamesContext db = new())
                {
                    Game? gameToUpdate = db.Games.SingleOrDefault(g => g.Id == selectedGame.Id);

                    gameToUpdate.Name = nameTxtBox.Text;
                    
                    studio = db.Studios.SingleOrDefault(s => s.Name == studioTxtBox.Text);
                    if (studio == null)
                    {
                        studio = new Studio { Name = studioTxtBox.Text };
                        db.Studios.Add(studio);
                    }
                    gameToUpdate.Studio = studio;

                    genre = db.Genres.SingleOrDefault(ge => ge.Name == genreTxtBox.Text);
                    if (genre == null)
                    {
                        genre = new Genre { Name = genreTxtBox.Text };
                        db.Genres.Add(genre);
                    }
                    gameToUpdate.Genre = genre;

                    db.SaveChanges();
                }

                MessageBox.Show("Дані змінено успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowGames();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Clear();
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Ви дійсно хочете видалити цю гру?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (GamesContext db = new())
                    {
                        Game gameToDelete = db.Games.SingleOrDefault(g => g.Id == selectedGame.Id);

                        db.Games.Remove(gameToDelete);
                        db.SaveChanges();
                    }
                }

                MessageBox.Show("Дані видалено успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowGames();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Clear();
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = nameTxtBox.Text;
                string studio = studioTxtBox.Text;
                string genre = genreTxtBox.Text;

                using (GamesContext db = new())
                {
                    IQueryable<Game> query = db.Games.AsQueryable();

                    if (!string.IsNullOrEmpty(name))
                        query = query.Where(g => g.Name.Contains(name));
                    if (!string.IsNullOrEmpty(studio))
                        query = query.Where(g => g.Studio.Name.Contains(studio));
                    if (!string.IsNullOrEmpty(genre))
                        query = query.Where(g => g.Genre.Name.Contains(genre));

                    List<Game> games = query.ToList();

                    foreach (Game game in games)
                    {
                        db.Entry(game).Reference(g => g.Studio).Load();
                        db.Entry(game).Reference(g => g.Genre).Load();
                    }

                    dataGridView.ItemsSource = games;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridView.CurrentItem is Game game) 
                {
                    using (GamesContext db = new())
                    {
                        selectedGame = db.Games.SingleOrDefault(g => g.Id == game.Id);

                        if (selectedGame != null)
                        {
                            db.Entry(selectedGame).Reference(g => g.Studio).Load();
                            db.Entry(selectedGame).Reference(g => g.Genre).Load(); 

                            nameTxtBox.Text = selectedGame.Name;
                            studioTxtBox.Text = selectedGame.Studio.Name;
                            genreTxtBox.Text = selectedGame.Genre.Name;

                            updateBtn.IsEnabled = true;
                            deleteBtn.IsEnabled = true;
                            addBtn.IsEnabled = false;
                            searchBtn.IsEnabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}